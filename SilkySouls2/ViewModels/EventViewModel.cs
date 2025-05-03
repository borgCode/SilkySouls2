using System.Collections.ObjectModel;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Services;

namespace SilkySouls2.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private readonly UtilityService _utilityService;
        
        private NpcInfo _selectedNpc;
        private ObservableCollection<NpcInfo> _npcList;
        private bool _canMoveToMajula;
        private string _flagId;
        private int _flagStateIndex;
        public EventViewModel(UtilityService utilityService)
        {
            _utilityService = utilityService;
            LoadNpcs();
        }

        private void LoadNpcs()
        {
            NpcList = new ObservableCollection<NpcInfo>(
                Utilities.DataLoader.GetNpcs()
            );
            
            if (NpcList.Count > 0)
                SelectedNpc = NpcList[0];
        }

        public ObservableCollection<NpcInfo> NpcList
        {
            get => _npcList;
            set => SetProperty(ref _npcList, value);
        }
        
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
        
        public bool CanMoveToMajula
        {
            get => _canMoveToMajula;
            private set => SetProperty(ref _canMoveToMajula, value);
        }
        public void SetNpcAlive()
        {
            if (SelectedNpc == null) return;
            _utilityService.SetEventOff(SelectedNpc.DeathFlagId);
        }

        public void SetNpcDead()
        {
            if (SelectedNpc == null) return;
            _utilityService.SetEventOn(SelectedNpc.DeathFlagId);
        }

        public void SetNpcFriendly()
        {
            if (SelectedNpc == null) return;
            _utilityService.SetEventOff(SelectedNpc.HostileFlagId);
        }

        public void SetNpcHostile()
        {
            if (SelectedNpc == null) return;
            _utilityService.SetEventOn(SelectedNpc.HostileFlagId);
        }

        public void MoveNpcToMajula()
        {
            if (SelectedNpc == null || !SelectedNpc.HasMajulaFlags) return;
            foreach (int flagId in SelectedNpc.MoveToMajulaFlagIds)
            {
                _utilityService.SetEventOn(flagId);
            }
        }
        
        public void UnlockDarklurker() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.DarklurkerDungeonsLit);
        public void UnlockNash() => _utilityService.SetEventOn(GameIds.EventFlags.GiantLordDefeated);
        public void UnlockAldia() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.UnlockAldia);
        public void VisibleAava() => _utilityService.SetEventOn(GameIds.EventFlags.VisibleAava);
        public void BreakIce() => _utilityService.SetEventOn(GameIds.EventFlags.Dlc3Ice);
        public void RescueKnights() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.Dlc3Knights);
        public void KingsRingAcquired() => _utilityService.SetEventOn(GameIds.EventFlags.KingsRingAcquired);
        public void ActivateBrume() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.Scepter);


        public string FlagId
        {
            get => _flagId;
            set => SetProperty(ref _flagId, value);
        }

        public int FlagStateIndex
        {
            get => _flagStateIndex;
            set => SetProperty(ref _flagStateIndex, value);
        }

        public void SetFlag()
        {
            if (string.IsNullOrWhiteSpace(FlagId))
                return;
            
            string trimmedFlagId = FlagId.Trim();
        
            if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
                return;
            
            if (FlagStateIndex == 0) _utilityService.SetEventOn(flagIdValue);
            else _utilityService.SetEventOff(flagIdValue);
        }
    }
}