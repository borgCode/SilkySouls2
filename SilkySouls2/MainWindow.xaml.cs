using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SilkySouls2.Memory;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.ViewModels;
using SilkySouls2.Views;

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

        private readonly PlayerViewModel _playerViewModel;
        private readonly TravelViewModel _travelViewModel;

        private readonly UtilityViewModel _utilityViewModel;
        private readonly EnemyViewModel _enemyViewModel;
        private readonly EventViewModel _eventViewModel;
        private readonly ItemViewModel _itemViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly DamageControlService _damageControlService;
        private readonly DllManager _dllManager;

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
            var travelService = new TravelService(_memoryIo, _hookManager, utilityService);
            var enemyService = new EnemyService(_memoryIo, _hookManager, _damageControlService);
            var itemService = new ItemService(_memoryIo, _hookManager);
            var settingsService = new SettingsService(_memoryIo, _hookManager);

            _playerViewModel = new PlayerViewModel(playerService, hotkeyManager, _damageControlService);
            _travelViewModel = new TravelViewModel(travelService, hotkeyManager);
            _eventViewModel = new EventViewModel(utilityService);
            _utilityViewModel = new UtilityViewModel(utilityService, hotkeyManager, _playerViewModel);
            _enemyViewModel = new EnemyViewModel(enemyService, hotkeyManager, _damageControlService);
            _itemViewModel = new ItemViewModel(itemService);
            _settingsViewModel = new SettingsViewModel(settingsService, hotkeyManager);

            var playerTab = new PlayerTab(_playerViewModel);
            var travelTab = new TravelTab(_travelViewModel);
            var eventTab = new EventTab(_eventViewModel);
            var utilityTab = new UtilityTab(_utilityViewModel);
            var enemyTab = new EnemyTab(_enemyViewModel);
            var itemTab = new ItemTab(_itemViewModel);
            var settingsTab = new SettingsTab(_settingsViewModel);

            MainTabControl.Items.Add(new TabItem { Header = "Player", Content = playerTab });
            MainTabControl.Items.Add(new TabItem { Header = "Travel", Content = travelTab });
            MainTabControl.Items.Add(new TabItem { Header = "Event", Content = eventTab });
            MainTabControl.Items.Add(new TabItem { Header = "Utility", Content = utilityTab });
            MainTabControl.Items.Add(new TabItem { Header = "Enemies", Content = enemyTab });
            MainTabControl.Items.Add(new TabItem { Header = "Items", Content = itemTab });
            MainTabControl.Items.Add(new TabItem { Header = "Settings", Content = settingsTab });

            _settingsViewModel.ApplyStartUpOptions();
            Closing += MainWindow_Closing;

            _gameLoadedTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            _gameLoadedTimer.Tick += Timer_Tick;
            _gameLoadedTimer.Start();


            // VersionChecker.CheckForUpdates(AppVersion, this);
        }

        private bool _loaded;

        private bool _hasScanned;

        private bool _hasAllocatedMemory;

        private bool _hasAppliedNoLogo;

        private bool _appliedOneTimeFeatures;


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_memoryIo.IsAttached)
            {
                IsAttachedText.Text = "Attached to game";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];

                // LaunchGameButton.IsEnabled = false;


                if (!_hasScanned)
                {
                    _aobScanner.Scan();
                    _hasScanned = true;
                    Console.WriteLine($"Base: 0x{_memoryIo.BaseAddress.ToInt64():X}");
                }

                //
                // if (!_hasAppliedNoLogo)
                // {
                //     _memoryIo.WriteBytes(Offsets.Patches.NoLogo, AsmLoader.GetAsmBytes("NoLogo"));
                //     _hasAppliedNoLogo = true;
                // }

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
                    if (_loaded) return;
                    _loaded = true;
                    TryEnableFeatures();
                    // TrySetGameStartPrefs();
                    _settingsViewModel.ApplyLoadedOptions();
                    if (_appliedOneTimeFeatures) return;
                    ApplyOneTimeFeatures();
                    _appliedOneTimeFeatures = true;
                }
                else if (_loaded)
                {
                    DisableFeatures();
                    _loaded = false;
                }
            }
            else
            {
                _hookManager.ClearHooks();
                DisableFeatures();
                _nopManager.ClearRegistry();
                // _settingsViewModel.ResetAttached();
                _loaded = false;
                _hasAllocatedMemory = false;
                // _hasAppliedNoLogo = false;
                _appliedOneTimeFeatures = false;
                IsAttachedText.Text = "Not attached";
                IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
                // LaunchGameButton.IsEnabled = true;
            }
        }

        private void ApplyOneTimeFeatures()
        {
            _playerViewModel.TryApplyOneTimeFeatures();
            _utilityViewModel.TryApplyOneTimeFeatures();
            _enemyViewModel.TrryApplyOneTimeFeatures();
        }

        private void TryEnableFeatures()
        {
            _playerViewModel.TryEnableFeatures();
            _utilityViewModel.TryEnableFeatures();
            _enemyViewModel.TryEnableFeatures();
        }

        private void DisableFeatures()
        {
            _utilityViewModel.DisableFeatures();
            _playerViewModel.DisableFeatures();
            _enemyViewModel.DisableFeatures();
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


        // private void LaunchGame_Click(object sender, RoutedEventArgs e)
        // {
        //     string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "Dll4.dll");
        //     Console.WriteLine($"Injecting DLL from: {dllPath}");
        //     bool success = _memoryIo.InjectDll(dllPath);
        //     Console.WriteLine($"Injection {(success ? "successful" : "failed")}");
        // }
        private void LaunchGame_Click(object sender, RoutedEventArgs e) => Task.Run(GameLauncher.LaunchDarkSouls2);
    }
}