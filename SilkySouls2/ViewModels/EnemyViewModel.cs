using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.ViewModels
{
    public class EnemyViewModel : BaseViewModel
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
        private bool _isFreezeTargetEnabled;
        private bool _isRepeatActEnabled;

        private int _lastAct;
        private int _forceAct;

        // private ResistancesWindow _resistancesWindowWindow;
        private bool _isResistancesWindowOpen;

        private float _targetCurrentPoise;
        private float _targetMaxPoise;
        private float _targetPoiseTimer;
        private bool _showPoise;

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

        private bool _isAllDisableAiEnabled;
        private bool _isAllNoDamageEnabled;
        private bool _isAllNoDeathEnabled;
        private bool _isAllRepeatActEnabled;


        private readonly Dictionary<int, Forlorn> _forlorns;
        private int _currentMapId;
        private bool _isForlornAvailable;
        private int _selectedForlornIndex;
        private bool _isGuaranteedSpawnEnabled;
        private Forlorn _currentForlorn;
        private string _currentAreaName = "No Forlorn in this area";
        
        private Forlorn _selectedForlorn;
        private ObservableCollection<Forlorn> _availableForlorns;


        private readonly EnemyService _enemyService;
        private readonly DamageControlService _damageControlService;
        private readonly HotkeyManager _hotkeyManager;

        public EnemyViewModel(EnemyService enemyService, HotkeyManager hotkeyManager,
            DamageControlService damageControlService)
        {
            _enemyService = enemyService;
            _damageControlService = damageControlService;
            _hotkeyManager = hotkeyManager;
            RegisterHotkeys();
            
            _availableForlorns = new ObservableCollection<Forlorn>(Forlorn.All);
            
            if (_availableForlorns.Count > 0)
            {
                SelectedForlorn = _availableForlorns[0];
            }
            
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
            // _hotkeyManager.RegisterAction("ShowAllResistances", () =>
            // {
            //     _showAllResistances = !_showAllResistances;
            //     UpdateResistancesDisplay();
            // });
            _hotkeyManager.RegisterAction("FreezeHp", () => { IsFreezeHealthEnabled = !IsFreezeHealthEnabled; });
            _hotkeyManager.RegisterAction("KillTarget", () => {
                if (!IsValidTarget) return;
                SetTargetHealth(0);
            });
            // _hotkeyManager.RegisterAction("DisableTargetAi",
            //     () =>
            //     {
            //         if (!IsValidTarget) return;
            //         IsDisableTargetAiEnabled = !IsDisableTargetAiEnabled;
            //     });
            // _hotkeyManager.RegisterAction("IncreaseTargetSpeed", () =>
            // {
            //     if (!IsValidTarget) return;
            //     SetSpeed(Math.Min(5, TargetSpeed + 0.25f));
            // });
            // _hotkeyManager.RegisterAction("DecreaseTargetSpeed", () =>
            // {
            //     if (!IsValidTarget) return;
            //     SetSpeed(Math.Max(0, TargetSpeed - 0.25f));
            // });
            // _hotkeyManager.RegisterAction("TargetRepeatAct", () =>
            // {
            //     if (!IsValidTarget) return;
            //     IsRepeatActEnabled = !IsRepeatActEnabled;
            // });
            // _hotkeyManager.RegisterAction("DisableAi", () => { IsAllDisableAiEnabled = !IsAllDisableAiEnabled; });
            // _hotkeyManager.RegisterAction("AllNoDeath", () => { IsAllNoDeathEnabled = !IsAllNoDeathEnabled; });
            // _hotkeyManager.RegisterAction("AllNoDamage", () => { IsAllNoDamageEnabled = !IsAllNoDamageEnabled; });
            // _hotkeyManager.RegisterAction("AllRepeatAct", () => { IsAllRepeatActEnabled = !IsAllRepeatActEnabled; });
        }

        private void TargetOptionsTimerTick(object sender, EventArgs e)
        {
            if (!IsTargetValid())
            {
                IsValidTarget = false;
                return;
            }
        
            IsValidTarget = true;
            TargetCurrentHealth = _enemyService.GetTargetHp();
            TargetMaxHealth = _enemyService.GetTargetMaxHp();
            LastAct = _enemyService.GetLastAct();
            long targetId = _enemyService.GetTargetId();
            if (targetId != _currentTargetId)
            {
            //     IsDisableTargetAiEnabled = _enemyService.IsTargetAiDisabled();
            //     int forceActValue = _enemyService.GetForceAct();
            //     if (forceActValue != 0)
            //     {
            //         IsRepeatActEnabled = true;
            //         ForceAct = forceActValue;
            //     }
            //     else
            //     {
            //         ForceAct = 0;
            //         IsRepeatActEnabled = false;
            //     }
                
                //
                // IsFreezeHealthEnabled = _enemyService.IsTargetNoDamageEnabled();
                _currentTargetId = targetId;
                TargetMaxPoise = _enemyService.GetTargetResistance(GameManagerImp.PlayerCtrlOffsets.PoiseMax);
                (IsPoisonToxicImmune, IsBleedImmune) = _enemyService.GetImmunities();
                TargetMaxPoison = IsPoisonToxicImmune
                    ? 0
                    : _enemyService.GetTargetResistance(GameManagerImp.PlayerCtrlOffsets.PoisonMax);
                TargetMaxToxic = IsPoisonToxicImmune
                    ? 0
                    : _enemyService.GetTargetResistance(GameManagerImp.PlayerCtrlOffsets.ToxicMax);
                TargetMaxBleed = IsBleedImmune
                    ? 0
                    : _enemyService.GetTargetResistance(GameManagerImp.PlayerCtrlOffsets.BleedMax);
            
                //     : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.FrostMax);
                // AreCinderOptionsEnabled = _cinderService.IsTargetCinder();
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }
        
            // TargetSpeed = _enemyService.GetTargetSpeed();
            TargetCurrentPoise = _enemyService.GetTargetResistance(GameManagerImp.PlayerCtrlOffsets.PoiseCurrent);
            TargetCurrentPoison = IsPoisonToxicImmune
                ? 0
                : _enemyService.GetTargetResistance(GameManagerImp.PlayerCtrlOffsets.PoisonCurrent);
            TargetCurrentToxic = IsPoisonToxicImmune
                ? 0
                : _enemyService.GetTargetResistance(GameManagerImp.PlayerCtrlOffsets.ToxicCurrent);
            TargetCurrentBleed = IsBleedImmune
                ? 0
                : _enemyService.GetTargetResistance(GameManagerImp.PlayerCtrlOffsets.BleedCurrent);
            
        }
        //
        //
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
            long targetId = _enemyService.GetTargetId();
            if (targetId == 0)
                return false;
        
            float health = _enemyService.GetTargetHp();
            float maxHealth = _enemyService.GetTargetMaxHp();
            if (health < 0 || maxHealth <= 0 || health > 10000000 || maxHealth > 10000000)
                return false;
        
            if (health > maxHealth * 1.5) return false;
        
            var position = _enemyService.GetTargetPos();
        
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
                    _enemyService.ToggleTargetHook(true);
                    _targetOptionsTimer.Start();
                    _enemyService.ToggleCurrentActHook(true);
                    // ShowAllResistances = true;
                }
                else
                {
                    _targetOptionsTimer.Stop();
                    _enemyService.ToggleCurrentActHook(false);
                    // IsRepeatActEnabled = false;
                    ShowAllResistances = false;
                    // IsResistancesWindowOpen = false;
                    IsFreezeHealthEnabled = false;
                    _enemyService.ToggleTargetHook(false);
                    ShowPoise = false;
                    ShowBleed = false;
                    ShowPoison = false;
                    ShowToxic = false;
                }
            }
        }
        
        private void UpdateResistancesDisplay()
        {
            if (!IsTargetOptionsEnabled) return;
            if (_showAllResistances)
            {
                ShowBleed = true;
                ShowPoise = true;
                ShowPoison = true;
                ShowToxic = true;
            }
            else
            {
                ShowBleed = false;
                ShowPoise = false;
                ShowPoison = false;
                ShowToxic = false;
            }
        
            // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
            // _resistancesWindowWindow.DataContext = null;
            // _resistancesWindowWindow.DataContext = this;
        }
        
        // public bool IsResistancesWindowOpen
        // {
        //     get => _isResistancesWindowOpen;
        //     set
        //     {
        //         if (!SetProperty(ref _isResistancesWindowOpen, value)) return;
        //         if (value)
        //             OpenResistancesWindow();
        //         else
        //             CloseResistancesWindow();
        //     }
        // }
        //
        // private void OpenResistancesWindow()
        // {
        //     if (_resistancesWindowWindow != null && _resistancesWindowWindow.IsVisible) return;
        //     _resistancesWindowWindow = new ResistancesWindow
        //     {
        //         DataContext = this
        //     };
        //     _resistancesWindowWindow.Closed += (s, e) => _isResistancesWindowOpen = false;
        //     _resistancesWindowWindow.Show();
        // }
        //
        // private void CloseResistancesWindow()
        // {
        //     if (_resistancesWindowWindow == null || !_resistancesWindowWindow.IsVisible) return;
        //     _resistancesWindowWindow.Close();
        //     _resistancesWindowWindow = null;
        // }
    
        public bool IsRepeatActEnabled
        {
            get => _isRepeatActEnabled;
            set
            {
                if (!SetProperty(ref _isRepeatActEnabled, value)) return;
                _enemyService.ToggleRepeatAct(_isRepeatActEnabled);
            }
        }
        
        public int LastAct
        {
            get => _lastAct;
            set => SetProperty(ref _lastAct, value);
        }
        
        // public int ForceAct
        // {
        //     get => _forceAct;
        //     set
        //     {
        //         if (!SetProperty(ref _forceAct, value)) return;
        //         _enemyService.ForceAct(_forceAct);
        //         if (_forceAct == 0) IsRepeatActEnabled = false;
        //     }
        // }
        //
        //
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
            _enemyService.SetTargetHp(health);
        }
        //
        public bool IsFreezeHealthEnabled
        {
            get => _isFreezeHealthEnabled;
            set
            {
                SetProperty(ref _isFreezeHealthEnabled, value);
                _damageControlService.ToggleFreezeTargetHp(_isFreezeHealthEnabled);
            }
        }
        //
        // public bool ShowBleedAndNotImmune => ShowBleed && !IsBleedImmune;
        // public bool ShowPoisonAndNotImmune => ShowPoison && !IsPoisonImmune;
        // public bool ShowToxicAndNotImmune => ShowToxic && !IsToxicImmune;
        // public bool ShowFrostAndNotImmune => ShowFrost && !IsFrostImmune;
        //
        
        public float TargetCurrentPoise
        {
            get => _targetCurrentPoise;
            set => SetProperty(ref _targetCurrentPoise, value);
        }
        
        public float TargetMaxPoise
        {
            get => _targetMaxPoise;
            set => SetProperty(ref _targetMaxPoise, value);
        }
        

        
        public bool ShowPoise
        {
            get => _showPoise;
            set
            {
                SetProperty(ref _showPoise, value);
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
            }
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
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
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
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
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
                // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
                // _resistancesWindowWindow.DataContext = null;
                // _resistancesWindowWindow.DataContext = this;
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
        //
        //
        // public float TargetSpeed
        // {
        //     get => _targetSpeed;
        //     set
        //     {
        //         if (SetProperty(ref _targetSpeed, value))
        //         {
        //             _enemyService.SetTargetSpeed(value);
        //         }
        //     }
        // }
        //
        // public void SetSpeed(float value)
        // {
        //     TargetSpeed = value;
        // }
        

        public bool IsAllDisableAiEnabled
        {
            get => _isAllDisableAiEnabled;
            set
            {
                if (SetProperty(ref _isAllDisableAiEnabled, value))
                {
                    _enemyService.ToggleDisableAi(_isAllDisableAiEnabled);
                }
            }
        }
       
        // public bool IsAllRepeatActEnabled
        // {
        //     get => _isAllRepeatActEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isAllRepeatActEnabled, value))
        //         {
        //             _enemyService.ToggleAllRepeatAct(_isAllRepeatActEnabled);
        //         }
        //     }
        // }
  

        public ObservableCollection<Forlorn> AvailableForlorns 
        {
            get => _availableForlorns;
            private set => SetProperty(ref _availableForlorns, value);
        }

        public Forlorn SelectedForlorn
        {
            get => _selectedForlorn;
            set
            {
                if (!SetProperty(ref _selectedForlorn, value)) return;
                CurrentAreaName = value?.AreaName ?? "No Forlorn selected";
                IsForlornAvailable = value != null;
                _selectedForlornIndex = 0;
                OnPropertyChanged(nameof(ForlornIndexes));
            }
        }
        
        public bool IsForlornAvailable
        {
            get => _isForlornAvailable;
            private set => SetProperty(ref _isForlornAvailable, value);
        }
        
        public string CurrentAreaName
        {
            get => _currentAreaName;
            private set => SetProperty(ref _currentAreaName, value);
        }


        public int SelectedForlornIndex
        {
            get => _selectedForlornIndex; 
            set
            {
                if (!SetProperty(ref _selectedForlornIndex, value)) return;
                if (SelectedForlorn == null || !IsGuaranteedSpawnEnabled) return;
                _enemyService.UpdateForlornIndex(_selectedForlornIndex + 1);
                _enemyService.ToggleForlornSpawn(true, 
                    SelectedForlorn.EsdFuncId,
                    _selectedForlornIndex + 1);
            }
        }


        public bool IsGuaranteedSpawnEnabled
        {
            get => _isGuaranteedSpawnEnabled;
            set
            {
                if (!SetProperty(ref _isGuaranteedSpawnEnabled, value)) return;
                if (SelectedForlorn != null)
                {
                    _enemyService.ToggleForlornSpawn(_isGuaranteedSpawnEnabled, 
                        SelectedForlorn.EsdFuncId,
                        SelectedForlornIndex + 1);
                }
            }
        }

        public IEnumerable<string> ForlornIndexes 
        {
            get
            {
                if (SelectedForlorn?.SpawnNames == null)
                    return Enumerable.Empty<string>();
                return SelectedForlorn.SpawnNames
                    .Select((name, i) => $"{i + 1}: {name}")
                    .ToArray();
            }
        }

        public void TryEnableFeatures()
        {
            if (IsTargetOptionsEnabled)
            {
                _enemyService.ToggleTargetHook(true);
                _targetOptionsTimer.Start();
            }
            // if (IsAllRepeatActEnabled) _enemyService.ToggleAllRepeatAct(true);
            AreOptionsEnabled = true;
        }
        
        public void DisableFeatures()
        {
            _targetOptionsTimer.Stop();
            IsFreezeHealthEnabled = false;
            LastAct = 0;
            // ForceAct = 0;
            AreOptionsEnabled = false;
        }
        
        public void TrryApplyOneTimeFeatures()
        {
            if (IsAllDisableAiEnabled) _enemyService.ToggleDisableAi(true);
        }
    }
}