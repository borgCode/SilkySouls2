using System;
using System.ComponentModel;
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
        private readonly HookManager _hookManager;
        private readonly NopManager _nopManager;
        private readonly GameStateService _gameStateService;

        private readonly EnemyViewModel _enemyViewModel;
        private readonly ItemViewModel _itemViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly DamageControlService _damageControlService;
        private readonly DllManager _dllManager;
        private readonly ItemService _itemService;
        private readonly INewGameService _newGameService;

        private const string VanillaSteamName = "Dark Souls II";
        private const string ScholarSteamName = "Dark Souls II Scholar of the First Sin";
        private const int StartingCutsceneId = 100200;

        public MainWindow()
        {
            _memoryService = new MemoryService();
            _memoryService.StartAutoAttach();

            InitializeComponent();


            if (SettingsManager.Default.WindowLeft != 0 || SettingsManager.Default.WindowTop != 0)
            {
                Left = SettingsManager.Default.WindowLeft;
                Top = SettingsManager.Default.WindowTop;
            }
            else WindowStartupLocation = WindowStartupLocation.CenterScreen;


            _hookManager = new HookManager(_memoryService);
            _nopManager = new NopManager(_memoryService);
            _aobScanner = new AoBScanner(_memoryService);
            _dllManager = new DllManager(_memoryService);
            var hotkeyManager = new HotkeyManager(_memoryService);

            _gameStateService = new GameStateService();
            _damageControlService = new DamageControlService(_memoryService, _hookManager);
            IChrCtrlService chrCtrlService = new ChrCtrlService(_memoryService);
            IPlayerService playerService = new PlayerService(_memoryService, _hookManager, _nopManager, chrCtrlService);
            var utilityService = new UtilityService(_memoryService, _hookManager, _dllManager);
            ITravelService travelService = new TravelService(_memoryService, _hookManager, playerService);
            var targetService = new TargetService(_memoryService, _hookManager, chrCtrlService);
            var enemyService = new EnemyService(_memoryService, _hookManager);
            IEventService eventService = new EventService(_memoryService);
            IEzStateService ezStateService = new EzStateService(_memoryService, _hookManager);
            _itemService = new ItemService(_memoryService);
            _newGameService = new NewGameService(_memoryService, _hookManager, _gameStateService);
            ISettingsService settingsService = new SettingsService(_memoryService, _hookManager);

            var playerViewModel = new PlayerViewModel(playerService, hotkeyManager, _damageControlService,
                _gameStateService, _newGameService);

            var travelViewModel = new TravelViewModel(travelService, hotkeyManager, _gameStateService);

            var eventViewModel = new EventViewModel(utilityService, eventService, ezStateService, _gameStateService);

            var utilityViewModel = new UtilityViewModel(utilityService, hotkeyManager, playerViewModel,
                _gameStateService, ezStateService);

            var targetViewModel =
                new TargetViewModel(targetService, hotkeyManager, _damageControlService, _gameStateService);

            _enemyViewModel = new EnemyViewModel(enemyService, hotkeyManager, ezStateService, _gameStateService,
                eventService);

            _itemViewModel = new ItemViewModel(_itemService, _gameStateService, _newGameService);

            _settingsViewModel = new SettingsViewModel(settingsService, hotkeyManager, _gameStateService);

            var playerTab = new PlayerTab(playerViewModel);
            var travelTab = new TravelTab(travelViewModel);
            var eventTab = new EventTab(eventViewModel);
            var utilityTab = new UtilityTab(utilityViewModel);
            var targetTab = new TargetTab(targetViewModel);
            var enemyTab = new EnemyTab(_enemyViewModel);
            var itemTab = new ItemTab(_itemViewModel);
            var settingsTab = new SettingsTab(_settingsViewModel);


            MainTabControl.Items.Add(new TabItem { Header = "Player", Content = playerTab });
            MainTabControl.Items.Add(new TabItem { Header = "Travel", Content = travelTab });
            MainTabControl.Items.Add(new TabItem { Header = "Event", Content = eventTab });
            MainTabControl.Items.Add(new TabItem { Header = "Utility", Content = utilityTab });
            MainTabControl.Items.Add(new TabItem { Header = "Target", Content = targetTab });
            MainTabControl.Items.Add(new TabItem { Header = "Enemies", Content = enemyTab });
            MainTabControl.Items.Add(new TabItem { Header = "Items", Content = itemTab });
            MainTabControl.Items.Add(new TabItem { Header = "Settings", Content = settingsTab });

            _gameStateService.Publish(GameState.AppStart);

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

        private bool _hasScanned;

        private bool _hasAllocatedMemory;

        private bool _hasAppliedNoLogo;

        private bool _appliedOneTimeFeatures;
        private int _storedArea;
        private int _currentArea;
        private bool _hasCheckedPatch;
        private bool _waitingForDetectNewGame;
        private DateTime? _attachedTime;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_memoryService.IsAttached)
            {
                IsAttachedText.Text = "Attached to game";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];

                LaunchVanillaButton.IsEnabled = false;
                LaunchScholarButton.IsEnabled = false;

                if (!_attachedTime.HasValue)
                {
                    _attachedTime = DateTime.Now;
                    return;
                }

                if ((DateTime.Now - _attachedTime.Value).TotalSeconds < 2)
                    return;

                if (!_hasCheckedPatch)
                {
                    if (!PatchManager.TryDetectVersion(_memoryService))
                    {
                        _aobScanner.FallBackScan(true);
                    }

#if DEBUG
                    Console.WriteLine($@"Base: 0x{(long)_memoryService.BaseAddress:X}");
#endif
                    _hasCheckedPatch = true;
                }

                _currentArea = _memoryService.Read<int>(MapId);
                if (_currentArea != _storedArea)
                {
                    _gameStateService.Publish(GameState.AreaChanged, _currentArea);
                    _storedArea = _currentArea;
                }

                if (!_hasAllocatedMemory)
                {
                    _memoryService.AllocCodeCave();

#if DEBUG
                    Console.WriteLine($"Code cave: 0x{CodeCaveOffsets.Base.ToInt64():X}");
#endif
                    _hasAllocatedMemory = true;
                    _damageControlService.WriteDamageControlCode();
                    _dllManager.CreateDrawSharedMem();
                    _dllManager.CreateSpeedSharedMem();
                    _gameStateService.Publish(GameState.Attached);
                }

                if (_waitingForDetectNewGame)
                {
                    var newGameFlagLoc = CodeCaveOffsets.Base + CodeCaveOffsets.NewGameStartedFlag;
                    if (_memoryService.Read<byte>(newGameFlagLoc) == 1)
                    {
                        _gameStateService.Publish(GameState.NewGameStarted);
                        _memoryService.Write(newGameFlagLoc, (byte)0);
                        _waitingForDetectNewGame = false;
                    }
                }

                if (_memoryService.IsGameLoaded())
                {
                    if (!_hasAppliedDelayedFeatures)
                    {
                        if (!_memoryService.IsLoadingScreen())
                        {
                            _hasAppliedDelayedFeatures = true;
                            _gameStateService.Publish(GameState.DelayedGameLoad);
                        }
                    }

                    if (_loaded) return;
                    _loaded = true;

                    if (_newGameService.GetCount() > 0)
                    {
                        var gameMan = _memoryService.Read<nint>(GameManagerImp.Base);

                        if (_memoryService.Read<int>((IntPtr)gameMan + GameManagerImp.PendingCutsceneId) ==
                            StartingCutsceneId)
                        {
                            _waitingForDetectNewGame = true;
                        }
                    }


                    _gameStateService.Publish(GameState.Loaded);

                    if (_appliedOneTimeFeatures) return;
                    _gameStateService.Publish(GameState.FirstLoaded);
                    ApplyOneTimeFeatures();
                    _appliedOneTimeFeatures = true;
                }
                else if (_loaded)
                {
                    _gameStateService.Publish(GameState.NotLoaded);
                    _loaded = false;
                    _hasAppliedDelayedFeatures = false;
                }
            }
            else
            {
                _hookManager.ClearHooks();
                _gameStateService.Publish(GameState.NotLoaded);
                _nopManager.ClearRegistry();
                _gameStateService.Publish(GameState.Detached);
                _newGameService.Reset();
                _attachedTime = null;
                _hasCheckedPatch = false;
                ResetState();
                _hasScanned = false;
                _loaded = false;
                _hasAllocatedMemory = false;
                _appliedOneTimeFeatures = false;
                IsAttachedText.Text = "Not attached";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
                LaunchVanillaButton.IsEnabled = true;
                LaunchScholarButton.IsEnabled = true;
            }
        }

        private void ResetState()
        {
            _dllManager.ResetState();
            _settingsViewModel.ResetAttached();
            _itemService.Reset();
        }

        private void ApplyOneTimeFeatures()
        {
            _enemyViewModel.TryApplyOneTimeFeatures();
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
            _itemService.SignalClose();
            _hookManager.UninstallAllHooks();
            _nopManager.RestoreAll();
        }

        private void CheckUpdate_Click(object sender, RoutedEventArgs e) => VersionChecker.CheckForUpdates(this, true);

        private void LaunchVanilla_Click(object sender, RoutedEventArgs e) =>
            Task.Run(() => GameLauncher.LaunchDarkSouls2(VanillaSteamName));

        private void LaunchScholar_Click(object sender, RoutedEventArgs e) =>
            Task.Run(() => GameLauncher.LaunchDarkSouls2(ScholarSteamName));
    }
}