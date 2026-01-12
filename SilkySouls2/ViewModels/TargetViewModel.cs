using System;
using System.Windows.Threading;
using SilkySouls2.enums;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.Views;
using SilkySouls2.Views.Windows;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.ViewModels
{
    public class TargetViewModel : BaseViewModel
    {
        private readonly DispatcherTimer _targetOptionsTimer;

        private long _currentTargetChrCtrl;

        private ResistancesWindow _resistancesWindowWindow;

        private DefenseWindow _defenseWindow;

        private readonly TargetService _targetService;
        private readonly DamageControlService _damageControlService;
        private readonly HotkeyManager _hotkeyManager;

        public TargetViewModel(TargetService targetService, HotkeyManager hotkeyManager,
            DamageControlService damageControlService, GameStateService gameStateService)
        {
            _targetService = targetService;
            _damageControlService = damageControlService;
            _hotkeyManager = hotkeyManager;
            RegisterHotkeys();

            gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
            gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);


            _targetOptionsTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(64)
            };
            _targetOptionsTimer.Tick += TargetOptionsTimerTick;
        }

        #region Commands

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private bool _isValidTarget;

        public bool IsValidTarget
        {
            get => _isValidTarget;
            set => SetProperty(ref _isValidTarget, value);
        }

        private bool _isTargetOptionsEnabled;

        public bool IsTargetOptionsEnabled
        {
            get => _isTargetOptionsEnabled;
            set
            {
                if (!SetProperty(ref _isTargetOptionsEnabled, value)) return;
                if (value)
                {
                    _targetService.ToggleTargetHook(true);
                    _targetOptionsTimer.Start();
                    _targetService.ToggleCurrentActHook(true);
                    ShowAllResistances = true;
                }
                else
                {
                    _targetOptionsTimer.Stop();
                    _targetService.ToggleCurrentActHook(false);
                    ShowAllResistances = false;
                    IsResistancesWindowOpen = false;
                    IsFreezeHealthEnabled = false;
                    _targetService.ToggleTargetHook(false);
                    ShowHeavyPoise = false;
                    ShowLightPoise = false;
                    ShowBleed = false;
                    ShowPoison = false;
                    ShowToxic = false;
                }
            }
        }

        private bool _isDisableTargetAiEnabled;

        public bool IsDisableTargetAiEnabled
        {
            get => _isDisableTargetAiEnabled;
            set
            {
                if (SetProperty(ref _isDisableTargetAiEnabled, value))
                {
                    _targetService.ToggleTargetAi(_isDisableTargetAiEnabled);
                }
            }
        }

        private bool _isResistancesWindowOpen;

        public bool IsResistancesWindowOpen
        {
            get => _isResistancesWindowOpen;
            set
            {
                if (!SetProperty(ref _isResistancesWindowOpen, value)) return;
                if (value)
                    OpenResistancesWindow();
                else
                    CloseResistancesWindow();
            }
        }

        private bool _isRepeatActEnabled;

        public bool IsRepeatActEnabled
        {
            get => _isRepeatActEnabled;
            set
            {
                if (!SetProperty(ref _isRepeatActEnabled, value)) return;
                _targetService.ToggleRepeatAct(_isRepeatActEnabled);
            }
        }

        private int _lastAct;

        public int LastAct
        {
            get => _lastAct;
            set => SetProperty(ref _lastAct, value);
        }

        private int _targetCurrentHealth;

        public int TargetCurrentHealth
        {
            get => _targetCurrentHealth;
            set => SetProperty(ref _targetCurrentHealth, value);
        }

        private int _targetMaxHealth;

        public int TargetMaxHealth
        {
            get => _targetMaxHealth;
            set => SetProperty(ref _targetMaxHealth, value);
        }

        private float _targetSpeed;

        public float TargetSpeed
        {
            get => _targetSpeed;
            set
            {
                if (SetProperty(ref _targetSpeed, value))
                {
                    _targetService.SetTargetSpeed(value);
                }
            }
        }

        private bool _isFreezeHealthEnabled;

        public bool IsFreezeHealthEnabled
        {
            get => _isFreezeHealthEnabled;
            set
            {
                SetProperty(ref _isFreezeHealthEnabled, value);
                _damageControlService.ToggleFreezeTargetHp(_isFreezeHealthEnabled);
            }
        }

        public bool ShowLightPoiseAndNotImmune => ShowLightPoise && !IsLightPoiseImmune;
        public bool ShowBleedAndNotImmune => ShowBleed && !IsBleedImmune;
        public bool ShowPoisonAndNotImmune => ShowPoison && !IsPoisonToxicImmune;
        public bool ShowToxicAndNotImmune => ShowToxic && !IsPoisonToxicImmune;
        private float _targetCurrentHeavyPoise;

        public float TargetCurrentHeavyPoise
        {
            get => _targetCurrentHeavyPoise;
            set => SetProperty(ref _targetCurrentHeavyPoise, value);
        }

        private float _targetMaxHeavyPoise;

        public float TargetMaxHeavyPoise
        {
            get => _targetMaxHeavyPoise;
            set => SetProperty(ref _targetMaxHeavyPoise, value);
        }

        private bool _showHeavyPoise;

        public bool ShowHeavyPoise
        {
            get => _showHeavyPoise;
            set
            {
                SetProperty(ref _showHeavyPoise, value);
                if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                _resistancesWindowWindow.DataContext = null;
                _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _showLightPoise;

        public bool ShowLightPoise
        {
            get => _showLightPoise;
            set
            {
                SetProperty(ref _showLightPoise, value);
                if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                _resistancesWindowWindow.DataContext = null;
                _resistancesWindowWindow.DataContext = this;
            }
        }

        private float _targetCurrentLightPoise;

        public float TargetCurrentLightPoise
        {
            get => _targetCurrentLightPoise;
            set => SetProperty(ref _targetCurrentLightPoise, value);
        }

        private float _targetMaxLightPoise;

        public float TargetMaxLightPoise
        {
            get => _targetMaxLightPoise;
            set => SetProperty(ref _targetMaxLightPoise, value);
        }

        private bool _isLightPoiseImmune;

        public bool IsLightPoiseImmune
        {
            get => _isLightPoiseImmune;
            set => SetProperty(ref _isLightPoiseImmune, value);
        }

        private float _targetCurrentBleed;

        public float TargetCurrentBleed
        {
            get => _targetCurrentBleed;
            set => SetProperty(ref _targetCurrentBleed, value);
        }

        private float _targetMaxBleed;

        public float TargetMaxBleed
        {
            get => _targetMaxBleed;
            set => SetProperty(ref _targetMaxBleed, value);
        }

        private bool _showBleed;

        public bool ShowBleed
        {
            get => _showBleed;
            set
            {
                SetProperty(ref _showBleed, value);
                if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                _resistancesWindowWindow.DataContext = null;
                _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _isBleedImmune;

        public bool IsBleedImmune
        {
            get => _isBleedImmune;
            set => SetProperty(ref _isBleedImmune, value);
        }

        private float _targetCurrentPoison;

        public float TargetCurrentPoison
        {
            get => _targetCurrentPoison;
            set => SetProperty(ref _targetCurrentPoison, value);
        }

        private float _targetMaxPoison;

        public float TargetMaxPoison
        {
            get => _targetMaxPoison;
            set => SetProperty(ref _targetMaxPoison, value);
        }

        private bool _showPoison;

        public bool ShowPoison
        {
            get => _showPoison;
            set
            {
                SetProperty(ref _showPoison, value);
                if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                _resistancesWindowWindow.DataContext = null;
                _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _isPoisonToxicImmune;

        public bool IsPoisonToxicImmune
        {
            get => _isPoisonToxicImmune;
            set => SetProperty(ref _isPoisonToxicImmune, value);
        }

        private float _targetCurrentToxic;

        public float TargetCurrentToxic
        {
            get => _targetCurrentToxic;
            set => SetProperty(ref _targetCurrentToxic, value);
        }

        private float _targetMaxToxic;

        public float TargetMaxToxic
        {
            get => _targetMaxToxic;
            set => SetProperty(ref _targetMaxToxic, value);
        }

        private bool _showToxic;

        public bool ShowToxic
        {
            get => _showToxic;
            set
            {
                SetProperty(ref _showToxic, value);
                if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                _resistancesWindowWindow.DataContext = null;
                _resistancesWindowWindow.DataContext = this;
            }
        }

        private bool _showAllResistances;

        public bool ShowAllResistances
        {
            get => _showAllResistances;
            set
            {
                if (SetProperty(ref _showAllResistances, value))
                {
                    UpdateResistancesDisplay();
                }
            }
        }
        
        private float _slashDefense;

        public float SlashDefense
        {
            get => _slashDefense;
            set => SetProperty(ref _slashDefense, value);
        }

        private float _thrustDefense;

        public float ThrustDefense
        {
            get => _thrustDefense;
            set => SetProperty(ref _thrustDefense, value);
        }

        private float _strikeDefense;

        public float StrikeDefense
        {
            get => _strikeDefense;
            set => SetProperty(ref _strikeDefense, value);
        }

        private int _magicResist;

        public int MagicResist
        {
            get => _magicResist;
            set => SetProperty(ref _magicResist, value);
        }

        private int _lightningResist;

        public int LightningResist
        {
            get => _lightningResist;
            set => SetProperty(ref _lightningResist, value);
        }

        private int _fireResist;

        public int FireResist
        {
            get => _fireResist;
            set => SetProperty(ref _fireResist, value);
        }

        private int _darkResist;

        public int DarkResist
        {
            get => _darkResist;
            set => SetProperty(ref _darkResist, value);
        }

        private int _poisonToxicResist;

        public int PoisonToxicResist
        {
            get => _poisonToxicResist;
            set => SetProperty(ref _poisonToxicResist, value);
        }

        private int _bleedResist;

        public int BleedResist
        {
            get => _bleedResist;
            set => SetProperty(ref _bleedResist, value);
        }

        #endregion

        #region Private Methods
        
        private void OnGameLoaded()
        {
            if (IsTargetOptionsEnabled)
            {
                _targetService.ToggleTargetHook(true);
                _targetOptionsTimer.Start();
            }

            _targetService.ClearDisableEntities();
            _targetService.ToggleTargetAi(false);
            AreOptionsEnabled = true;
        }

        private void OnGameNotLoaded()
        {
            _targetOptionsTimer.Stop();
            IsFreezeHealthEnabled = false;
            LastAct = 0;
            AreOptionsEnabled = false;
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.EnableTargetOptions,
                () => { IsTargetOptionsEnabled = !IsTargetOptionsEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.ShowAllResistances, () =>
            {
                ShowAllResistances = !ShowAllResistances;
                UpdateResistancesDisplay();
            });
            _hotkeyManager.RegisterAction(HotkeyActions.FreezeHp, () =>
            {
                if (!IsValidTarget) return;
                IsFreezeHealthEnabled = !IsFreezeHealthEnabled;
            });
            _hotkeyManager.RegisterAction(HotkeyActions.KillTarget, () =>
            {
                if (!IsValidTarget) return;
                SetTargetHealth(0);
            });
            _hotkeyManager.RegisterAction(HotkeyActions.DisableTargetAi,
                () =>
                {
                    if (!IsValidTarget) return;
                    IsDisableTargetAiEnabled = !IsDisableTargetAiEnabled;
                });
            _hotkeyManager.RegisterAction(HotkeyActions.IncreaseTargetSpeed, () =>
            {
                if (!IsValidTarget) return;
                SetSpeed(Math.Min(5, TargetSpeed + 0.25f));
            });
            _hotkeyManager.RegisterAction(HotkeyActions.DecreaseTargetSpeed, () =>
            {
                if (!IsValidTarget) return;
                SetSpeed(Math.Max(0, TargetSpeed - 0.25f));
            });
            _hotkeyManager.RegisterAction(HotkeyActions.TargetRepeatAct, () =>
            {
                if (!IsValidTarget) return;
                IsRepeatActEnabled = !IsRepeatActEnabled;
            });
        }

        private void TargetOptionsTimerTick(object sender, EventArgs e)
        {
            if (!IsTargetValid())
            {
                IsValidTarget = false;
                _targetService.ClearLockedTarget();
                _isDisableTargetAiEnabled = false;
                OnPropertyChanged(nameof(IsDisableTargetAiEnabled));
                TargetCurrentHealth = 0;
                TargetCurrentLightPoise = 0;
                TargetCurrentHeavyPoise = 0;
                TargetCurrentBleed = 0;
                TargetCurrentPoison = 0;
                TargetCurrentToxic = 0;
                return;
            }

            IsValidTarget = true;
            long targetChrCtrl = _targetService.GetTargetChrCtrl();
            if (targetChrCtrl != _currentTargetChrCtrl)
            {
#if DEBUG
                Console.WriteLine($@"Locked on target chrCtrl: 0x{targetChrCtrl:X}");
#endif

                IsDisableTargetAiEnabled = _targetService.IsAiDisabled(targetChrCtrl);

                _currentTargetChrCtrl = targetChrCtrl;

                (IsPoisonToxicImmune, IsBleedImmune) = _targetService.GetImmunities();
                
                
                var targetFullState = _targetService.GetMaxValues();
                TargetMaxHeavyPoise = targetFullState.MaxHeavyPoise;
                TargetMaxLightPoise = targetFullState.MaxLightPoise;
                TargetMaxPoison = IsPoisonToxicImmune ? 0 : targetFullState.PoisonMax;
                TargetMaxToxic = IsPoisonToxicImmune ? 0 : targetFullState.ToxicMax;
                TargetMaxBleed = IsBleedImmune ? 0 : targetFullState.BleedMax;

                IsLightPoiseImmune = _targetService.IsLightPoiseImmune();
                
                
                UpdateDefenses();

                if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                _resistancesWindowWindow.DataContext = null;
                _resistancesWindowWindow.DataContext = this;
            }

            LastAct = _targetService.GetLastAct();

            var targetState = _targetService.GetTargetState();

            TargetCurrentHealth = targetState.CurrentHp;
            TargetMaxHealth = targetState.MaxHp;

            TargetCurrentHeavyPoise = targetState.CurrentHeavyPoise;
            TargetCurrentLightPoise = targetState.CurrentLightPoise;

            TargetCurrentPoison = IsPoisonToxicImmune ? 0 : targetState.CurrentPoison;
            TargetCurrentToxic = IsPoisonToxicImmune ? 0 : targetState.CurrentToxic;
            TargetCurrentBleed = IsBleedImmune ? 0 : targetState.CurrentBleed;
            
            TargetSpeed = targetState.Speed;
        }

        private void UpdateDefenses()
        {
            MagicResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParamOffsets.MagicResist);
            LightningResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParamOffsets.LightningResist);
            FireResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParamOffsets.FireResist);
            DarkResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParamOffsets.DarkResist);
            PoisonToxicResist =
                _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParamOffsets.PoisonToxicResist);
            BleedResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParamOffsets.BleedResist);
            SlashDefense = _targetService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommonOffsets.Slash);
            ThrustDefense = _targetService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommonOffsets.Thrust);
            StrikeDefense = _targetService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommonOffsets.Strike);
        }

        private bool IsTargetValid()
        {
            nint targetId = _targetService.GetTargetChrCtrl();
            if (targetId == 0)
                return false;

            float health = _targetService.GetTargetHp();
            float maxHealth = _targetService.GetTargetMaxHp();
            if (health < 0 || maxHealth <= 0 || health > 10000000 || maxHealth > 10000000)
                return false;

            if (health > maxHealth * 1.5) return false;

            var position = _targetService.GetTargetPos();

            if (float.IsNaN(position.X) || float.IsNaN(position.Y) || float.IsNaN(position.Z))
                return false;

            if (Math.Abs(position.X) > 10000 || Math.Abs(position.Y) > 10000 || Math.Abs(position.Z) > 10000)
                return false;

            return true;
        }

        private void UpdateResistancesDisplay()
        {
            if (!IsTargetOptionsEnabled) return;
            if (_showAllResistances)
            {
                ShowBleed = true;
                ShowHeavyPoise = true;
                ShowLightPoise = true;
                ShowPoison = true;
                ShowToxic = true;
            }
            else
            {
                ShowBleed = false;
                ShowHeavyPoise = false;
                ShowLightPoise = false;
                ShowPoison = false;
                ShowToxic = false;
            }

            if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
            _resistancesWindowWindow.DataContext = null;
            _resistancesWindowWindow.DataContext = this;
        }

        private void OpenResistancesWindow()
        {
            if (_resistancesWindowWindow != null && _resistancesWindowWindow.IsVisible) return;
            _resistancesWindowWindow = new ResistancesWindow
            {
                DataContext = this
            };
            _resistancesWindowWindow.Closed += (s, e) => _isResistancesWindowOpen = false;
            _resistancesWindowWindow.Show();
        }

        private void CloseResistancesWindow()
        {
            if (_resistancesWindowWindow == null || !_resistancesWindowWindow.IsVisible) return;
            _resistancesWindowWindow.Close();
            _resistancesWindowWindow = null;
        }

        #endregion

        public void SetTargetHealth(int value)
        {
            int health = TargetMaxHealth * value / 100;
            _targetService.SetTargetHp(health);
        }

        public void SetSpeed(float value)
        {
            TargetSpeed = value;
        }

        
        

        public void OpenDefenseWindow()
        {
            if (_defenseWindow != null && _defenseWindow.IsVisible)
            {
                _defenseWindow.Activate();
                return;
            }

            _defenseWindow = new DefenseWindow
            {
                DataContext = this
            };

            _defenseWindow.Closed += (s, e) => _defenseWindow = null;

            _defenseWindow.Show();
        }
    }
}