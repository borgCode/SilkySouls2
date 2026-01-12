using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Threading;
using SilkySouls2.Core;
using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
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
        private CharacterState _saveState1 = new();
        private CharacterState _saveState2 = new();
        private int _currentSoulLevel;
        private HealthWindow _healthWindow;

        private float _playerDesiredSpeed = -1f;
        private const float DefaultSpeed = 1f;
        private const float Epsilon = 0.0001f;

        private bool _pauseUpdates = true;
        private bool _customHpHasBeenSet;

        private readonly DispatcherTimer _playerTick;

        private readonly IPlayerService _playerService;
        private readonly DamageControlService _damageControlService;
        private readonly INewGameService _newGameService;
        private readonly HotkeyManager _hotkeyManager;

        public PlayerViewModel(IPlayerService playerService, HotkeyManager hotkeyManager,
            DamageControlService damageControlService, GameStateService gameStateService, INewGameService newGameService)
        {
            _playerService = playerService;
            _damageControlService = damageControlService;
            _newGameService = newGameService;
            _hotkeyManager = hotkeyManager;

            gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
            gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);
            gameStateService.Subscribe(GameState.FirstLoaded, OnGameFirstLoaded);
            gameStateService.Subscribe(GameState.NewGameStarted, OnNewGameStarted);
            gameStateService.Subscribe(GameState.Attached, OnGameAttached);

            RegisterHotkeys();

            SetRtsrCommand = new DelegateCommand(SetRtsr);
            SetMaxHpCommand = new DelegateCommand(SetMaxHp);
            SetCustomHpCommand = new DelegateCommand(SetCustomHp);
            DieCommand = new DelegateCommand(Die);

            SavePositionCommand = new DelegateCommand(SavePosition);
            RestorePositionCommand = new DelegateCommand(RestorePosition);

            RestoreSpellcastsCommand = new DelegateCommand(RestoreSpellcasts);
            RestoreHumanityCommand = new DelegateCommand(RestoreHumanity);
            RestCommand = new DelegateCommand(Rest);

            GiveSoulsCommand = new DelegateCommand(GiveSouls);


            _playerTick = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _playerTick.Tick += PlayerTick;
        }

        #region Commands

        public ICommand SetRtsrCommand { get; set; }
        public ICommand SetMaxHpCommand { get; set; }
        public ICommand SetCustomHpCommand { get; set; }
        public ICommand DieCommand { get; set; }

        public ICommand SavePositionCommand { get; set; }
        public ICommand RestorePositionCommand { get; set; }

        public ICommand RestoreSpellcastsCommand { get; set; }
        public ICommand RestoreHumanityCommand { get; set; }
        public ICommand RestCommand { get; set; }
        public ICommand GiveSoulsCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private int _currentHp;

        public int CurrentHp
        {
            get => _currentHp;
            set => SetProperty(ref _currentHp, value);
        }

        private int _currentMaxHp;

        public int CurrentMaxHp
        {
            get => _currentMaxHp;
            set => SetProperty(ref _currentMaxHp, value);
        }

        private string _customHp;

        public string CustomHp
        {
            get => _customHp;
            set
            {
                if (SetProperty(ref _customHp, value))
                {
                    _customHpHasBeenSet = true;
                }
            }
        }

        private bool _isHealthWindowOpen;

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
                    _healthWindow.Closed += (_, _) => _isHealthWindowOpen = false;
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

        private bool _isPos1Saved;

        public bool IsPos1Saved
        {
            get => _isPos1Saved;
            set => SetProperty(ref _isPos1Saved, value);
        }

        private bool _isPos2Saved;

        public bool IsPos2Saved
        {
            get => _isPos2Saved;
            set => SetProperty(ref _isPos2Saved, value);
        }

        private float _posX;

        public float PosX
        {
            get => _posX;
            set => SetProperty(ref _posX, value);
        }

        private float _posY;

        public float PosY
        {
            get => _posY;
            set => SetProperty(ref _posY, value);
        }

        private float _posZ;

        public float PosZ
        {
            get => _posZ;
            set => SetProperty(ref _posZ, value);
        }

        private bool _isNoDeathEnabled;

        public bool IsNoDeathEnabled
        {
            get => _isNoDeathEnabled;
            set
            {
                if (SetProperty(ref _isNoDeathEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleNoDeath(_isNoDeathEnabled);
                }
            }
        }

        private bool _isNoDamageEnabled;

        public bool IsNoDamageEnabled
        {
            get => _isNoDamageEnabled;
            set
            {
                if (SetProperty(ref _isNoDamageEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleNoDamage(_isNoDamageEnabled);
                }
            }
        }

        private bool _isInfiniteStaminaEnabled;

        public bool IsInfiniteStaminaEnabled
        {
            get => _isInfiniteStaminaEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteStaminaEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleInfiniteStamina(_isInfiniteStaminaEnabled);
                }
            }
        }

        private bool _isNoGoodsConsumeEnabled;

        public bool IsNoGoodsConsumeEnabled
        {
            get => _isNoGoodsConsumeEnabled;
            set
            {
                if (SetProperty(ref _isNoGoodsConsumeEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleNoGoodsConsume(_isNoGoodsConsumeEnabled);
                }
            }
        }

        private bool _isInfiniteDurabilityEnabled;

        public bool IsInfiniteDurabilityEnabled
        {
            get => _isInfiniteDurabilityEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteDurabilityEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleInfiniteDurability(_isInfiniteDurabilityEnabled);
                }
            }
        }

        private bool _isInfiniteCastsEnabled;

        public bool IsInfiniteCastsEnabled
        {
            get => _isInfiniteCastsEnabled;
            set
            {
                if (SetProperty(ref _isInfiniteCastsEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleInfiniteCasts(_isInfiniteCastsEnabled);
                }
            }
        }

        private bool _isDealNoDamageEnabled;

        public bool IsDealNoDamageEnabled
        {
            get => _isDealNoDamageEnabled;
            set
            {
                if (!SetProperty(ref _isDealNoDamageEnabled, value)) return;
                if (!PatchManager.IsInitialized) return;
                if (IsOneShotEnabled && _isDealNoDamageEnabled)
                {
                    _damageControlService.ToggleOneShot(false);
                    IsOneShotEnabled = false;
                }

                _damageControlService.ToggleDealNoDamage(_isDealNoDamageEnabled);
            }
        }

        private bool _isOneShotEnabled;

        public bool IsOneShotEnabled
        {
            get => _isOneShotEnabled;
            set
            {
                if (!SetProperty(ref _isOneShotEnabled, value)) return;
                if (!PatchManager.IsInitialized) return;
                if (IsDealNoDamageEnabled && _isOneShotEnabled)
                {
                    _damageControlService.ToggleDealNoDamage(false);
                    IsDealNoDamageEnabled = false;
                }

                _damageControlService.ToggleOneShot(_isOneShotEnabled);
            }
        }

        private bool _isHiddenEnabled;

        public bool IsHiddenEnabled
        {
            get => _isHiddenEnabled;
            set
            {
                if (SetProperty(ref _isHiddenEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleHidden(_isHiddenEnabled);
                }
            }
        }

        private bool _isSilentEnabled;

        public bool IsSilentEnabled
        {
            get => _isSilentEnabled;
            set
            {
                if (SetProperty(ref _isSilentEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleSilent(_isSilentEnabled);
                }
            }
        }

        private bool _isNoSoulLossEnabled;

        public bool IsNoSoulLossEnabled
        {
            get => _isNoSoulLossEnabled;
            set
            {
                if (SetProperty(ref _isNoSoulLossEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleNoSoulLoss(_isNoSoulLossEnabled);
                }
            }
        }

        private bool _isNoSoulGainEnabled;

        public bool IsNoSoulGainEnabled
        {
            get => _isNoSoulGainEnabled;
            set
            {
                if (SetProperty(ref _isNoSoulGainEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleNoSoulGain(_isNoSoulGainEnabled);
                }
            }
        }

        private bool _isNoHollowingEnabled;

        public bool IsNoHollowingEnabled
        {
            get => _isNoHollowingEnabled;
            set
            {
                if (SetProperty(ref _isNoHollowingEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleNoHollowing(_isNoHollowingEnabled);
                }
            }
        }

        private bool _isHotEnabled;

        public bool IsHotEnabled
        {
            get => _isHotEnabled;
            set => SetProperty(ref _isHotEnabled, value);
        }

        private bool _isInfinitePoiseEnabled;

        public bool IsInfinitePoiseEnabled
        {
            get => _isInfinitePoiseEnabled;
            set
            {
                if (SetProperty(ref _isInfinitePoiseEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    _playerService.ToggleInfinitePoise(_isInfinitePoiseEnabled);
                }
            }
        }

        private bool _isAutoSetNewGameSevenEnabled;

        public bool IsAutoSetNewGameSevenEnabled
        {
            get => _isAutoSetNewGameSevenEnabled;
            set
            {
                if (SetProperty(ref _isAutoSetNewGameSevenEnabled, value))
                {
                    if (!PatchManager.IsInitialized) return;
                    if (_isAutoSetNewGameSevenEnabled) _newGameService.RequestNewGameDetect();
                    else _newGameService.ReleaseNewGameDetect();
                }
            }
        }

        private bool _isDisableSoulMemWriteEnabled;

        public bool IsDisableSoulMemWriteEnabled
        {
            get => _isDisableSoulMemWriteEnabled;
            set => SetProperty(ref _isDisableSoulMemWriteEnabled, value);
        }

        private bool _isStateIncluded;

        public bool IsStateIncluded
        {
            get => _isStateIncluded;
            set => SetProperty(ref _isStateIncluded, value);
        }

        private int _vigor;

        public int Vigor
        {
            get => _vigor;
            set => SetProperty(ref _vigor, value);
        }

        private int _attunement;

        public int Attunement
        {
            get => _attunement;
            set => SetProperty(ref _attunement, value);
        }

        private int _endurance;

        public int Endurance
        {
            get => _endurance;
            set => SetProperty(ref _endurance, value);
        }

        private int _strength;

        public int Strength
        {
            get => _strength;
            set => SetProperty(ref _strength, value);
        }

        private int _dexterity;

        public int Dexterity
        {
            get => _dexterity;
            set => SetProperty(ref _dexterity, value);
        }

        private int _intelligence;

        public int Intelligence
        {
            get => _intelligence;
            set => SetProperty(ref _intelligence, value);
        }

        private int _faith;

        public int Faith
        {
            get => _faith;
            set => SetProperty(ref _faith, value);
        }

        private int _adp;

        public int Adp
        {
            get => _adp;
            set => SetProperty(ref _adp, value);
        }

        private int _vitality;

        public int Vitality
        {
            get => _vitality;
            set => SetProperty(ref _vitality, value);
        }

        private int _soulLevel;

        public int SoulLevel
        {
            get => _soulLevel;
            private set => SetProperty(ref _soulLevel, value);
        }

        private int _soulMemory;

        public int SoulMemory
        {
            get => _soulMemory;
            private set => SetProperty(ref _soulMemory, value);
        }

        private int _souls = 10000;

        public int Souls
        {
            get => _souls;
            set => SetProperty(ref _souls, value);
        }

        private int _newGame;

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

        private float _playerSpeed;

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

        #endregion

        #region Public Methods

        public void PauseUpdates() => _pauseUpdates = true;
        public void ResumeUpdates() => _pauseUpdates = false;

        public void SetHp(int hp)
        {
            _playerService.SetHp(hp);
            CurrentHp = hp;
        }
        
        public void SetStat(string statName, int value)
        {
            var property = typeof(GameManagerImp.ChrCtrlOffsets.Stats)
                .GetProperty(statName, BindingFlags.Public | BindingFlags.Static);

            if (property != null)
            {
                int statOffset = (int)property.GetValue(null);
                if (IsDisableSoulMemWriteEnabled) _playerService.ToggleSoulMemWrite(true);
                _playerService.SetPlayerStat(statOffset, (byte)value);
                if (IsDisableSoulMemWriteEnabled) _playerService.ToggleSoulMemWrite(false);
            }
            else
            {
                Console.WriteLine($@"Invalid stat name: {statName}");
            }
        }

        
        #endregion

        #region Private Methods

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.SavePos1, () => SavePosition(0));
            _hotkeyManager.RegisterAction(HotkeyActions.SavePos2, () => SavePosition(1));
            _hotkeyManager.RegisterAction(HotkeyActions.RestorePos1, () => RestorePosition(0));
            _hotkeyManager.RegisterAction(HotkeyActions.RestorePos2, () => RestorePosition(1));
            _hotkeyManager.RegisterAction(HotkeyActions.RTSR, SetRtsr);
            _hotkeyManager.RegisterAction(HotkeyActions.MaxHp, SetMaxHp);
            _hotkeyManager.RegisterAction(HotkeyActions.PlayerSetCustomHp, SetCustomHp);
            _hotkeyManager.RegisterAction(HotkeyActions.NoDeath, () => { IsNoDeathEnabled = !IsNoDeathEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.OneShot, () => { IsOneShotEnabled = !IsOneShotEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.DealNoDamage,
                () => { IsDealNoDamageEnabled = !IsDealNoDamageEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.PlayerNoDamage,
                () => { IsNoDamageEnabled = !IsNoDamageEnabled; });
            _hotkeyManager.RegisterAction(HotkeyActions.RestoreSpellcasts, () =>
            {
                if (!AreOptionsEnabled) return;
                _playerService.RestoreSpellcasts();
            });
            _hotkeyManager.RegisterAction(HotkeyActions.RestoreHumanity, () =>
            {
                if (!AreOptionsEnabled) return;
                _playerService.SetSpEffect(SpEffect.RestoreHumanity);
            });
            _hotkeyManager.RegisterAction(HotkeyActions.RestCharacter, () =>
            {
                if (!AreOptionsEnabled) return;
                _playerService.SetSpEffect(SpEffect.BonfireRest);
            });
            _hotkeyManager.RegisterAction(HotkeyActions.TogglePlayerSpeed, ToggleSpeed);
            _hotkeyManager.RegisterAction(HotkeyActions.IncreasePlayerSpeed,
                () => SetSpeed(Math.Min(10, PlayerSpeed + 0.25f)));
            _hotkeyManager.RegisterAction(HotkeyActions.DecreasePlayerSpeed,
                () => SetSpeed(Math.Max(0, PlayerSpeed - 0.25f)));
        }

        private void PlayerTick(object sender, EventArgs e)
        {
            if (IsHotEnabled) TryApplyHot();

            if (_pauseUpdates) return;

            CurrentHp = _playerService.GetHp();
            CurrentMaxHp = _playerService.GetMaxHp();
            PlayerSpeed = _playerService.GetPlayerSpeed();
            int newSoulLevel = _playerService.GetSoulLevel();
            SoulMemory = _playerService.GetSoulMemory();
            var coords = _playerService.GetCoords();
            PosX = coords.X;
            PosY = coords.Y;
            PosZ = coords.Z;


            if (_currentSoulLevel == newSoulLevel) return;
            SoulLevel = newSoulLevel;
            _currentSoulLevel = newSoulLevel;
            LoadStats();
        }

        private void TryApplyHot()
        {
            if (CurrentHp >= CurrentMaxHp) return;
            int hpToSet = CurrentHp + 10;
            if (hpToSet >= CurrentMaxHp) hpToSet = CurrentMaxHp;
            _playerService.SetHp(hpToSet);
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

        private void OnGameLoaded()
        {
            if (IsNoDeathEnabled) _playerService.ToggleNoDeath(true);

            AreOptionsEnabled = true;
            LoadStats();
            _playerTick.Start();
        }

        private void OnGameFirstLoaded()
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

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
            _playerTick.Stop();
        }

        private void OnNewGameStarted()
        {
            if (!IsAutoSetNewGameSevenEnabled) return;
            _playerService.SetNewGame(9);
            NewGame = _playerService.GetNewGame();
            // _ezStateService.ExecuteEvent(EzState.EventCommands.OpenCharacterCreationMenu);
        }

        private void OnGameAttached()
        {
            if (IsAutoSetNewGameSevenEnabled) _newGameService.RequestNewGameDetect();
            else _newGameService.ReleaseNewGameDetect();
        }

        private void SavePosition(object parameter)
        {
            int index = Convert.ToInt32(parameter);
            var state = index == 0 ? _saveState1 : _saveState2;
            if (index == 0) IsPos1Saved = true;
            else IsPos2Saved = true;

            state.IncludesState = IsStateIncluded;
            if (IsStateIncluded)
            {
                state.Hp = CurrentHp;
                state.Sp = _playerService.GetSp();
            }

            _playerService.SavePos(index);
        }

        private void RestorePosition(object parameter)
        {
            int index = Convert.ToInt32(parameter);

            if (index == 0 && !IsPos1Saved) return;
            if (index == 1 && !IsPos2Saved) return;
            _playerService.RestorePos(index);
            if (!IsStateIncluded) return;

            var state = index == 0 ? _saveState1 : _saveState2;
            if (IsStateIncluded && state.IncludesState)
            {
                _playerService.SetHp(state.Hp);
                _playerService.SetSp(state.Sp);
            }
        }

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
        
        private void SetRtsr() => _playerService.SetRtsr();
        private void SetMaxHp() => _playerService.SetFullHp();
        private void Die() => _playerService.SetHp(0);

        private void SetCustomHp()
        {
            if (!_customHpHasBeenSet) return;
            var (customHp, error) = ParseCustomHp();
            if (customHp == null)
            {
                MsgBox.Show(error, "Invalid Input");
                return;
            }

            if (customHp > CurrentMaxHp)
                customHp = CurrentMaxHp;

            _playerService.SetHp(customHp.Value);
        }

        private (int? value, string error) ParseCustomHp()
        {
            var input = CustomHp?.Trim();
            if (string.IsNullOrEmpty(input))
                return (null, "Please enter a value");

            if (input.EndsWith("%"))
            {
                if (double.TryParse(input.TrimEnd('%'), out var percent))
                    return ((int)(percent / 100.0 * CurrentMaxHp), null);
                return (null, "Invalid percentage format");
            }

            if (int.TryParse(input, out var absolute))
                return (absolute, null);

            return (null, "Enter a number or percentage (e.g. 545 or 40%)");
        }
        
        private void RestoreSpellcasts() => _playerService.RestoreSpellcasts();

        private void RestoreHumanity() => _playerService.SetSpEffect(SpEffect.RestoreHumanity);

        private void Rest() => _playerService.SetSpEffect(SpEffect.BonfireRest);

        private void GiveSouls() => _playerService.GiveSouls(Souls);
        
        private void SetSpeed(float value) => PlayerSpeed = value;

        #endregion
    }
}