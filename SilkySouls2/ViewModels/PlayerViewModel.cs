using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
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
        private bool _isInvisibleEnabled;
        private bool _isSilentEnabled;
        private bool _isNoAmmoConsumeEnabled;
        private bool _isInfinitePoiseEnabled;
        private bool _isAutoSetNewGameSevenEnabled;
        private bool _isNoRollEnabled;

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
        private int _souls;
        private int _newGame;
        private float _playerSpeed;
        private int _currentSoulLevel;

        private float _playerDesiredSpeed = -1f;
        private const float DefaultSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private bool _pauseUpdates;
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
                // Souls = _playerService.GetPlayerStat(Offsets.GameManagerImp.PlayerCtrlOffsets.Stats.Souls);
                PlayerSpeed = _playerService.GetPlayerSpeed();
                int newSoulLevel = _playerService.GetSoulLevel();
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
            _hotkeyManager.RegisterAction("RTSR", () => SetHp(1));
            _hotkeyManager.RegisterAction("NoDeath", () => { IsNoDeathEnabled = !IsNoDeathEnabled; });
            // _hotkeyManager.RegisterAction("OneShot", () => { IsOneShotEnabled = !IsOneShotEnabled; });
            // _hotkeyManager.RegisterAction("PlayerNoDamage", () => { IsNoDamageEnabled = !IsNoDamageEnabled; });
            _hotkeyManager.RegisterAction("TogglePlayerSpeed", ToggleSpeed);
            _hotkeyManager.RegisterAction("IncreasePlayerSpeed", () => SetSpeed(Math.Min(10, PlayerSpeed + 0.25f)));
            _hotkeyManager.RegisterAction("DecreasePlayerSpeed", () => SetSpeed(Math.Max(0, PlayerSpeed - 0.25f)));
        }

        private void LoadStats()
        {
            Vigor = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.Vig);
            Endurance = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.End);
            Vitality = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.Vit);
            Attunement = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.Atn);
            Strength = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.Str);
            Dexterity = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.Dex);
            Adp = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.Adp);
            Intelligence = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.Int);
            Faith = _playerService.GetPlayerStat(GameManagerImp.PlayerCtrlOffsets.Stats.Fth);
            SoulLevel = _playerService.GetSoulLevel();
            // Souls = _playerService.GetPlayerStat(Offsets.GameManagerImp.PlayerCtrlOffsets.Stats.Souls);
            NewGame = _playerService.GetNewGame();
            PlayerSpeed = _playerService.GetPlayerSpeed();
            UpdateAgilityAndIFrames();
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

        public void SetMaxHp()
        {
            _playerService.SetHp(CurrentMaxHp);
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

        
       
        

        //
        // public bool IsInvisibleEnabled
        // {
        //     get => _isInvisibleEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isInvisibleEnabled, value))
        //         {
        //             _playerService.ToggleDebugFlag(DebugFlags.Invisible, _isInvisibleEnabled ? 1 : 0);
        //         }
        //     }
        // }
        //
        // public bool IsSilentEnabled
        // {
        //     get => _isSilentEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isSilentEnabled, value))
        //         {
        //             _playerService.ToggleDebugFlag(DebugFlags.Silent, _isSilentEnabled ? 1 : 0);
        //         }
        //     }
        // }
        //
        // public bool IsNoAmmoConsumeEnabled
        // {
        //     get => _isNoAmmoConsumeEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isNoAmmoConsumeEnabled, value))
        //         {
        //             _playerService.ToggleDebugFlag(DebugFlags.InfiniteArrows, _isNoAmmoConsumeEnabled ? 1 : 0);
        //         }
        //     }
        // }
        //
        // public bool IsInfinitePoiseEnabled
        // {
        //     get => _isInfinitePoiseEnabled;
        //     set
        //     {
        //         if (SetProperty(ref _isInfinitePoiseEnabled, value))
        //         {
        //             _playerService.ToggleInfinitePoise(_isInfinitePoiseEnabled);
        //         }
        //     }
        // }
        //
        // public bool IsAutoSetNewGameSevenEnabled
        // {
        //     get => _isAutoSetNewGameSevenEnabled;
        //     set => SetProperty(ref _isAutoSetNewGameSevenEnabled, value);
        // }
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
        public void TryEnableFeatures()
        {
            if (IsOneShotEnabled) _damageControlService.ToggleOneShot(true);
            if (IsDealNoDamageEnabled) _damageControlService.ToggleDealNoDamage(true);
            if (IsInfiniteStaminaEnabled) _playerService.ToggleInfiniteStamina(true);
            if (IsNoGoodsConsumeEnabled) _playerService.ToggleNoGoodsConsume(true);
            if (IsInfiniteCastsEnabled) _playerService.ToggleInfiniteCasts(true);
            // if (IsInvisibleEnabled)
            //     _playerService.ToggleDebugFlag(DebugFlags.Invisible, 1);
            // if (IsSilentEnabled)
            //     _playerService.ToggleDebugFlag(DebugFlags.Silent, 1);
            // if (IsNoAmmoConsumeEnabled)
            //     _playerService.ToggleDebugFlag(DebugFlags.InfiniteArrows, 1);
            // if (IsInfinitePoiseEnabled)
            //     _playerService.ToggleInfinitePoise(true);
            if (IsInfiniteDurabilityEnabled) _playerService.ToggleInfiniteDurability(true);
            // if (IsNoRollEnabled)
            //     _playerService.ToggleNoRoll(true);
            AreOptionsEnabled = true;
            LoadStats();
            _timer.Start();
        }

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
        //
        // public void SetStat(string statName, int value)
        // {
        //     Offsets.GameManagerImp.PlayerCtrlOffsets.Stats stat = (Offsets.GameManagerImp.PlayerCtrlOffsets.Stats)Enum.Parse(typeof(Offsets.GameManagerImp.PlayerCtrlOffsets.Stats), statName);
        //     _playerService.SetPlayerStat(stat, value);
        // }
        //
        // public int Souls
        // {
        //     get => _souls;
        //     set => SetProperty(ref _souls, value);
        // }
        //
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

        private int _agility;

        public int Agility
        {
            get => _agility;
            private set => SetProperty(ref _agility, value);
        }

        private string _rollIFrames;

        public string RollIFrames
        {
            get => _rollIFrames;
            private set => SetProperty(ref _rollIFrames, value);
        }

        private string _backstepIFrames;

        public string BackstepIFrames
        {
            get => _backstepIFrames;
            private set => SetProperty(ref _backstepIFrames, value);
        }

        private string _nextBreakpoint;

        public string NextBreakpoint
        {
            get => _nextBreakpoint;
            private set => SetProperty(ref _nextBreakpoint, value);
        }


        private static readonly Dictionary<int, int> RollFrames = new Dictionary<int, int>
        {
            { 85, 5 }, { 86, 8 }, { 88, 9 },
            { 92, 10 }, { 96, 11 }, { 99, 12 },
            { 105, 13 }, { 111, 14 }, { 114, 15 }, { 116, 16 }
        };

        private static readonly Dictionary<int, int> BackstepFrames = new Dictionary<int, int>
        {
            { 85, 3 }, { 87, 5 }, { 91, 6 },
            { 96, 7 }, { 99, 8 }, { 105, 9 }
        };

        private void UpdateAgilityAndIFrames()
        {
            int baseValue = Attunement + (3 * Adp);
            int calculatedAgility;
    
            if (Attunement == 99 && Adp == 99)
                calculatedAgility = 120;
            else if (baseValue <= 120)
                calculatedAgility = 80 + baseValue / 4;
            else
                calculatedAgility = 110 + (baseValue - 120) / 28;
    
            Agility = Math.Max(85, calculatedAgility);
    
            int currentRoll = 0;
            int currentBackstep = 0;
    
            foreach (var entry in RollFrames.OrderBy(e => e.Key))
                if (Agility >= entry.Key) currentRoll = entry.Value;
    
            foreach (var entry in BackstepFrames.OrderBy(e => e.Key))
                if (Agility >= entry.Key) currentBackstep = entry.Value;
    
            RollIFrames = $"{currentRoll} iframes";
            BackstepIFrames = $"{currentBackstep} iframes";
    
            int nextRoll = RollFrames.Keys.OrderBy(k => k).FirstOrDefault(k => k > Agility);
    
            if (nextRoll == 0)
            {
                NextBreakpoint = "Maximum roll iFrames reached";
                return;
            }
    
            NextBreakpoint = $"Next iframe at {nextRoll} agility";
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
        
        // public void TrySetNgPref()
        // {
        //     if (!IsAutoSetNewGameSevenEnabled) return;
        //     _playerService.SetNewGame(8);
        //     NewGame = _playerService.GetNewGame();
        // }
        //
        // public void GiveSouls()
        // {
        //     _playerService.GiveSouls();
        // }
        public void TryApplyOneTimeFeatures()
        {
            if (IsNoDeathEnabled) _playerService.ToggleNoDeath(true);
            if (IsNoDamageEnabled) _playerService.ToggleNoDamage(true);
        }

        public void DisableFeatures()
        {
            AreOptionsEnabled = false;
            _timer.Stop();
        }

        public void SetStat(string statName, int upDownValue)
        {
            // throw new NotImplementedException();
        }

        public void GiveSouls()
        {
            // throw new NotImplementedException();
        }
    }
}