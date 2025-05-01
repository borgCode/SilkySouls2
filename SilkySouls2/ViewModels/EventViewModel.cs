using SilkySouls2.Memory;
using SilkySouls2.Services;

namespace SilkySouls2.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private readonly UtilityService _utilityService;
        public EventViewModel(UtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        public void UnlockDarklurker() => _utilityService.SetMultipleEventOn(GameIds.EventFlags.DarklurkerDungeonsLit);
    }
}