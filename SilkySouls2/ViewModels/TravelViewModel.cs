using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SilkySouls2.Core;
using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;
using SilkySouls2.Views.Windows;

namespace SilkySouls2.ViewModels
{
    public class TravelViewModel : BaseViewModel
    {
        private readonly ITravelService _travelService;
        private readonly IPlayerService _playerService;
        private readonly HotkeyManager _hotkeyManager;
        private readonly ISpEffectService _spEffectService;

        private CreateCustomWarpWindow _customWarpWindow;

        public SearchableGroupedCollection<string, WarpEntry> Areas { get; }
        public SearchableGroupedCollection<string, WarpEntry> Bosses { get; }
        public SearchableGroupedCollection<string, WarpEntry> CustomWarps { get; }

        public TravelViewModel(ITravelService travelService, IPlayerService playerService,
            HotkeyManager hotkeyManager, StateService stateService, ISpEffectService spEffectService)
        {
            _travelService = travelService;
            _playerService = playerService;
            _hotkeyManager = hotkeyManager;
            _spEffectService = spEffectService;

            stateService.Subscribe(State.Loaded, () => AreOptionsEnabled = true);
            stateService.Subscribe(State.NotLoaded, () => AreOptionsEnabled = false);

            var shipping = DataLoader.GetWarpEntries();
            
            var bossEntries = shipping.Where(e =>
                e.Kind is WarpKind.Direct or WarpKind.DirectWithOffset || e.Name == "Darklurker");
            var areaEntries = shipping.Where(e =>
                e.Kind is WarpKind.Bonfire or WarpKind.EventPoint && e.Name != "Darklurker");

            Areas = new SearchableGroupedCollection<string, WarpEntry>(
                DataLoader.GroupByArea(areaEntries),
                MatchesSearch);

            Bosses = new SearchableGroupedCollection<string, WarpEntry>(
                DataLoader.GroupByArea(bossEntries),
                MatchesSearch);

            CustomWarps = new SearchableGroupedCollection<string, WarpEntry>(
                DataLoader.GroupByArea(DataLoader.LoadCustomWarps()),
                MatchesSearch);

            AreaWarpCommand = new DelegateCommand(() => WarpFrom(Areas));
            BossWarpCommand = new DelegateCommand(() => WarpFrom(Bosses));
            CustomWarpCommand = new DelegateCommand(() => WarpFrom(CustomWarps));
            UnlockBonfiresCommand = new DelegateCommand(UnlockAllBonfires);
            OpenCreateCustomWarpCommand = new DelegateCommand(OpenCreateCustomWarp);

            RegisterHotkeys();
        }

        public ICommand AreaWarpCommand { get; }
        public ICommand BossWarpCommand { get; }
        public ICommand CustomWarpCommand { get; }
        public ICommand UnlockBonfiresCommand { get; }
        public ICommand OpenCreateCustomWarpCommand { get; }

        private bool _areOptionsEnabled;
        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private bool _isRestOnWarpEnabled;
        public bool IsRestOnWarpEnabled
        {
            get => _isRestOnWarpEnabled;
            set => SetProperty(ref _isRestOnWarpEnabled, value);
        }

        private static bool MatchesSearch(WarpEntry e, string s) =>
            e.Name.ToLower().Contains(s) || e.Area.ToLower().Contains(s);

        private void WarpFrom(SearchableGroupedCollection<string, WarpEntry> source)
        {
            if (!AreOptionsEnabled || source.SelectedItem == null) return;
            var entry = source.SelectedItem;
            if (IsRestOnWarpEnabled) _spEffectService.ApplySpEffect(_playerService.GetPlayerCtrl(), SpEffect.BonfireRest);
            _travelService.Warp(entry);
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.Warp, () =>
            {
                if (!AreOptionsEnabled) return;
                var entry = Areas.SelectedItem ?? Bosses.SelectedItem ?? CustomWarps.SelectedItem;
                if (entry == null) return;
                if (IsRestOnWarpEnabled) _spEffectService.ApplySpEffect(_playerService.GetPlayerCtrl(), SpEffect.BonfireRest);
                _travelService.Warp(entry);
            });
        }

        private void UnlockAllBonfires()
        {
            var result = MsgBox.ShowOkCancel(
                "Are you sure you want to unlock all bonfires?",
                "Unlock Bonfires");
            if (result) _travelService.UnlockAllBonfires();
        }

        private void OpenCreateCustomWarp()
        {
            if (_customWarpWindow != null && _customWarpWindow.IsVisible)
            {
                _customWarpWindow.Activate();
                return;
            }
            
            var cloned = CustomWarps.GroupedItems
                .ToDictionary(kvp => kvp.Key, kvp => new List<WarpEntry>(kvp.Value));

            _customWarpWindow = new CreateCustomWarpWindow(
                cloned,
                AreOptionsEnabled,
                _playerService,
                OnCustomWarpChanged);

            _customWarpWindow.Closed += (_, _) => _customWarpWindow = null;
            _customWarpWindow.Show();
        }

        private void OnCustomWarpChanged(CustomWarpChange change)
        {
            switch (change)
            {
                case CustomWarpAdded added:
                    CustomWarps.Add(added.Warp.Area, added.Warp);
                    break;
                case CustomWarpDeleted deleted:
                    CustomWarps.Remove(deleted.Category, deleted.Warp);
                    break;
                case CustomCategoryDeleted catDel:
                    if (CustomWarps.GroupedItems.TryGetValue(catDel.Category, out var list))
                    {
                        foreach (var item in list.ToList())
                            CustomWarps.Remove(catDel.Category, item);
                    }
                    break;
            }

            DataLoader.SaveCustomWarps(CustomWarps.AllItems);
        }
    }
}
