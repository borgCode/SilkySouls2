using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SilkySouls2.Core;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class TravelViewModel : BaseViewModel
    {
        private readonly ITravelService _travelService;
        private readonly HotkeyManager _hotkeyManager;

        private Dictionary<string, List<WarpLocation>> _locationDict;
        private List<WarpLocation> _allLocations;

        private string _preSearchMainArea;
        private readonly ObservableCollection<WarpLocation> _searchResultsCollection = new();

        public TravelViewModel(ITravelService travelService, HotkeyManager hotkeyManager,
            GameStateService gameStateService)
        {
            _travelService = travelService;
            _hotkeyManager = hotkeyManager;

            _mainAreas = new ObservableCollection<string>();
            _areaLocations = new ObservableCollection<WarpLocation>();

            gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
            gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);

            WarpCommand = new DelegateCommand(Warp);
            UnlockBonfiresCommand = new DelegateCommand(UnlockAllBonfires);

            LoadLocations();
            RegisterHotkeys();
        }

        #region Commands

        public ICommand WarpCommand { get; set; }
        public ICommand UnlockBonfiresCommand { get; set; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private ObservableCollection<string> _mainAreas;

        public ObservableCollection<string> MainAreas
        {
            get => _mainAreas;
            private set => SetProperty(ref _mainAreas, value);
        }

        private ObservableCollection<WarpLocation> _areaLocations;

        public ObservableCollection<WarpLocation> AreaLocations
        {
            get => _areaLocations;
            set => SetProperty(ref _areaLocations, value);
        }

        private string _selectedMainArea;

        public string SelectedMainArea
        {
            get => _selectedMainArea;
            set
            {
                if (!SetProperty(ref _selectedMainArea, value) || value == null) return;

                if (_isSearchActive)
                {
                    IsSearchActive = false;
                    _searchText = string.Empty;
                    OnPropertyChanged(nameof(SearchText));
                    _preSearchMainArea = null;
                }

                UpdateLocationsList();
            }
        }

        private WarpLocation _selectedWarpLocation;

        public WarpLocation SelectedWarpLocation
        {
            get => _selectedWarpLocation;
            set => SetProperty(ref _selectedWarpLocation, value);
        }

        private bool _isSearchActive;

        public bool IsSearchActive
        {
            get => _isSearchActive;
            private set => SetProperty(ref _isSearchActive, value);
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (!SetProperty(ref _searchText, value)) return;

                if (string.IsNullOrEmpty(value))
                {
                    _isSearchActive = false;

                    if (_preSearchMainArea != null)
                    {
                        _selectedMainArea = _preSearchMainArea;
                        UpdateLocationsList();
                        _preSearchMainArea = null;
                    }
                }
                else
                {
                    if (!_isSearchActive)
                    {
                        _preSearchMainArea = SelectedMainArea;
                        _isSearchActive = true;
                    }

                    ApplyFilter();
                }
            }
        }

        private bool _isRestOnWarpEnabled;

        public bool IsRestOnWarpEnabled
        {
            get => _isRestOnWarpEnabled;
            set => SetProperty(ref _isRestOnWarpEnabled, value);
        }

        #endregion

        #region Private Methods

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction(HotkeyActions.Warp, () =>
            {
                if (!AreOptionsEnabled) return;
                Task.Run(() => _travelService.Warp(SelectedWarpLocation, IsRestOnWarpEnabled));
            });
        }

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
        }

        private void LoadLocations()
        {
            _locationDict = DataLoader.GetLocations();

            _allLocations = _locationDict.Values.SelectMany(x => x).ToList();

            foreach (var area in _locationDict.Keys)
            {
                _mainAreas.Add(area);
            }

            SelectedMainArea = _mainAreas.FirstOrDefault();
        }

        private void UpdateLocationsList()
        {
            if (string.IsNullOrEmpty(SelectedMainArea) || !_locationDict.ContainsKey(SelectedMainArea))
            {
                AreaLocations = new ObservableCollection<WarpLocation>();
                return;
            }

            AreaLocations = new ObservableCollection<WarpLocation>(_locationDict[SelectedMainArea]);
            SelectedWarpLocation = AreaLocations.FirstOrDefault();
        }

        private void ApplyFilter()
        {
            _searchResultsCollection.Clear();
            var searchTextLower = SearchText.ToLower();

            foreach (var location in _allLocations)
            {
                if (location.LocationName.ToLower().Contains(searchTextLower) ||
                    location.MainArea.ToLower().Contains(searchTextLower))
                {
                    _searchResultsCollection.Add(location);
                }
            }

            AreaLocations = new ObservableCollection<WarpLocation>(_searchResultsCollection);
            SelectedWarpLocation = AreaLocations.FirstOrDefault();
        }

        private void Warp() => Task.Run(() => _travelService.Warp(SelectedWarpLocation, IsRestOnWarpEnabled));

        private void UnlockAllBonfires()
        {
            var result = MsgBox.ShowOkCancel(
                "Are you sure you want to unlock all bonfires?",
                "Unlock Bonfires");

            if (result)
            {
                _travelService.UnlockAllBonfires();
            }
        }

        #endregion
    }
}