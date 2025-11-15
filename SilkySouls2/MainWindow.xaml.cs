using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SilkySouls2.enums;
using SilkySouls2.Memory;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.ViewModels;
using SilkySouls2.Views;
using SilkySouls2.Views.Tabs;

namespace SilkySouls2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MemoryIo _memoryIo;
        private readonly AoBScanner _aobScanner;
        private readonly DispatcherTimer _gameLoadedTimer;
        private readonly HookManager _hookManager;
        private readonly NopManager _nopManager;
        private readonly GameStateService _gameStateService;

        private readonly PlayerViewModel _playerViewModel;
        private readonly TravelViewModel _travelViewModel;

        private readonly UtilityViewModel _utilityViewModel;
        private readonly TargetViewModel _targetViewModel;
        private readonly EnemyViewModel _enemyViewModel;
        private readonly EventViewModel _eventViewModel;
        private readonly ItemViewModel _itemViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly DamageControlService _damageControlService;
        private readonly DllManager _dllManager;
        private readonly ItemService _itemService;

        public MainWindow()
        {
            _memoryIo = new MemoryIo();
            _memoryIo.StartAutoAttach();

            InitializeComponent();


            if (SettingsManager.Default.WindowLeft != 0 || SettingsManager.Default.WindowTop != 0)
            {
                Left = SettingsManager.Default.WindowLeft;
                Top = SettingsManager.Default.WindowTop;
            }
            else WindowStartupLocation = WindowStartupLocation.CenterScreen;


            _hookManager = new HookManager(_memoryIo);
            _nopManager = new NopManager(_memoryIo);
            _aobScanner = new AoBScanner(_memoryIo);
            _dllManager = new DllManager(_memoryIo);
            var hotkeyManager = new HotkeyManager(_memoryIo);

            _damageControlService = new DamageControlService(_memoryIo, _hookManager);
            var playerService = new PlayerService(_memoryIo, _hookManager, _nopManager);
            var utilityService = new UtilityService(_memoryIo, _hookManager, _dllManager);
            var travelService = new TravelService(_memoryIo, _hookManager, playerService);
            var targetService = new TargetService(_memoryIo, _hookManager);
            var enemyService = new EnemyService(_memoryIo, _hookManager);
            var eventService = new EventService(_memoryIo);
            _gameStateService = new GameStateService();
            var ezStateService = new EzStateService(_memoryIo, _hookManager);
            _itemService = new ItemService(_memoryIo);
            var settingsService = new SettingsService(_memoryIo, _hookManager);

            _playerViewModel = new PlayerViewModel(playerService, hotkeyManager, _damageControlService, _gameStateService);
            _travelViewModel = new TravelViewModel(travelService, hotkeyManager);
            _eventViewModel = new EventViewModel(utilityService, eventService);
            _utilityViewModel = new UtilityViewModel(utilityService, hotkeyManager, _playerViewModel, _gameStateService);
            _targetViewModel = new TargetViewModel(targetService, hotkeyManager, _damageControlService);
            _enemyViewModel = new EnemyViewModel(enemyService, hotkeyManager, ezStateService, _gameStateService, eventService);
            _itemViewModel = new ItemViewModel(_itemService);
            _settingsViewModel = new SettingsViewModel(settingsService, hotkeyManager, _gameStateService);

            var playerTab = new PlayerTab(_playerViewModel);
            var travelTab = new TravelTab(_travelViewModel);
            var eventTab = new EventTab(_eventViewModel);
            var utilityTab = new UtilityTab(_utilityViewModel);
            var targetTab = new TargetTab(_targetViewModel);
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

            _settingsViewModel.ApplyStartUpOptions();
            _utilityViewModel.ApplyStartUpOptions();
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
        private bool _hasAppliedLaunchFeatures;
        private int _storedArea;
        private int _currentArea;
        private bool _hasShownVersionError;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_memoryIo.IsAttached)
            {
                IsAttachedText.Text = "Attached to game";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];

                LaunchGameButton.IsEnabled = false;

                if (!_hasScanned)
                {
                    if (!GameVersion.TryDetectVersion(_memoryIo.GetFileSize()))
                    {
                        _memoryIo.Detach();

                        Dispatcher.Invoke(() =>
                        {
                            if (!_hasShownVersionError)
                            {
                                _hasShownVersionError = true;
                                MessageBox.Show(
                                    "Unknown game version detected. Please ensure you're running a supported version of Dark Souls 2.");
                            }
                            
                            IsAttachedText.Foreground =
                                (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
                            LaunchGameButton.IsEnabled = true;
                        });
                        return;
                    }

                    _hasShownVersionError = false;
                    _aobScanner.Scan(GameVersion.Current.Edition == GameEdition.Scholar);
                    _hasScanned = true;
                    Offsets.Initialize(GameVersion.Current.Edition);
                    Console.WriteLine($"Base: 0x{_memoryIo.BaseAddress.ToInt64():X}");
                }


                if (!_hasAppliedLaunchFeatures)
                {
                    _gameStateService.Publish(GameState.Launched);
                    ApplyLaunchFeatures();
                }


                _currentArea = _memoryIo.ReadInt32(Offsets.MapId);
                if (_currentArea != _storedArea)
                {
                    _eventViewModel.AreaChange(_currentArea);
                    _storedArea = _currentArea;
                }

                if (!_hasAllocatedMemory)
                {
                    _memoryIo.AllocCodeCave();
                    Console.WriteLine($"Code cave: 0x{CodeCaveOffsets.Base.ToInt64():X}");
                    _hasAllocatedMemory = true;
                    _damageControlService.WriteDamageControlCode();
                    _dllManager.CreateDrawSharedMem();
                    _dllManager.CreateSpeedSharedMem();
                }

                if (_memoryIo.IsGameLoaded())
                {
                    if (!_hasAppliedDelayedFeatures)
                    {
                        if (!_memoryIo.IsLoadingScreen())
                        {
                            _hasAppliedDelayedFeatures = true;
                            _gameStateService.Publish(GameState.DelayedGameLoad);
                        }
                    }
                    if (_loaded) return;
                    _loaded = true;
                    _gameStateService.Publish(GameState.Loaded);
                    TryEnableFeatures();
                    
                    if (_appliedOneTimeFeatures) return;
                    _gameStateService.Publish(GameState.FirstLoaded);
                    ApplyOneTimeFeatures();
                    _appliedOneTimeFeatures = true;
                }
                else if (_loaded)
                {
                    _gameStateService.Publish(GameState.NotLoaded);
                    DisableFeatures();
                    _loaded = false;
                    _hasAppliedDelayedFeatures = false;
                }
            }
            else
            {
                _hookManager.ClearHooks();
                _gameStateService.Publish(GameState.NotLoaded);
                DisableFeatures();
                _nopManager.ClearRegistry();
                _gameStateService.Publish(GameState.Detached);
                ResetState();
                _hasScanned = false;
                _loaded = false;
                _hasAllocatedMemory = false;
                _appliedOneTimeFeatures = false;
                IsAttachedText.Text = "Not attached";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
                LaunchGameButton.IsEnabled = true;
            }
        }

        private void ResetState()
        {
            _dllManager.ResetState();
            _settingsViewModel.ResetAttached();
            _itemService.Reset();
        }

        private void ApplyLaunchFeatures()
        {
            _itemViewModel.ApplyLaunchFeatures();
        }

        private void ApplyOneTimeFeatures()
        {
            _enemyViewModel.TryApplyOneTimeFeatures();
            _eventViewModel.TryApplyOneTimeFeatures();
        }

        private void TryEnableFeatures()
        {
      
            _targetViewModel.TryEnableFeatures();
            _itemViewModel.TryEnableFeatures();
            _travelViewModel.TryEnableFeatures();
            _eventViewModel.TryEnableFeatures();
        }

        private void DisableFeatures()
        {
            _targetViewModel.DisableFeatures();
            _itemViewModel.DisableFeatures();
            _travelViewModel.DisableFeatures();
            _eventViewModel.DisableFeatures();
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

        private void LaunchGame_Click(object sender, RoutedEventArgs e) => Task.Run(GameLauncher.LaunchDarkSouls2);
        private void CheckUpdate_Click(object sender, RoutedEventArgs e) => VersionChecker.CheckForUpdates(this, true);
    }
}