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
            set => SetProperty(ref _selectedNpc, value);
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
            if (SelectedNpc == null || SelectedNpc.MoveToMajulaFlagId == 0) return;
            _utilityService.SetEventOn(SelectedNpc.MoveToMajulaFlagId);
        }
        
        public void UnlockDarklurker() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.DarklurkerDungeonsLit);
        public void UnlockNash() => _utilityService.SetEventOn(GameIds.EventFlags.GiantLordDefeated);
        public void UnlockAldia() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.UnlockAldia);
        public void VisibleAava() => _utilityService.SetEventOn(GameIds.EventFlags.VisibleAava);
        public void BreakIce() => _utilityService.SetEventOn(GameIds.EventFlags.Dlc3Ice);
        public void RescueKnights() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.Dlc3Knights);
        public void KingsRingAcquired() => _utilityService.SetEventOn(GameIds.EventFlags.KingsRingAcquired);
    }
}