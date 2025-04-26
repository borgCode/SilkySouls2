﻿using System.Collections.Generic;
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
        private ObservableCollection<WarpLocation> _areaLocations;
        
        private string _selectedMainArea;
        private WarpLocation _selectedWarpLocation;
        
        private Dictionary<string, List<WarpLocation>> _locationDict;
        private List<WarpLocation> _allLocations;
        
        private string _searchText = string.Empty;
        private bool _isSearchActive;
        private string _preSearchMainArea;
        private readonly ObservableCollection<WarpLocation> _searchResultsCollection = new ObservableCollection<WarpLocation>();
        
        
        
        public TravelViewModel(TravelService travelService, HotkeyManager hotkeyManager)
        {
            _travelService = travelService;
            _hotkeyManager = hotkeyManager;
            
            _mainAreas = new ObservableCollection<string>();
            _areaLocations = new ObservableCollection<WarpLocation>();

            LoadLocations();
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
        
        public ObservableCollection<string> MainAreas
        {
            get => _mainAreas;
            private set => SetProperty(ref _mainAreas, value);
        }
        
        public ObservableCollection<WarpLocation> AreaLocations
        {
            get => _areaLocations;
            set => SetProperty(ref _areaLocations, value);
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
                
                UpdateLocationsList();
            }
        }
        
        public WarpLocation SelectedWarpLocation
        {
            get => _selectedWarpLocation;
            set => SetProperty(ref _selectedWarpLocation, value);
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

        public void Warp() => _travelService.Warp(SelectedWarpLocation);
    }
}