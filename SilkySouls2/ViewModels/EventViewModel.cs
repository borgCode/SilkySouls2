using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using SilkySouls2.Core;
using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private readonly UtilityService _utilityService;
        private readonly IEventService _eventService;
        private readonly IEzStateService _ezStateService;

        public EventViewModel(UtilityService utilityService, IEventService eventService, IEzStateService ezStateService,
            GameStateService gameStateService)
        {
            _utilityService = utilityService;
            _eventService = eventService;
            _ezStateService = ezStateService;
            
            gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
            gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);
            gameStateService.Subscribe(GameState.FirstLoaded, OnGameFirstLoaded);
            gameStateService.Subscribe(GameState.AreaChanged, OnAreaChanged);

            LoadNpcs();

            SetEventCommand = new DelegateCommand(SetEvent);
            GetEventCommand = new DelegateCommand(GetEvent);

            SetAliveCommand = new DelegateCommand(SetNpcAlive);
            SetDeadCommand = new DelegateCommand(SetNpcDead);
            SetFriendlyCommand = new DelegateCommand(SetNpcFriendly);
            SetHostileCommand = new DelegateCommand(SetNpcHostile);
            MoveToMajulaCommand = new DelegateCommand(MoveNpcToMajula);
            UnlockDarklurkerCommand = new DelegateCommand(UnlockDarklurker);
            UnlockNashCommand = new DelegateCommand(UnlockNash);
            UnlockAldiaCommand = new DelegateCommand(UnlockAldia);
            VisibleAavaCommand = new DelegateCommand(VisibleAava);
            BreakIceCommand = new DelegateCommand(BreakIce);
            RescueKnightsCommand = new DelegateCommand(RescueKnights);
            KingsRingCommand = new DelegateCommand(KingsRingAcquired);
            ActivateBrumeCommand = new DelegateCommand(ActivateBrume);
            OpenGargsDoorCommand = new DelegateCommand(OpenGargsDoor);
            LightSinnerFiresCommand = new DelegateCommand(LightSinner);
        }
        

        #region Commands

        public ICommand SetEventCommand { get; set; }
        public ICommand GetEventCommand { get; set; }

        public ICommand SetAliveCommand { get; set; }
        public ICommand SetDeadCommand { get; set; }
        public ICommand SetFriendlyCommand { get; set; }
        public ICommand SetHostileCommand { get; set; }
        public ICommand MoveToMajulaCommand { get; set; }
        public ICommand UnlockDarklurkerCommand { get; set; }
        public ICommand UnlockNashCommand { get; set; }
        public ICommand UnlockAldiaCommand { get; set; }
        public ICommand VisibleAavaCommand { get; set; }
        public ICommand BreakIceCommand { get; set; }
        public ICommand RescueKnightsCommand { get; set; }
        public ICommand KingsRingCommand { get; set; }
        public ICommand ActivateBrumeCommand { get; set; }
        public ICommand OpenGargsDoorCommand { get; set; }
        public ICommand LightSinnerFiresCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private bool _isAreaBastille;

        public bool IsAreaBastille
        {
            get => _isAreaBastille;
            set => SetProperty(ref _isAreaBastille, value);
        }

        private bool _isSnowstormDisabled;

        public bool IsSnowstormDisabled
        {
            get => _isSnowstormDisabled;
            set
            {
                if (SetProperty(ref _isSnowstormDisabled, value))
                {
                    _utilityService.ToggleSnowstormHook(_isSnowstormDisabled);
                    if (AreOptionsEnabled) _eventService.SetEvent(EventFlag.FrigidSnowstorm, false);
                }
            }
        }

        private bool _isMemoryTimerDisabled;

        public bool IsMemoryTimerDisabled
        {
            get => _isMemoryTimerDisabled;
            set
            {
                if (SetProperty(ref _isMemoryTimerDisabled, value))
                {
                    _utilityService.ToggleMemoryTimer(_isMemoryTimerDisabled);
                }
            }
        }

        private ObservableCollection<NpcInfo> _npcList;

        public ObservableCollection<NpcInfo> NpcList
        {
            get => _npcList;
            set => SetProperty(ref _npcList, value);
        }

        private NpcInfo _selectedNpc;

        public NpcInfo SelectedNpc
        {
            get => _selectedNpc;
            set
            {
                SetProperty(ref _selectedNpc, value);

                CanMoveToMajula = value != null &&
                                  (value.MoveToMajulaFlagIds != null &&
                                   value.MoveToMajulaFlagIds.Length > 0 &&
                                   value.MoveToMajulaFlagIds[0] != 0);
            }
        }

        private bool _canMoveToMajula;

        public bool CanMoveToMajula
        {
            get => _canMoveToMajula;
            private set => SetProperty(ref _canMoveToMajula, value);
        }

        private string _setFlagId;

        public string SetFlagId
        {
            get => _setFlagId;
            set => SetProperty(ref _setFlagId, value);
        }

        private int _flagStateIndex;

        public int FlagStateIndex
        {
            get => _flagStateIndex;
            set => SetProperty(ref _flagStateIndex, value);
        }

        private string _getFlagId;

        public string GetFlagId
        {
            get => _getFlagId;
            set => SetProperty(ref _getFlagId, value);
        }

        private string _eventStatusText;

        public string EventStatusText
        {
            get => _eventStatusText;
            set => SetProperty(ref _eventStatusText, value);
        }

        private Brush _eventStatusColor;

        public Brush EventStatusColor
        {
            get => _eventStatusColor;
            set => SetProperty(ref _eventStatusColor, value);
        }

        private bool _isIvorySkipEnabled;

        public bool IsIvorySkipEnabled
        {
            get => _isIvorySkipEnabled;
            set
            {
                if (!SetProperty(ref _isIvorySkipEnabled, value)) return;
                _utilityService.ToggleIvorySkip(_isIvorySkipEnabled);
            }
        }

        #endregion

        #region Private Methods
        
        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
        }
        
        private void OnGameFirstLoaded()
        {
            if (IsSnowstormDisabled)
            {
                _utilityService.ToggleSnowstormHook(true);
                _eventService.SetEvent(EventFlag.FrigidSnowstorm, false);
            }

            if (IsMemoryTimerDisabled) _utilityService.ToggleMemoryTimer(true);
            if (IsIvorySkipEnabled) _utilityService.ToggleIvorySkip(true);
        }

        private void OnAreaChanged(object[] objects)
        {
            IsAreaBastille = (int) objects[0] == Area.Bastille;
        }

        private void SetEvent()
        {
            if (string.IsNullOrWhiteSpace(SetFlagId))
                return;

            string trimmedFlagId = SetFlagId.Trim();

            if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
                return;

            _eventService.SetEvent(flagIdValue, FlagStateIndex == 0);
        }

        private void GetEvent()
        {
            if (string.IsNullOrWhiteSpace(GetFlagId))
                return;

            string trimmedFlagId = GetFlagId.Trim();

            if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
                return;

            if (_eventService.GetEvent(flagIdValue))
            {
                EventStatusText = "True";
                EventStatusColor = Brushes.Chartreuse;
            }
            else
            {
                EventStatusText = "False";
                EventStatusColor = Brushes.Red;
            }
        }

        private void LoadNpcs()
        {
            NpcList = new ObservableCollection<NpcInfo>(
                DataLoader.GetNpcs()
            );

            if (NpcList.Count > 0)
                SelectedNpc = NpcList[0];
        }

        private void SetNpcAlive()
        {
            if (SelectedNpc == null) return;
            _eventService.SetEvent(SelectedNpc.DeathFlagId, false);
        }

        private void SetNpcDead()
        {
            if (SelectedNpc == null) return;
            _eventService.SetEvent(SelectedNpc.DeathFlagId, true);
        }

        private void SetNpcFriendly()
        {
            if (SelectedNpc == null) return;
            _eventService.SetEvent(SelectedNpc.HostileFlagId, false);
        }

        private void SetNpcHostile()
        {
            if (SelectedNpc == null) return;
            _eventService.SetEvent(SelectedNpc.HostileFlagId, true);
        }

        private void MoveNpcToMajula()
        {
            if (SelectedNpc == null || !SelectedNpc.HasMajulaFlags) return;
            foreach (int flagId in SelectedNpc.MoveToMajulaFlagIds)
            {
                _eventService.SetEvent(flagId, true);
            }
        }

        private void UnlockDarklurker() => _eventService.SetMultipleEventOn(EventFlag.DarklurkerDungeonsLit);
        private void UnlockNash() => _eventService.SetEvent(EventFlag.GiantLordDefeated, true);
        private void UnlockAldia() => _eventService.SetMultipleEventOn(EventFlag.UnlockAldia);
        private void VisibleAava() => _eventService.SetEvent(EventFlag.VisibleAava, true);
        private void BreakIce() => _eventService.SetMultipleEventOn(EventFlag.Dlc3Ice);
        private void RescueKnights() => _eventService.SetMultipleEventOn(EventFlag.Dlc3Knights);
        private void KingsRingAcquired() => _eventService.SetEvent(EventFlag.KingsRingAcquired, true);
        private void ActivateBrume() => _eventService.SetMultipleEventOn(EventFlag.Scepter);

        private void OpenGargsDoor()
        {
            _utilityService.SetObjState(Area.Bastille, Obj.GargoylesDoor);
            _utilityService.DisableNavimesh(Area.Bastille, Navimesh.GargoylesDoor);
            _utilityService.DisableWhiteDoor(Area.Bastille, WhiteDoor.GargoylesDoor);
        }

        private void LightSinner()
        {
            _utilityService.SetObjState(Area.Bastille, Obj.SinnerLighting1);
            _utilityService.SetObjState(Area.Bastille, Obj.SinnerLighting2);
            _utilityService.SetObjState(Area.Bastille, Obj.SinnerLighting3);
            _utilityService.SetObjState(Area.Bastille, Obj.SinnerLighting4);
        }

        #endregion
        
    }
}