using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class TravelViewModel : BaseViewModel
    {
        private readonly TravelService _travelService;
        private readonly HotkeyManager _hotkeyManager;
        
        
        private ObservableCollection<string> _mainAreas;
        private ObservableCollection<Bonfire> _areaBonfires;
        
        private string _selectedMainArea;
        private Bonfire _selectedBonfire;
        
        private Dictionary<string, List<Bonfire>> _bonfireDict;
        private List<Bonfire> _allBonfires;
        
        private string _searchText = string.Empty;
        private bool _isSearchActive;
        private string _preSearchMainArea;
        private readonly ObservableCollection<Bonfire> _searchResultsCollection = new ObservableCollection<Bonfire>();
        
        
        
        public TravelViewModel(TravelService travelService, HotkeyManager hotkeyManager)
        {
            _travelService = travelService;
            _hotkeyManager = hotkeyManager;
            
            _mainAreas = new ObservableCollection<string>();
            _areaBonfires = new ObservableCollection<Bonfire>();

            LoadBonfires();
        }

        private void LoadBonfires()
        {
            _bonfireDict = DataLoader.GetBonfires();
            
            _allBonfires = _bonfireDict.Values.SelectMany(x => x).ToList();
            
            foreach (var area in _bonfireDict.Keys)
            {
                _mainAreas.Add(area);
            }
            
            SelectedMainArea = _mainAreas.FirstOrDefault();
        }
        
        public ObservableCollection<string> MainAreas
        {
            get => _mainAreas;
            private set => SetProperty(ref _mainAreas, value);
        }
        
        public ObservableCollection<Bonfire> AreaBonfires
        {
            get => _areaBonfires;
            set => SetProperty(ref _areaBonfires, value);
        }
        
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
                
                UpdateBonfiresList();
            }
        }
        
        public Bonfire SelectedBonfire
        {
            get => _selectedBonfire;
            set => SetProperty(ref _selectedBonfire, value);
        }
        
        public bool IsSearchActive
        {
            get => _isSearchActive;
            private set => SetProperty(ref _isSearchActive, value);
        }
        
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
                        UpdateBonfiresList();
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
        
        private void UpdateBonfiresList()
        {
            if (string.IsNullOrEmpty(SelectedMainArea) || !_bonfireDict.ContainsKey(SelectedMainArea))
            {
                AreaBonfires = new ObservableCollection<Bonfire>();
                return;
            }
            
            AreaBonfires = new ObservableCollection<Bonfire>(_bonfireDict[SelectedMainArea]);
            SelectedBonfire = AreaBonfires.FirstOrDefault();
        }
        
        private void ApplyFilter()
        {
            _searchResultsCollection.Clear();
            var searchTextLower = SearchText.ToLower();
            
            foreach (var bonfire in _allBonfires)
            {
                if (bonfire.BonfireName.ToLower().Contains(searchTextLower) || 
                    bonfire.MainArea.ToLower().Contains(searchTextLower))
                {
                    _searchResultsCollection.Add(bonfire);
                }
            }
            
            AreaBonfires = new ObservableCollection<Bonfire>(_searchResultsCollection);
            SelectedBonfire = AreaBonfires.FirstOrDefault();
        }

        public void BonfireWarp() => _travelService.BonfireWarp(SelectedBonfire.BonfireId);
    }
}