﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.ViewModels;


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
        private ulong _currentTargetId;
        private float _targetSpeed;
        private bool _isFreezeHealthEnabled;
        private bool _isDisableTargetAiEnabled;
        private bool _isRepeatActEnabled;

        private int _lastAct;
        private int _forceAct;

        // private ResistancesWindow _resistancesWindowWindow;
        private bool _isResistancesWindowOpen;

        private float _targetCurrentPoise;
        private float _targetMaxPoise;
        private float _targetPoiseTimer;
        private bool _showPoise;

        private int _targetCurrentBleed;
        private int _targetMaxBleed;
        private bool _showBleed;
        private bool _isBleedImmune;

        private int _targetCurrentPoison;
        private int _targetMaxPoison;
        private bool _showPoison;
        private bool _isPoisonImmune;

        private int _targetCurrentToxic;
        private int _targetMaxToxic;
        private bool _showToxic;
        private bool _isToxicImmune;

        private int _targetCurrentFrost;
        private int _targetMaxFrost;
        private bool _showFrost;
        private bool _isFrostImmune;

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
        private readonly HotkeyManager _hotkeyManager;

        public EnemyViewModel(EnemyService enemyService, HotkeyManager hotkeyManager)
        {
            _enemyService = enemyService;
            _hotkeyManager = hotkeyManager;
            // RegisterHotkeys();



            _availableForlorns = new ObservableCollection<Forlorn>(Forlorn.All);
            
            if (_availableForlorns.Count > 0)
            {
                SelectedForlorn = _availableForlorns[0];
            }
            
            
            
            _targetOptionsTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(64)
            };
            // _targetOptionsTimer.Tick += TargetOptionsTimerTick;
        }

        // private void RegisterHotkeys()
        // {
        //     _hotkeyManager.RegisterAction("EnableTargetOptions",
        //         () => { IsTargetOptionsEnabled = !IsTargetOptionsEnabled; });
        //     _hotkeyManager.RegisterAction("ShowAllResistances", () =>
        //     {
        //         _showAllResistances = !_showAllResistances;
        //         UpdateResistancesDisplay();
        //     });
        //     _hotkeyManager.RegisterAction("FreezeHp", () => { IsFreezeHealthEnabled = !IsFreezeHealthEnabled; });
        //     _hotkeyManager.RegisterAction("DisableTargetAi",
        //         () =>
        //         {
        //             if (!IsValidTarget) return;
        //             IsDisableTargetAiEnabled = !IsDisableTargetAiEnabled;
        //         });
        //     _hotkeyManager.RegisterAction("IncreaseTargetSpeed", () =>
        //     {
        //         if (!IsValidTarget) return;
        //         SetSpeed(Math.Min(5, TargetSpeed + 0.25f));
        //     });
        //     _hotkeyManager.RegisterAction("DecreaseTargetSpeed", () =>
        //     {
        //         if (!IsValidTarget) return;
        //         SetSpeed(Math.Max(0, TargetSpeed - 0.25f));
        //     });
        //     _hotkeyManager.RegisterAction("TargetRepeatAct", () =>
        //     {
        //         if (!IsValidTarget) return;
        //         IsRepeatActEnabled = !IsRepeatActEnabled;
        //     });
        //     _hotkeyManager.RegisterAction("DisableAi", () => { IsAllDisableAiEnabled = !IsAllDisableAiEnabled; });
        //     _hotkeyManager.RegisterAction("AllNoDeath", () => { IsAllNoDeathEnabled = !IsAllNoDeathEnabled; });
        //     _hotkeyManager.RegisterAction("AllNoDamage", () => { IsAllNoDamageEnabled = !IsAllNoDamageEnabled; });
        //     _hotkeyManager.RegisterAction("AllRepeatAct", () => { IsAllRepeatActEnabled = !IsAllRepeatActEnabled; });
        //     _hotkeyManager.RegisterAction("SetSwordPhase", () => SetCinderPhase(0));
        //     _hotkeyManager.RegisterAction("SetLancePhase", () => SetCinderPhase(1));
        //     _hotkeyManager.RegisterAction("SetCurvedPhase", () => SetCinderPhase(2));
        //     _hotkeyManager.RegisterAction("SetStaffPhase", () => SetCinderPhase(3));
        //     _hotkeyManager.RegisterAction("SetGwynPhase", () => SetCinderPhase(4));
        //     _hotkeyManager.RegisterAction("PhaseLock", () => IsCinderPhasedLocked = !IsCinderPhasedLocked);
        //     _hotkeyManager.RegisterAction("CastSoulmass", CastSoulmass);
        //     _hotkeyManager.RegisterAction("EndlessSoulmass",
        //         () => IsEndlessSoulmassEnabled = !IsEndlessSoulmassEnabled);
        // }

        // private void TargetOptionsTimerTick(object sender, EventArgs e)
        // {
        //     if (!IsTargetValid())
        //     {
        //         IsValidTarget = false;
        //         return;
        //     }
        //
        //     IsValidTarget = true;
        //     TargetCurrentHealth = _enemyService.GetTargetHp();
        //     TargetMaxHealth = _enemyService.GetTargetMaxHp();
        //
        //     ulong targetId = _enemyService.GetTargetId();
        //
        //     if (targetId != _currentTargetId)
        //     {
        //         IsDisableTargetAiEnabled = _enemyService.IsTargetAiDisabled();
        //         int forceActValue = _enemyService.GetForceAct();
        //         if (forceActValue != 0)
        //         {
        //             IsRepeatActEnabled = true;
        //             ForceAct = forceActValue;
        //         }
        //         else
        //         {
        //             ForceAct = 0;
        //             IsRepeatActEnabled = false;
        //         }
        //         
        //         
        //         IsFreezeHealthEnabled = _enemyService.IsTargetNoDamageEnabled();
        //         _currentTargetId = targetId;
        //         TargetMaxPoise = _enemyService.GetTargetPoise(Offsets.WorldChrMan.ChrSuperArmorModule.MaxPoise);
        //         (IsPoisonImmune, IsToxicImmune, IsBleedImmune, IsFrostImmune) = _enemyService.GetImmunities();
        //         TargetMaxPoison = IsPoisonImmune
        //             ? 0
        //             : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.PoisonMax);
        //         TargetMaxToxic = IsToxicImmune
        //             ? 0
        //             : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.ToxicMax);
        //         TargetMaxBleed = IsBleedImmune
        //             ? 0
        //             : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.BleedMax);
        //         TargetMaxFrost = IsFrostImmune
        //             ? 0
        //             : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.FrostMax);
        //         AreCinderOptionsEnabled = _cinderService.IsTargetCinder();
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        //
        //     TargetSpeed = _enemyService.GetTargetSpeed();
        //     TargetCurrentPoise = _enemyService.GetTargetPoise(Offsets.WorldChrMan.ChrSuperArmorModule.Poise);
        //     TargetPoiseTimer = _enemyService.GetTargetPoise(Offsets.WorldChrMan.ChrSuperArmorModule.PoiseTimer);
        //     TargetCurrentPoison = IsPoisonImmune
        //         ? 0
        //         : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.PoisonCurrent);
        //     TargetCurrentToxic = IsToxicImmune
        //         ? 0
        //         : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.ToxicCurrent);
        //     TargetCurrentBleed = IsBleedImmune
        //         ? 0
        //         : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.BleedCurrent);
        //     TargetCurrentFrost = IsFrostImmune
        //         ? 0
        //         : _enemyService.GetTargetResistance(Offsets.WorldChrMan.ChrResistModule.FrostCurrent);
        //     LastAct = _enemyService.GetLastAct();
        // }
        //
        //
        // public bool AreOptionsEnabled
        // {
        //     get => _areOptionsEnabled;
        //     set => SetProperty(ref _areOptionsEnabled, value);
        // }
        //
        // public bool IsValidTarget
        // {
        //     get => _isValidTarget;
        //     set => SetProperty(ref _isValidTarget, value);
        // }
        //
        // private bool IsTargetValid()
        // {
        //     ulong targetId = _enemyService.GetTargetId();
        //     if (targetId == 0)
        //         return false;
        //
        //     float health = _enemyService.GetTargetHp();
        //     float maxHealth = _enemyService.GetTargetMaxHp();
        //     if (health < 0 || maxHealth <= 0 || health > 10000000 || maxHealth > 10000000)
        //         return false;
        //
        //     if (health > maxHealth * 1.5) return false;
        //
        //     var position = _enemyService.GetTargetPos();
        //
        //     if (float.IsNaN(position[0]) || float.IsNaN(position[1]) || float.IsNaN(position[2]))
        //         return false;
        //
        //     if (Math.Abs(position[0]) > 10000 || Math.Abs(position[1]) > 10000 || Math.Abs(position[2]) > 10000)
        //         return false;
        //
        //     return true;
        // }
        //
        // public bool IsTargetOptionsEnabled
        // {
        //     get => _isTargetOptionsEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isTargetOptionsEnabled, value)) return;
        //         if (value)
        //         {
        //             _enemyService.InstallTargetHook();
        //             _targetOptionsTimer.Start();
        //             ShowAllResistances = true;
        //         }
        //         else
        //         {
        //             _targetOptionsTimer.Stop();
        //             IsRepeatActEnabled = false;
        //             IsCinderPhasedLocked = false;
        //             ShowAllResistances = false;
        //             IsResistancesWindowOpen = false;
        //             IsFreezeHealthEnabled = false;
        //             _enemyService.UninstallTargetHook();
        //             ShowPoise = false;
        //             ShowBleed = false;
        //             ShowPoison = false;
        //             ShowFrost = false;
        //             ShowToxic = false;
        //         }
        //     }
        // }
        //
        // private void UpdateResistancesDisplay()
        // {
        //     if (!IsTargetOptionsEnabled) return;
        //     if (_showAllResistances)
        //     {
        //         ShowBleed = true;
        //         ShowPoise = true;
        //         ShowPoison = true;
        //         ShowFrost = true;
        //         ShowToxic = true;
        //     }
        //     else
        //     {
        //         ShowBleed = false;
        //         ShowPoise = false;
        //         ShowPoison = false;
        //         ShowFrost = false;
        //         ShowToxic = false;
        //     }
        //
        //     if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //     _resistancesWindowWindow.DataContext = null;
        //     _resistancesWindowWindow.DataContext = this;
        // }
        //
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
        //
        // public bool AreCinderOptionsEnabled
        // {
        //     get => _areCinderOptionsEnabled;
        //     set => SetProperty(ref _areCinderOptionsEnabled, value);
        // }
        //
        // public bool IsRepeatActEnabled
        // {
        //     get => _isRepeatActEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isRepeatActEnabled, value)) return;
        //
        //         bool isRepeating = _enemyService.IsTargetRepeating();
        //
        //         switch (value)
        //         {
        //             case true when !isRepeating:
        //                 _enemyService.ToggleTargetRepeatAct(true);
        //                 ForceAct = _enemyService.GetLastAct();
        //                 break;
        //             case false when isRepeating:
        //                 _enemyService.ToggleTargetRepeatAct(false);
        //                 ForceAct = 0;
        //                 break;
        //         }
        //     }
        // }
        //
        // public int LastAct
        // {
        //     get => _lastAct;
        //     set => SetProperty(ref _lastAct, value);
        // }
        //
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
        // public int TargetCurrentHealth
        // {
        //     get => _targetCurrentHealth;
        //     set => SetProperty(ref _targetCurrentHealth, value);
        // }
        //
        // public int TargetMaxHealth
        // {
        //     get => _targetMaxHealth;
        //     set => SetProperty(ref _targetMaxHealth, value);
        // }
        //
        // public void SetTargetHealth(int value)
        // {
        //     int health = TargetMaxHealth * value / 100;
        //     _enemyService.SetTargetHp(health);
        // }
        //
        // public bool IsFreezeHealthEnabled
        // {
        //     get => _isFreezeHealthEnabled;
        //     set
        //     {
        //         SetProperty(ref _isFreezeHealthEnabled, value);
        //         _enemyService.ToggleTargetNoDamage(_isFreezeHealthEnabled);
        //     }
        // }
        //
        // public bool ShowBleedAndNotImmune => ShowBleed && !IsBleedImmune;
        // public bool ShowPoisonAndNotImmune => ShowPoison && !IsPoisonImmune;
        // public bool ShowToxicAndNotImmune => ShowToxic && !IsToxicImmune;
        // public bool ShowFrostAndNotImmune => ShowFrost && !IsFrostImmune;
        //
        //
        // public float TargetCurrentPoise
        // {
        //     get => _targetCurrentPoise;
        //     set => SetProperty(ref _targetCurrentPoise, value);
        // }
        //
        // public float TargetMaxPoise
        // {
        //     get => _targetMaxPoise;
        //     set => SetProperty(ref _targetMaxPoise, value);
        // }
        //
        // public float TargetPoiseTimer
        // {
        //     get => _targetPoiseTimer;
        //     set => SetProperty(ref _targetPoiseTimer, value);
        // }
        //
        // public bool ShowPoise
        // {
        //     get => _showPoise;
        //     set
        //     {
        //         SetProperty(ref _showPoise, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public int TargetCurrentBleed
        // {
        //     get => _targetCurrentBleed;
        //     set => SetProperty(ref _targetCurrentBleed, value);
        // }
        //
        // public int TargetMaxBleed
        // {
        //     get => _targetMaxBleed;
        //     set => SetProperty(ref _targetMaxBleed, value);
        // }
        //
        // public bool ShowBleed
        // {
        //     get => _showBleed;
        //     set
        //     {
        //         SetProperty(ref _showBleed, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public bool IsBleedImmune
        // {
        //     get => _isBleedImmune;
        //     set => SetProperty(ref _isBleedImmune, value);
        // }
        //
        // public int TargetCurrentPoison
        // {
        //     get => _targetCurrentPoison;
        //     set => SetProperty(ref _targetCurrentPoison, value);
        // }
        //
        // public int TargetMaxPoison
        // {
        //     get => _targetMaxPoison;
        //     set => SetProperty(ref _targetMaxPoison, value);
        // }
        //
        // public bool ShowPoison
        // {
        //     get => _showPoison;
        //     set
        //     {
        //         SetProperty(ref _showPoison, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public bool IsPoisonImmune
        // {
        //     get => _isPoisonImmune;
        //     set => SetProperty(ref _isPoisonImmune, value);
        // }
        //
        // public int TargetCurrentToxic
        // {
        //     get => _targetCurrentToxic;
        //     set => SetProperty(ref _targetCurrentToxic, value);
        // }
        //
        // public int TargetMaxToxic
        // {
        //     get => _targetMaxToxic;
        //     set => SetProperty(ref _targetMaxToxic, value);
        // }
        //
        // public bool ShowToxic
        // {
        //     get => _showToxic;
        //     set
        //     {
        //         SetProperty(ref _showToxic, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public bool IsToxicImmune
        // {
        //     get => _isToxicImmune;
        //     set => SetProperty(ref _isToxicImmune, value);
        // }
        //
        // public int TargetCurrentFrost
        // {
        //     get => _targetCurrentFrost;
        //     set => SetProperty(ref _targetCurrentFrost, value);
        // }
        //
        // public int TargetMaxFrost
        // {
        //     get => _targetMaxFrost;
        //     set => SetProperty(ref _targetMaxFrost, value);
        // }
        //
        // public bool ShowFrost
        // {
        //     get => _showFrost;
        //     set
        //     {
        //         SetProperty(ref _showFrost, value);
        //         if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
        //         _resistancesWindowWindow.DataContext = null;
        //         _resistancesWindowWindow.DataContext = this;
        //     }
        // }
        //
        // public bool IsFrostImmune
        // {
        //     get => _isFrostImmune;
        //     set => SetProperty(ref _isFrostImmune, value);
        // }
        //
        // public bool ShowAllResistances
        // {
        //     get => _showAllResistances;
        //     set
        //     {
        //         if (SetProperty(ref _showAllResistances, value))
        //         {
        //             UpdateResistancesDisplay();
        //         }
        //     }
        // }
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
        //
        //
        // public bool IsDisableTargetAiEnabled
        // {
        //     get => _isDisableTargetAiEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isDisableTargetAiEnabled, value))
        //         {
        //             _enemyService.ToggleTargetAi(_isDisableTargetAiEnabled);
        //         }
        //     }
        // }
        //
        // public bool IsAllDisableAiEnabled
        // {
        //     get => _isAllDisableAiEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isAllDisableAiEnabled, value))
        //         {
        //             _enemyService.ToggleDebugFlag(Offsets.DebugFlags.DisableAllAi, _isAllDisableAiEnabled ? 1 : 0);
        //         }
        //     }
        // }
        //
        // public bool IsAllNoDamageEnabled
        // {
        //     get => _isAllNoDamageEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isAllNoDamageEnabled, value))
        //         {
        //             _enemyService.ToggleDebugFlag(Offsets.DebugFlags.AllNoDamage, _isAllNoDamageEnabled ? 1 : 0);
        //         }
        //     }
        // }
        //
        // public bool IsAllNoDeathEnabled
        // {
        //     get => _isAllNoDeathEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isAllNoDeathEnabled, value))
        //         {
        //             _enemyService.ToggleDebugFlag(Offsets.DebugFlags.AllNoDeath, _isAllNoDeathEnabled ? 1 : 0);
        //         }
        //     }
        // }
        //
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
        //
        // public bool IsCinderPhasedLocked
        // {
        //     get => _isCinderPhaseLocked;
        //     set
        //     {
        //         if (SetProperty(ref _isCinderPhaseLocked, value))
        //         {
        //             _cinderService.ToggleCinderPhaseLock(_isCinderPhaseLocked);
        //         }
        //     }
        // }
        //
        // public bool IsEndlessSoulmassEnabled
        // {
        //     get => _isEndlessSoulmassEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isEndlessSoulmassEnabled, value))
        //         {
        //             _cinderService.ToggleEndlessSoulmass(_isEndlessSoulmassEnabled);
        //         }
        //     }
        // }
        //
        // public bool IsCinderNoStaggerEnabled
        // {
        //     get => _isCinderNoStaggerEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isCinderNoStaggerEnabled, value))
        //         {
        //             _cinderService.ToggleCinderStagger(_isCinderNoStaggerEnabled);
        //         }
        //     }
        // }
        //
        // public void SetCinderPhase(int phaseIndex) => _cinderService.ForcePhaseTransition(phaseIndex);
        // public void CastSoulmass() => _cinderService.CastSoulMass();
        //

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
                if (SetProperty(ref _selectedForlorn, value))
                {
                    CurrentAreaName = value?.AreaName ?? "No Forlorn selected";
                    IsForlornAvailable = value != null;
                    _selectedForlornIndex = 0;
                    OnPropertyChanged(nameof(ForlornIndexes));
                }
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
                if (SetProperty(ref _selectedForlornIndex, value)) 
                {
                    if (SelectedForlorn != null && IsGuaranteedSpawnEnabled)
                    {
                        _enemyService.UpdateForlornIndex(_selectedForlornIndex + 1);
                        _enemyService.ToggleForlornSpawn(true, 
                            SelectedForlorn.EsdFuncId,
                            _selectedForlornIndex + 1);
                    }
                }
            }
        }


        public bool IsGuaranteedSpawnEnabled
        {
            get => _isGuaranteedSpawnEnabled;
            set
            {
                if (SetProperty(ref _isGuaranteedSpawnEnabled, value))
                {
                    if (SelectedForlorn != null)
                    {
                        _enemyService.ToggleForlornSpawn(_isGuaranteedSpawnEnabled, 
                            SelectedForlorn.EsdFuncId,
                            SelectedForlornIndex + 1);
                    }
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

        // public IEnumerable<int> SpawnPositions
        // {
        //     get
        //     {
        //         if (_currentForlorn == null)
        //             return Enumerable.Empty<int>();
        //         
        //         int baseValue = 80000000 + (_selectedForlornIndex - 1) * 100;
        //         return new[] { baseValue + 1, baseValue + 99 };
        //     }
        // }

        public void TryEnableFeatures()
        {
            // if (IsTargetOptionsEnabled)
            // {
            //     _enemyService.InstallTargetHook();
            //     _targetOptionsTimer.Start();
            // }

            // if (IsAllDisableAiEnabled) _enemyService.ToggleDebugFlag(Offsets.DebugFlags.DisableAllAi, 1);
            // if (IsAllNoDamageEnabled) _enemyService.ToggleDebugFlag(Offsets.DebugFlags.AllNoDamage, 1);
            // if (IsAllNoDeathEnabled) _enemyService.ToggleDebugFlag(Offsets.DebugFlags.AllNoDeath, 1);
            // if (IsAllRepeatActEnabled) _enemyService.ToggleAllRepeatAct(true);
            // AreOptionsEnabled = true;
        }
        //
        // public void DisableFeatures()
        // {
        //     _targetOptionsTimer.Stop();
        //     IsFreezeHealthEnabled = false;
        //     IsCinderPhasedLocked = false;
        //     LastAct = 0;
        //     ForceAct = 0;
        //     IsCinderNoStaggerEnabled = false;
        //     IsEndlessSoulmassEnabled = false;
        //     AreOptionsEnabled = false;
        // }
    }
}