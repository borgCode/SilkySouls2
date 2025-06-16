﻿using System;
using System.Reflection;
using System.Windows.Threading;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.Views;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        private int _currentHp;
        private int _currentMaxHp;

        private bool _isPos1Saved;
        private bool _isPos2Saved;
        private bool _isStateIncluded;
        private (float x, float y, float z) _coords;
        private float _posX;
        private float _posZ;
        private float _posY;
        private CharacterState _saveState1 = new CharacterState();
        private CharacterState _saveState2 = new CharacterState();

        private bool _isNoDeathEnabled;
        private bool _isNoDamageEnabled;
        private bool _isInfiniteStaminaEnabled;
        private bool _isNoGoodsConsumeEnabled;
        private bool _isInfiniteCastsEnabled;
        private bool _isInfiniteDurabilityEnabled;
        private bool _isOneShotEnabled;
        private bool _isDealNoDamageEnabled;
        private bool _isHiddenEnabled;
        private bool _isSilentEnabled;
        private bool _isNoSoulGainEnabled;
        private bool _isNoSoulLossEnabled;
        private bool _isNoHollowingEnabled;
        private bool _isInfinitePoiseEnabled;
        private bool _isAutoSetNewGameSevenEnabled;

        private int _vigor;
        private int _attunement;
        private int _endurance;
        private int _strength;
        private int _dexterity;
        private int _intelligence;
        private int _faith;
        private int _adp;
        private int _vitality;
        private int _soulLevel;
        private int _soulMemory;
        private int _souls = 10000; 
        private int _newGame;
        private float _playerSpeed;
        private int _currentSoulLevel;

        private HealthWindow _healthWindow;
        private bool _isHealthWindowOpen;

        private float _playerDesiredSpeed = -1f;
        private const float DefaultSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private bool _pauseUpdates = true;
        private bool _areOptionsEnabled;
        private readonly DispatcherTimer _timer;

        private readonly PlayerService _playerService;
        private readonly DamageControlService _damageControlService;
        private readonly HotkeyManager _hotkeyManager;

        public PlayerViewModel(PlayerService playerService, HotkeyManager hotkeyManager,
            DamageControlService damageControlService)
        {
            _playerService = playerService;
            _damageControlService = damageControlService;
            _hotkeyManager = hotkeyManager;


            RegisterHotkeys();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += (s, e) =>
            {
                if (_pauseUpdates) return;

                CurrentHp = _playerService.GetHp();
                CurrentMaxHp = _playerService.GetMaxHp();
                PlayerSpeed = _playerService.GetPlayerSpeed();
                int newSoulLevel = _playerService.GetSoulLevel();
                SoulMemory = _playerService.GetSoulMemory();
                _coords = _playerService.GetCoords();
                PosX = _coords.x;
                PosY = _coords.y;
                PosZ = _coords.z;
                if (_currentSoulLevel == newSoulLevel) return;
                SoulLevel = newSoulLevel;
                _currentSoulLevel = newSoulLevel;
                LoadStats();
            };
            _timer.Start();
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction("SavePos1", () => SavePos(0));
            _hotkeyManager.RegisterAction("SavePos2", () => SavePos(1));
            _hotkeyManager.RegisterAction("RestorePos1", () => RestorePos(0));
            _hotkeyManager.RegisterAction("RestorePos2", () => RestorePos(1));
            _hotkeyManager.RegisterAction("RTSR", SetRtsr);
            _hotkeyManager.RegisterAction("NoDeath", () => { IsNoDeathEnabled = !IsNoDeathEnabled; });
            _hotkeyManager.RegisterAction("OneShot", () => { IsOneShotEnabled = !IsOneShotEnabled; });
            _hotkeyManager.RegisterAction("DealNoDamage", () => { IsDealNoDamageEnabled = !IsDealNoDamageEnabled; });
            _hotkeyManager.RegisterAction("PlayerNoDamage", () => { IsNoDamageEnabled = !IsNoDamageEnabled; });
            _hotkeyManager.RegisterAction("RestoreSpellcasts", () =>
            {
                if (!AreOptionsEnabled) return;
                _playerService.RestoreSpellcasts();
            });
            _hotkeyManager.RegisterAction("RestoreHumanity", () =>
            {
                if (!AreOptionsEnabled) return;
                _playerService.SetSpEffect(GameIds.SpEffects.SpEffectData.RestoreHumanity);
            });
            
            _hotkeyManager.RegisterAction("Rest", () =>
            {
                if (!AreOptionsEnabled) return;
                _playerService.SetSpEffect(GameIds.SpEffects.SpEffectData.BonfireRest);
            });
            _hotkeyManager.RegisterAction("TogglePlayerSpeed", ToggleSpeed);
            _hotkeyManager.RegisterAction("IncreasePlayerSpeed", () => SetSpeed(Math.Min(10, PlayerSpeed + 0.25f)));
            _hotkeyManager.RegisterAction("DecreasePlayerSpeed", () => SetSpeed(Math.Max(0, PlayerSpeed - 0.25f)));
        }

        private void LoadStats()
        {
            Vigor = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Vigor);
            Endurance = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Endurance);
            Vitality = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Vitality);
            Attunement = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Attunement);
            Strength = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Strength);
            Dexterity = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Dexterity);
            Adp = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Adp);
            Intelligence = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Intelligence);
            Faith = _playerService.GetPlayerStat(GameManagerImp.ChrCtrlOffsets.Stats.Faith);
            SoulLevel = _playerService.GetSoulLevel();
            NewGame = _playerService.GetNewGame();
            PlayerSpeed = _playerService.GetPlayerSpeed();
        }


        public void PauseUpdates()
        {
            _pauseUpdates = true;
        }

        public void ResumeUpdates()
        {
            _pauseUpdates = false;
        }

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        public int CurrentHp
        {
            get => _currentHp;
            set => SetProperty(ref _currentHp, value);
        }

        public int CurrentMaxHp
        {
            get => _currentMaxHp;
            set => SetProperty(ref _currentMaxHp, value);
        }

        public void SetHp(int hp)
        {
            _playerService.SetHp(hp);
            CurrentHp = hp;
        }

        public void SetRtsr() => _playerService.SetRtsr();

        public void SetMaxHp() =>_playerService.SetFullHp();
  
        
        public bool IsHealthWindowOpen
        {
            get => _isHealthWindowOpen;
            set
            {
                if (!SetProperty(ref _isHealthWindowOpen, value)) return;
                if (value)
                {
                    if (_healthWindow != null && _healthWindow.IsVisible) return;
                    _healthWindow = new HealthWindow { DataContext = this };
                    _healthWindow.Closed += (sender, args) => _isHealthWindowOpen = false;
                    _healthWindow.Show();
                }
                else
                {
                    if (_healthWindow == null || !_healthWindow.IsVisible) return;
                    _healthWindow.Close();
                    _healthWindow = null;
                }
            }
        }

        public bool IsPos1Saved
        {
            get => _isPos1Saved;
            set => SetProperty(ref _isPos1Saved, value);
        }
        
        public bool IsPos2Saved
        {
            get => _isPos2Saved;
            set => SetProperty(ref _isPos2Saved, value);
        }
        
        public void SavePos(int index)
        {
            var state = index == 0 ? _saveState1 : _saveState2;
            if (index == 0) IsPos1Saved = true;
            else IsPos2Saved = true;
        
            if (IsStateIncluded)
            {
                state.Hp = CurrentHp;
                state.Sp = _playerService.GetSp();
            }
            _playerService.SavePos(index);
        }
        
        public void RestorePos(int index)
        {
            _playerService.RestorePos(index);
            if (!IsStateIncluded) return;
        
            var state = index == 0 ? _saveState1 : _saveState2;
            _playerService.SetHp(state.Hp);
            _playerService.SetSp(state.Sp);
        }
        
        public bool IsStateIncluded
        {
            get => _isStateIncluded;
            set => SetProperty(ref _isStateIncluded, value);
        }
        public bool IsNoDeathEnabled
        {
            get => _isNoDeathEnabled;
            set
            {
                if (SetProperty(ref _isNoDeathEnabled, value))
                {
                    _playerService.ToggleNoDeath(_isNoDeathEnabled);
                }
            }
        }
        
        public float PosX
        {
            get => _posX;
            set => SetProperty(ref _posX, value);
        }

        public float PosY
        {
            get => _posY;
            set => SetProperty(ref _posY, value);
        }

        public float PosZ
        {
            get => _posZ;
            set => SetProperty(ref _posZ, value);
        }

        public bool IsNoDamageEnabled
        {
            get => _isNoDamageEnabled;
            set
            {
                if (SetProperty(ref _isNoDamageEnabled, value))
                {
                    _playerService.ToggleNoDamage(_isNoDamageEnabled);
                }
            }
        }

        public bool IsInfiniteStaminaEnabled
        {
            get => _isInfiniteStaminaEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteStaminaEnabled, value))
                {
                    _playerService.ToggleInfiniteStamina(_isInfiniteStaminaEnabled);
                }
            }
        }
        
        public bool IsNoGoodsConsumeEnabled
        {
            get => _isNoGoodsConsumeEnabled;
            set
            {
                if (SetProperty(ref _isNoGoodsConsumeEnabled, value))
                {
                    _playerService.ToggleNoGoodsConsume(_isNoGoodsConsumeEnabled);
                }
            }
        }
        
        public bool IsInfiniteDurabilityEnabled
        {
            get => _isInfiniteDurabilityEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteDurabilityEnabled, value))
                {
                    _playerService.ToggleInfiniteDurability(_isInfiniteDurabilityEnabled);
                }
            }
        }
        
        
        public bool IsInfiniteCastsEnabled
        {
            get => _isInfiniteCastsEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteCastsEnabled, value))
                {
                    _playerService.ToggleInfiniteCasts(_isInfiniteCastsEnabled);
                }
            }
        }
        

        public bool IsDealNoDamageEnabled
        {
            get => _isDealNoDamageEnabled;
            set
            {
                if (!SetProperty(ref _isDealNoDamageEnabled, value)) return;
                if (IsOneShotEnabled && _isDealNoDamageEnabled)
                {
                    _damageControlService.ToggleOneShot(false);
                    IsOneShotEnabled = false;
                }

                _damageControlService.ToggleDealNoDamage(_isDealNoDamageEnabled);
            }
        }

        public bool IsOneShotEnabled
        {
            get => _isOneShotEnabled;
            set
            {
                if (!SetProperty(ref _isOneShotEnabled, value)) return;
                if (IsDealNoDamageEnabled && _isOneShotEnabled)
                {
                    _damageControlService.ToggleDealNoDamage(false);
                    IsDealNoDamageEnabled = false;
                }

                _damageControlService.ToggleOneShot(_isOneShotEnabled);
            }
        }
        
        public bool IsHiddenEnabled
        {
            get => _isHiddenEnabled;
            set
            {
                if (SetProperty(ref _isHiddenEnabled, value))
                {
                    _playerService.ToggleHidden(_isHiddenEnabled);
                }
            }
        }
        
        public bool IsSilentEnabled
        {
            get => _isSilentEnabled;
            set
            {
                if (SetProperty(ref _isSilentEnabled, value))
                {
                    _playerService.ToggleSilent(_isSilentEnabled);
                }
            }
        }
        
        public bool IsNoSoulLossEnabled
        {
            get => _isNoSoulLossEnabled;
            set
            {
                if (SetProperty(ref _isNoSoulLossEnabled, value))
                {
                    _playerService.ToggleNoSoulLoss(_isNoSoulLossEnabled);
                }
            }
        }
        
        public bool IsNoSoulGainEnabled
        {
            get => _isNoSoulGainEnabled;
            set
            {
                if (SetProperty(ref _isNoSoulGainEnabled, value))
                {
                    _playerService.ToggleNoSoulGain(_isNoSoulGainEnabled);
                }
            }
        }
        
        public bool IsNoHollowingEnabled
        {
            get => _isNoHollowingEnabled;
            set
            {
                if (SetProperty(ref _isNoHollowingEnabled, value))
                {
                    _playerService.ToggleNoHollowing(_isNoHollowingEnabled);
                }
            }
        }
 
        public bool IsInfinitePoiseEnabled
        {
            get => _isInfinitePoiseEnabled;
            set
            {
                if (SetProperty(ref _isInfinitePoiseEnabled, value))
                {
                    _playerService.ToggleInfinitePoise(_isInfinitePoiseEnabled);
                }
            }
        }
        
        public bool IsAutoSetNewGameSevenEnabled
        {
            get => _isAutoSetNewGameSevenEnabled;
            set
            {
                if (SetProperty(ref _isAutoSetNewGameSevenEnabled, value))
                {
                    _playerService.ToggleAutoSetNg7(_isAutoSetNewGameSevenEnabled);
                }
            }
        }
        //
        // public bool IsNoRollEnabled
        // {
        //     get => _isNoRollEnabled;
        //     set
        //     {
        //         if (!SetProperty(ref _isNoRollEnabled, value)) return;
        //         _playerService.ToggleNoRoll(_isNoRollEnabled);
        //     }
        // }
        //
        
        public int Vigor
        {
            get => _vigor;
            set => SetProperty(ref _vigor, value);
        }

        public int Attunement
        {
            get => _attunement;
            set => SetProperty(ref _attunement, value);
        }

        public int Endurance
        {
            get => _endurance;
            set => SetProperty(ref _endurance, value);
        }

        public int Strength
        {
            get => _strength;
            set => SetProperty(ref _strength, value);
        }

        public int Dexterity
        {
            get => _dexterity;
            set => SetProperty(ref _dexterity, value);
        }

        public int Intelligence
        {
            get => _intelligence;
            set => SetProperty(ref _intelligence, value);
        }

        public int Faith
        {
            get => _faith;
            set => SetProperty(ref _faith, value);
        }

        public int Adp
        {
            get => _adp;
            set => SetProperty(ref _adp, value);
        }

        public int Vitality
        {
            get => _vitality;
            set => SetProperty(ref _vitality, value);
        }

        public int SoulLevel
        {
            get => _soulLevel;
            private set => SetProperty(ref _soulLevel, value);
        }
        
        public int SoulMemory
        {
            get => _soulMemory;
            private set => SetProperty(ref _soulMemory, value);
        }
        
        public void SetStat(string statName, int value)
        {
            var field = typeof(GameManagerImp.ChrCtrlOffsets.Stats)
                .GetField(statName, BindingFlags.Public | BindingFlags.Static);
            
            if (field != null)
            {
                int statOffset = (int)field.GetValue(null);
                _playerService.SetPlayerStat(statOffset, (byte)value);
            }
            else
            {
                throw new ArgumentException($"Invalid stat name: {statName}");
            }
        }
        
        public int Souls
        {
            get => _souls;
            set => SetProperty(ref _souls, value);
        }
        
        public int NewGame
        {
            get => _newGame;
            set
            {
                if (SetProperty(ref _newGame, value))
                {
                    _playerService.SetNewGame(value);
                }
            }
        }

        public float PlayerSpeed
        {
            get => _playerSpeed;
            set
            {
                if (SetProperty(ref _playerSpeed, value))
                {
                    _playerService.SetPlayerSpeed(value);
                }
            }
        }
        
        public void SetSpeed(float value) => PlayerSpeed = value;
        
        private void ToggleSpeed()
        {
            if (!AreOptionsEnabled) return;
        
            if (!IsApproximately(PlayerSpeed, DefaultSpeed))
            {
                _playerDesiredSpeed = PlayerSpeed;
                SetSpeed(DefaultSpeed);
            }
            else if (_playerDesiredSpeed >= 0)
            {
                SetSpeed(_playerDesiredSpeed);
            }
        }
        
        private bool IsApproximately(float a, float b)
        {
            return Math.Abs(a - b) < Epsilon;
        }
        
        
        public void TryEnableFeatures()
        {
    
            if (IsNoDeathEnabled) _playerService.ToggleNoDeath(true);
            
            AreOptionsEnabled = true;
            LoadStats();
            _timer.Start();
        }
        
        public void TryApplyOneTimeFeatures()
        {
            if (IsOneShotEnabled) _damageControlService.ToggleOneShot(true);
            if (IsDealNoDamageEnabled) _damageControlService.ToggleDealNoDamage(true);
            if (IsNoDamageEnabled) _playerService.ToggleNoDamage(true);
            if (IsInfiniteStaminaEnabled) _playerService.ToggleInfiniteStamina(true);
            if (IsNoGoodsConsumeEnabled) _playerService.ToggleNoGoodsConsume(true);
            if (IsInfiniteCastsEnabled) _playerService.ToggleInfiniteCasts(true);
            if (IsInfiniteDurabilityEnabled) _playerService.ToggleInfiniteDurability(true);
            if (IsInfinitePoiseEnabled) _playerService.ToggleInfinitePoise(true);
            if (IsSilentEnabled) _playerService.ToggleSilent(true);
            if (IsHiddenEnabled) _playerService.ToggleHidden(true);
            if (IsNoSoulGainEnabled) _playerService.ToggleNoSoulGain(true);
            if (IsNoSoulLossEnabled) _playerService.ToggleNoSoulLoss(true);
            if (IsNoHollowingEnabled) _playerService.ToggleNoHollowing(true);
            _pauseUpdates = false;
        }

        public void DisableFeatures()
        {
            AreOptionsEnabled = false;
            _timer.Stop();
        }
        
        public void GiveSouls() => _playerService.GiveSouls(Souls);

        public void RestoreSpellcasts() => _playerService.RestoreSpellcasts();

        public void ApplyLaunchFeatures()
        {
            if (IsAutoSetNewGameSevenEnabled) _playerService.ToggleAutoSetNg7(true);
        }

        public void RestoreHumanity() => _playerService.SetSpEffect(GameIds.SpEffects.SpEffectData.RestoreHumanity);

        public void Rest() => _playerService.SetSpEffect(GameIds.SpEffects.SpEffectData.BonfireRest);
    }
}