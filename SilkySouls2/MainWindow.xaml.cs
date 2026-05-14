using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.ViewModels;
using SilkySouls2.Views.Tabs;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IMemoryService _memoryService;
        private readonly AoBScanner _aobScanner;
        private readonly DispatcherTimer _gameLoadedTimer;
        private readonly StateService _stateService;

        private readonly INewGameService _newGameService;

        private const string VanillaSteamName = "Dark Souls II";
        private const string ScholarSteamName = "Dark Souls II Scholar of the First Sin";

        public MainWindow()
        {
            _memoryService = new MemoryService();
            _memoryService.StartAutoAttach();

            InitializeComponent();


            var savedLeft = SettingsManager.Default.WindowLeft;
            var savedTop = SettingsManager.Default.WindowTop;
            if ((savedLeft != 0 || savedTop != 0) && IsOnVisibleScreen(savedLeft, savedTop))
            {
                Left = savedLeft;
                Top = savedTop;
            }
            else WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _stateService = new StateService(_memoryService);

            var hookManager = new HookManager(_memoryService, _stateService);
            _aobScanner = new AoBScanner(_memoryService);
            var dllManager = new DllManager(_memoryService, _stateService);
            var hotkeyManager = new HotkeyManager(_memoryService);


            IGameTickService gameTickService = new GameTickService(_stateService);
            IMenuService menuService = new MenuService(_memoryService, gameTickService);
            IReminderService reminderService = new ReminderService(_memoryService, hookManager, _stateService);
            IDamageControlService damageControlService = new DamageControlService(_memoryService, hookManager, _stateService);
            IChrCtrlService chrCtrlService = new ChrCtrlService(_memoryService);
            IPlayerService playerService =
                new PlayerService(_memoryService, hookManager, chrCtrlService, reminderService);
            IUtilityService utilityService = new UtilityService(_memoryService, hookManager, dllManager);
            ITravelService travelService = new TravelService(_memoryService, hookManager, playerService);
            ITargetService targetService = new TargetService(_memoryService, hookManager, chrCtrlService);
            IEnemyService enemyService = new EnemyService(_memoryService, hookManager);
            IEventService eventService = new EventService(_memoryService);
            IEzStateService ezStateService = new EzStateService(_memoryService, hookManager);
            IItemService itemService = new ItemService(_memoryService, _stateService);
            _newGameService = new NewGameService(_memoryService, hookManager, _stateService);
            ISettingsService settingsService = new SettingsService(_memoryService, hookManager);

            var playerViewModel = new PlayerViewModel(playerService, hotkeyManager, damageControlService,
                _stateService, _newGameService, gameTickService);

            var travelViewModel = new TravelViewModel(travelService, hotkeyManager, _stateService);

            var eventViewModel = new EventViewModel(utilityService, eventService, ezStateService, _stateService);

            var utilityViewModel = new UtilityViewModel(utilityService, hotkeyManager, playerViewModel,
                _stateService, ezStateService, menuService);

            var targetViewModel =
                new TargetViewModel(targetService, hotkeyManager, damageControlService, _stateService, reminderService,
                    gameTickService);

            var enemyViewModel = new EnemyViewModel(enemyService, hotkeyManager, ezStateService, _stateService,
                eventService);

            var itemViewModel = new ItemViewModel(itemService, _stateService, _newGameService);

            var settingsViewModel = new SettingsViewModel(settingsService, hotkeyManager, _stateService);

            var playerTab = new PlayerTab(playerViewModel);
            var travelTab = new TravelTab(travelViewModel);
            var eventTab = new EventTab(eventViewModel);
            var utilityTab = new UtilityTab(utilityViewModel);
            var targetTab = new TargetTab(targetViewModel);
            var enemyTab = new EnemyTab(enemyViewModel);
            var itemTab = new ItemTab(itemViewModel);
            var settingsTab = new SettingsTab(settingsViewModel);


            MainTabControl.Items.Add(new TabItem { Header = "Player", Content = playerTab });
            MainTabControl.Items.Add(new TabItem { Header = "Travel", Content = travelTab });
            MainTabControl.Items.Add(new TabItem { Header = "Enemies", Content = enemyTab });
            MainTabControl.Items.Add(new TabItem { Header = "Target", Content = targetTab });
            MainTabControl.Items.Add(new TabItem { Header = "Utility", Content = utilityTab });
            MainTabControl.Items.Add(new TabItem { Header = "Event", Content = eventTab });
            MainTabControl.Items.Add(new TabItem { Header = "Items", Content = itemTab });
            MainTabControl.Items.Add(new TabItem { Header = "Settings", Content = settingsTab });

            _stateService.Publish(State.AppStart);

            Closing += MainWindow_Closing;

            _gameLoadedTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(25)
            };
            _gameLoadedTimer.Tick += Timer_Tick;
            _gameLoadedTimer.Start();

            VersionChecker.UpdateVersionText(AppVersion);

            if (SettingsManager.Default.EnableUpdateChecks)
            {
                VersionChecker.CheckForUpdates(this);
            }
        }

        private bool _loaded;
        private bool _hasAppliedDelayedFeatures;
        private bool _appliedOneTimeFeatures;
        private int _storedArea;
        private int _currentArea;
        private bool _waitingForDetectNewGame;
        private bool _wasAttached;
        private bool _isReady;
        private CancellationTokenSource _attachCts;

        private void Timer_Tick(object sender, EventArgs e)
        {
            var isAttached = _memoryService.IsAttached;

            if (isAttached != _wasAttached)
            {
                _wasAttached = isAttached;
                if (isAttached) OnAttached();
                else OnDetached();
            }

            if (!isAttached || !_isReady) return;

            _currentArea = _memoryService.Read<int>(MapId);
            if (_currentArea != _storedArea)
            {
                _stateService.Publish(State.AreaChanged, _currentArea);
                _storedArea = _currentArea;
            }

            if (_waitingForDetectNewGame && _newGameService.PollNewGameStarted())
            {
                _waitingForDetectNewGame = false;
            }

            if (_stateService.IsGameLoaded())
            {
                if (!_hasAppliedDelayedFeatures)
                {
                    if (!_stateService.IsLoadingScreen())
                    {
                        _hasAppliedDelayedFeatures = true;
                        _stateService.Publish(State.DelayedGameLoad);
                    }
                }

                if (_loaded) return;
                _loaded = true;

                if (_newGameService.GetCount() > 0 && _newGameService.IsAtNewGameStartCutscene())
                {
                    _waitingForDetectNewGame = true;
                }

                _stateService.Publish(State.Loaded);

                if (_appliedOneTimeFeatures) return;
                _stateService.Publish(State.FirstLoaded);
                _appliedOneTimeFeatures = true;
            }
            else if (_loaded)
            {
                _stateService.Publish(State.NotLoaded);
                _loaded = false;
                _hasAppliedDelayedFeatures = false;
            }
        }

        private void OnAttached()
        {
            IsAttachedText.Text = "Attached to game";
            IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];
            LaunchVanillaButton.IsEnabled = false;
            LaunchScholarButton.IsEnabled = false;

            _attachCts = new CancellationTokenSource();
            _ = RunAttachSequenceAsync(_attachCts.Token);
        }

        private void OnDetached()
        {
            _attachCts?.Cancel();
            _isReady = false;
            _loaded = false;
            _hasAppliedDelayedFeatures = false;
            _appliedOneTimeFeatures = false;
            _stateService.Publish(State.NotLoaded);
            _stateService.Publish(State.Detached);
            IsAttachedText.Text = "Not attached";
            IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
            LaunchVanillaButton.IsEnabled = true;
            LaunchScholarButton.IsEnabled = true;
        }

        private async Task RunAttachSequenceAsync(CancellationToken ct)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), ct);

                if (!PatchManager.TryDetectVersion(_memoryService))
                {
                    _aobScanner.FallBackScan();
                }

#if DEBUG
                Console.WriteLine($@"Base: 0x{(long)_memoryService.BaseAddress:X}");
#endif

                ct.ThrowIfCancellationRequested();
                _memoryService.AllocCodeCave();

#if DEBUG
                Console.WriteLine($"Code cave: 0x{(long)CustomCodeOffsets.Base:X}");
#endif

                _stateService.Publish(State.Attached);

                ct.ThrowIfCancellationRequested();
                _isReady = true;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attach sequence failed: {ex}");
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            SettingsManager.Default.WindowLeft = Left;
            SettingsManager.Default.WindowTop = Top;
            SettingsManager.Default.Save();
        }

        private void CheckUpdate_Click(object sender, RoutedEventArgs e) => VersionChecker.CheckForUpdates(this, true);

        private static bool IsOnVisibleScreen(double left, double top)
        {
            const double minVisibleX = 100;
            const double minVisibleY = 30;
            var vLeft = SystemParameters.VirtualScreenLeft;
            var vTop = SystemParameters.VirtualScreenTop;
            var vRight = vLeft + SystemParameters.VirtualScreenWidth;
            var vBottom = vTop + SystemParameters.VirtualScreenHeight;
            return left + minVisibleX > vLeft
                   && left < vRight - minVisibleX
                   && top + minVisibleY > vTop
                   && top < vBottom - minVisibleY;
        }

        private void LaunchVanilla_Click(object sender, RoutedEventArgs e) =>
            Task.Run(() => GameLauncher.LaunchDarkSouls2(VanillaSteamName));

        private void LaunchScholar_Click(object sender, RoutedEventArgs e) =>
            Task.Run(() => GameLauncher.LaunchDarkSouls2(ScholarSteamName));
    }
}