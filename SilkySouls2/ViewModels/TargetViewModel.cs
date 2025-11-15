using System;
using System.Collections.Generic;
using System.Windows.Threading;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.Views;
using SilkySouls2.Views.Windows;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.ViewModels
{
    public class TargetViewModel : BaseViewModel
    {
        private bool _areOptionsEnabled = true;
        private bool _isTargetOptionsEnabled;
        private bool _isValidTarget;
        private readonly DispatcherTimer _targetOptionsTimer;

        private int _targetCurrentHealth;
        private int _targetMaxHealth;
        private long _currentTargetId;
        private float _targetSpeed;
        private bool _isFreezeHealthEnabled;
        private bool _isDisableTargetAiEnabled;
        private bool _isRepeatActEnabled;
        
        private int _lastAct;

        private ResistancesWindow _resistancesWindowWindow;
        private bool _isResistancesWindowOpen;
        
        private DefenseWindow _defenseWindow;

        private float _targetCurrentHeavyPoise;
        private float _targetMaxHeavyPoise;
        private bool _showHeavyPoise;
        private float _targetCurrentLightPoise;
        private float _targetMaxLightPoise;
        private bool _showLightPoise;
        private bool _isLightPoiseImmune;

        private float _targetCurrentBleed;
        private float _targetMaxBleed;
        private bool _showBleed;
        private bool _isBleedImmune;

        private float _targetCurrentPoison;
        private float _targetMaxPoison;
        private bool _showPoison;
        private bool _isPoisonToxicImmune;

        private float _targetCurrentToxic;
        private float _targetMaxToxic;
        private bool _showToxic;
        
        private bool _showAllResistances;
        
        private readonly TargetService _targetService;
        private readonly DamageControlService _damageControlService;
        private readonly HotkeyManager _hotkeyManager;

        public TargetViewModel(TargetService targetService, HotkeyManager hotkeyManager,
            DamageControlService damageControlService)
        {
            _targetService = targetService;
            _damageControlService = damageControlService;
            _hotkeyManager = hotkeyManager;
            RegisterHotkeys();
            
            
            
            _targetOptionsTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(64)
            };
            _targetOptionsTimer.Tick += TargetOptionsTimerTick;
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction("EnableTargetOptions",
                () => { IsTargetOptionsEnabled = !IsTargetOptionsEnabled; });
            _hotkeyManager.RegisterAction("ShowAllResistances", () =>
            {
                ShowAllResistances = !ShowAllResistances;
                UpdateResistancesDisplay();
            });
            _hotkeyManager.RegisterAction("FreezeHp", () =>
            {
                if (!IsValidTarget) return;
                IsFreezeHealthEnabled = !IsFreezeHealthEnabled;
            });
            _hotkeyManager.RegisterAction("KillTarget", () => {
                if (!IsValidTarget) return;
                SetTargetHealth(0);
            });
            _hotkeyManager.RegisterAction("DisableTargetAi",
                () =>
                {
                    if (!IsValidTarget) return;
                    IsDisableTargetAiEnabled = !IsDisableTargetAiEnabled;
                });
            _hotkeyManager.RegisterAction("IncreaseTargetSpeed", () =>
            {
                if (!IsValidTarget) return;
                SetSpeed(Math.Min(5, TargetSpeed + 0.25f));
            });
            _hotkeyManager.RegisterAction("DecreaseTargetSpeed", () =>
            {
                if (!IsValidTarget) return;
                SetSpeed(Math.Max(0, TargetSpeed - 0.25f));
            });
            _hotkeyManager.RegisterAction("TargetRepeatAct", () =>
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
            long targetId = _targetService.GetTargetId();
            if (targetId != _currentTargetId)
            {
                IsDisableTargetAiEnabled = _targetService.IsAiDisabled(targetId);
            
                _currentTargetId = targetId;
                TargetMaxHeavyPoise = _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.HeavyPoiseMax);
                TargetMaxLightPoise = _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.LightPoiseMax);
                (IsPoisonToxicImmune, IsBleedImmune) = _targetService.GetImmunities();
                TargetMaxPoison = IsPoisonToxicImmune
                    ? 0
                    : _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.PoisonMax);
                TargetMaxToxic = IsPoisonToxicImmune
                    ? 0
                    : _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.ToxicMax);
                TargetMaxBleed = IsBleedImmune
                    ? 0
                    : _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.BleedMax);

                IsLightPoiseImmune = _targetService.IsLightPoiseImmune();
                UpdateDefenses();
                
                if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                _resistancesWindowWindow.DataContext = null;
                _resistancesWindowWindow.DataContext = this;
              
            }
            TargetCurrentHealth = _targetService.GetTargetHp();
            TargetMaxHealth = _targetService.GetTargetMaxHp();
            LastAct = _targetService.GetLastAct();
            
            TargetSpeed = _targetService.GetTargetSpeed();
            TargetCurrentHeavyPoise = _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.HeavyPoiseCurrent);
            TargetCurrentLightPoise = _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.LightPoiseCurrent);
            TargetCurrentPoison = IsPoisonToxicImmune
                ? 0
                : _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.PoisonCurrent);
            TargetCurrentToxic = IsPoisonToxicImmune
                ? 0
                : _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.ToxicCurrent);
            TargetCurrentBleed = IsBleedImmune
                ? 0
                : _targetService.GetTargetResistance(GameManagerImp.ChrCtrlOffsets.BleedCurrent);
            
        }

        private void UpdateDefenses()
        {
            MagicResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.MagicResist);
            LightningResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.LightningResist);
            FireResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.FireResist);
            DarkResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.DarkResist);
            PoisonToxicResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.PoisonToxicResist);
            BleedResist = _targetService.GetChrParam(GameManagerImp.ChrCtrlOffsets.ChrParam.BleedResist);
            SlashDefense = _targetService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommon.Slash);
            ThrustDefense = _targetService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommon.Thrust);
            StrikeDefense = _targetService.GetChrCommonParam(GameManagerImp.ChrCtrlOffsets.ChrCommon.Strike);
        }

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }
        
        public bool IsValidTarget
        {
            get => _isValidTarget;
            set => SetProperty(ref _isValidTarget, value);
        }
        
        private bool IsTargetValid()
        {
            long targetId = _targetService.GetTargetId();
            if (targetId == 0)
                return false;
        
            float health = _targetService.GetTargetHp();
            float maxHealth = _targetService.GetTargetMaxHp();
            if (health < 0 || maxHealth <= 0 || health > 10000000 || maxHealth > 10000000)
                return false;
        
            if (health > maxHealth * 1.5) return false;
        
            var position = _targetService.GetTargetPos();
        
            if (float.IsNaN(position[0]) || float.IsNaN(position[1]) || float.IsNaN(position[2]))
                return false;
        
            if (Math.Abs(position[0]) > 10000 || Math.Abs(position[1]) > 10000 || Math.Abs(position[2]) > 10000)
                return false;
        
            return true;
        }
        
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
    
        public bool IsRepeatActEnabled
        {
            get => _isRepeatActEnabled;
            set
            {
                if (!SetProperty(ref _isRepeatActEnabled, value)) return;
                _targetService.ToggleRepeatAct(_isRepeatActEnabled);
            }
        }
        
        public int LastAct
        {
            get => _lastAct;
            set => SetProperty(ref _lastAct, value);
        }
        
        public int TargetCurrentHealth
        {
            get => _targetCurrentHealth;
            set => SetProperty(ref _targetCurrentHealth, value);
        }
        
        public int TargetMaxHealth
        {
            get => _targetMaxHealth;
            set => SetProperty(ref _targetMaxHealth, value);
        }
        
        public void SetTargetHealth(int value)
        {
            int health = TargetMaxHealth * value / 100;
            _targetService.SetTargetHp(health);
        }
        
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
        
        public void SetSpeed(float value)
        {
            TargetSpeed = value;
        }
        
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
  
        
        public float TargetCurrentHeavyPoise
        {
            get => _targetCurrentHeavyPoise;
            set => SetProperty(ref _targetCurrentHeavyPoise, value);
        }
        
        public float TargetMaxHeavyPoise
        {
            get => _targetMaxHeavyPoise;
            set => SetProperty(ref _targetMaxHeavyPoise, value);
        }
        

        
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
        
        public float TargetCurrentLightPoise
        {
            get => _targetCurrentLightPoise;
            set => SetProperty(ref _targetCurrentLightPoise, value);
        }
        
        public float TargetMaxLightPoise
        {
            get => _targetMaxLightPoise;
            set => SetProperty(ref _targetMaxLightPoise, value);
        }
        
        public bool IsLightPoiseImmune
        {
            get => _isLightPoiseImmune;
            set => SetProperty(ref _isLightPoiseImmune, value);
        }
        
        public float TargetCurrentBleed
        {
            get => _targetCurrentBleed;
            set => SetProperty(ref _targetCurrentBleed, value);
        }
        
        public float TargetMaxBleed
        {
            get => _targetMaxBleed;
            set => SetProperty(ref _targetMaxBleed, value);
        }
        
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

        public bool IsBleedImmune
        {
            get => _isBleedImmune;
            set => SetProperty(ref _isBleedImmune, value);
        }
        
        public float TargetCurrentPoison
        {
            get => _targetCurrentPoison;
            set => SetProperty(ref _targetCurrentPoison, value);
        }
        
        public float TargetMaxPoison
        {
            get => _targetMaxPoison;
            set => SetProperty(ref _targetMaxPoison, value);
        }
        
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
        
        public bool IsPoisonToxicImmune
        {
            get => _isPoisonToxicImmune;
            set => SetProperty(ref _isPoisonToxicImmune, value);
        }
        
        public float TargetCurrentToxic
        {
            get => _targetCurrentToxic;
            set => SetProperty(ref _targetCurrentToxic, value);
        }
        
        public float TargetMaxToxic
        {
            get => _targetMaxToxic;
            set => SetProperty(ref _targetMaxToxic, value);
        }
        
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

        public void TryEnableFeatures()
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
        
        public void DisableFeatures()
        {
            _targetOptionsTimer.Stop();
            IsFreezeHealthEnabled = false;
            LastAct = 0;
            AreOptionsEnabled = false;
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