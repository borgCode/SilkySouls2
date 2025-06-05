using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class ItemViewModel : BaseViewModel
    {
        private readonly ItemService _itemService;
        
        private ILookup<string, Item> _allItems;
        private readonly Dictionary<string, ObservableCollection<Item>> _itemsByCategory =
            new Dictionary<string, ObservableCollection<Item>>();
        private readonly ObservableCollection<Item> _searchResultsCollection = new ObservableCollection<Item>();
        
        public string[] _infusionNames =
            { "Normal", "Fire", "Magic", "Lightning", "Dark", "Poison", "Bleed", "Raw", "Enchanted", "Mundane" };

        public Dictionary<int, int[]> _infusionDict;
        
        
        public ItemViewModel(ItemService itemService)
        {
            _itemService = itemService;
            LoadData();
        }

        private void LoadData()
        {
            Categories.Add("Ammo");
            Categories.Add("Armor");
            Categories.Add("Consumables");
            Categories.Add("Gestures");
            Categories.Add("Key Items");
            Categories.Add("Rings");
            Categories.Add("Spells");
            Categories.Add("Upgrade Materials");
            Categories.Add("Weapons");
            
            _itemsByCategory.Add("Ammo", new ObservableCollection<Item>(DataLoader.GetItemList("Ammo")));
            _itemsByCategory.Add("Armor", new ObservableCollection<Item>(DataLoader.GetItemList("Armor")));
            _itemsByCategory.Add("Consumables", new ObservableCollection<Item>(DataLoader.GetItemList("Consumables")));
            _itemsByCategory.Add("Gestures", new ObservableCollection<Item>(DataLoader.GetItemList("Gestures")));
            _itemsByCategory.Add("Key Items", new ObservableCollection<Item>(DataLoader.GetItemList("KeyItems")));
            _itemsByCategory.Add("Rings", new ObservableCollection<Item>(DataLoader.GetItemList("Rings")));
            _itemsByCategory.Add("Spells", new ObservableCollection<Item>(DataLoader.GetItemList("Spells")));
            _itemsByCategory.Add("Upgrade Materials",
                new ObservableCollection<Item>(DataLoader.GetItemList("UpgradeMaterials")));
            _itemsByCategory.Add("Weapons", new ObservableCollection<Item>(DataLoader.GetItemList("Weapons")));
            
            _allItems = _itemsByCategory.Values.SelectMany(x => x).ToLookup(i => i.Name);

            _infusionDict = DataLoader.GetInfusions();


            // _loadoutTemplatesByName = LoadoutTemplates.All.ToDictionary(lt => lt.Name);
            //
            // LoadCustomLoadouts();
            //
            // _loadouts = new ObservableCollection<string>(_loadoutTemplatesByName.Keys);
            //
            // SelectedLoadoutName = Loadouts.FirstOrDefault();
            SelectedCategory = Categories.FirstOrDefault();
            SelectedMassSpawnCategory = Categories.FirstOrDefault();
            // SelectedAutoSpawnWeapon = WeaponList.FirstOrDefault();
        }
        
        private bool _areOptionsEnabled;
        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }
        
        private ObservableCollection<string> _categories = new ObservableCollection<string>();
        public ObservableCollection<string> Categories
        {
            get => _categories;
            private set => SetProperty(ref _categories, value);
        }
        
        private ObservableCollection<Item> _items = new ObservableCollection<Item>();
        public ObservableCollection<Item> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
        
        private ObservableCollection<string> _availableInfusions = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableInfusions
        {
            get => _availableInfusions;
            private set => SetProperty(ref _availableInfusions, value);
        }
        
        private bool _isSearchActive;
        public bool IsSearchActive
        {
            get => _isSearchActive;
            private set => SetProperty(ref _isSearchActive, value);
        }
        
        private string _preSearchCategory;
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (!SetProperty(ref _searchText, value))
                {
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    _isSearchActive = false;

                    if (_preSearchCategory != null)
                    {
                        _selectedCategory = _preSearchCategory;
                        Items = _itemsByCategory[_selectedCategory];
                        SelectedItem = Items.FirstOrDefault();
                        _preSearchCategory = null;
                    }
                }
                else
                {
                    if (!_isSearchActive)
                    {
                        _preSearchCategory = SelectedCategory;
                        _isSearchActive = true;
                    }

                    ApplyFilter();
                }
            }
        }
        
        private void ApplyFilter()
        {
            _searchResultsCollection.Clear();
            var searchTextLower = SearchText.ToLower();

            foreach (var category in _itemsByCategory)
            {
                foreach (var item in category.Value)
                {
                    if (item.Name.ToLower().Contains(searchTextLower))
                    {
                        item.CategoryName = category.Key;
                        _searchResultsCollection.Add(item);
                    }
                }
            }

            Items = _searchResultsCollection;
        }
        

        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if (_selectedItem == null) return;
                
                QuantityEnabled = _selectedItem.StackSize > 1;
                MaxQuantity = _selectedItem.StackSize;
                SelectedQuantity = _selectedItem.StackSize;
                AvailableInfusions.Clear();
                int[] infusionFlags = _infusionDict[_selectedItem.InfuseId];

                for (int i = 0; i < infusionFlags.Length; i++)
                {
                    if (infusionFlags[i] == 1)
                    {
                        AvailableInfusions.Add(_infusionNames[i]);
                    }
                }
                
                CanInfuse = AvailableInfusions.Count > 1;
                if (!CanInfuse) SelectedInfusionType = "Normal";
                
                CanUpgrade = _selectedItem.MaxUpgrade > 0;
                if (!CanUpgrade) SelectedUpgrade = 0;
                else MaxUpgradeLevel = _selectedItem.MaxUpgrade;
                
                if (SelectedUpgrade > MaxUpgradeLevel) SelectedUpgrade = MaxUpgradeLevel;
            }
        }
        
        private int _selectedQuantity = 1;
        public int SelectedQuantity
        {
            get => _selectedQuantity;
            set
            {
                int clampedValue = Math.Max(1, Math.Min(value, MaxQuantity));
                SetProperty(ref _selectedQuantity, clampedValue);
            }
        }
        
        private int _maxQuantity;
        public int MaxQuantity
        {
            get => _maxQuantity;
            private set => SetProperty(ref _maxQuantity, value);
        }
        
        private bool _canUpgrade;
        public bool CanUpgrade
        {
            get => _canUpgrade;
            private set => SetProperty(ref _canUpgrade, value);
        }
        
        private int _maxUpgradeLevel = 10;
        public int MaxUpgradeLevel
        {
            get => _maxUpgradeLevel;
            private set => SetProperty(ref _maxUpgradeLevel, value);
        }
        
        private int _selectedUpgrade;
        public int SelectedUpgrade
        {
            get => _selectedUpgrade;
            set => SetProperty(ref _selectedUpgrade, Math.Max(0, Math.Min(value, MaxUpgradeLevel)));
        }

        
        private bool _quantityEnabled;
        public bool QuantityEnabled
        {
            get => _quantityEnabled;
            private set => SetProperty(ref _quantityEnabled, value);
        }
        
        private bool _canInfuse;
        public bool CanInfuse
        {
            get => _canInfuse;
            private set => SetProperty(ref _canInfuse, value);
        }
        
        private string _selectedInfusionType = "Normal";
        public string SelectedInfusionType
        {
            get => _selectedInfusionType;
            set => SetProperty(ref _selectedInfusionType, value);
        }
        
        
        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (!SetProperty(ref _selectedCategory, value) || value == null) return;
                if (_selectedCategory == null) return;

                if (_isSearchActive)
                {
                    IsSearchActive = false;
                    _searchText = string.Empty;
                    OnPropertyChanged(nameof(SearchText));
                    _preSearchCategory = null;
                }

                Items = _itemsByCategory[_selectedCategory];
                SelectedItem = Items.FirstOrDefault();
            }
        }
        
        private string _selectedMassSpawnCategory;
        public string SelectedMassSpawnCategory
        {
            get => _selectedMassSpawnCategory;
            set => SetProperty(ref _selectedMassSpawnCategory, value);
        }

        // public ObservableCollection<string> Loadouts
        // {
        //     get => _loadouts;
        //     private set => SetProperty(ref _loadouts, value);
        // }
        //
        // public string SelectedLoadoutName
        // {
        //     get => _selectedLoadoutName;
        //     set => SetProperty(ref _selectedLoadoutName, value);
        // }

        public void SpawnItem()
        {
            if (_selectedItem == null) return;

            _itemService.SpawnItem(_selectedItem, SelectedUpgrade, SelectedQuantity, Array.IndexOf(_infusionNames, SelectedInfusionType));
        }

        public void TryEnableFeatures()
        {
            AreOptionsEnabled = true;
        }

        public void DisableFeatures()
        {
            AreOptionsEnabled = false;
        }

        public void MassSpawn()
        {
            if (SelectedMassSpawnCategory == "Weapons")
            {
                foreach (var weapon in _itemsByCategory[SelectedMassSpawnCategory])
                {
                    _itemService.SpawnItem(weapon, 0, 1, 0);
                    _itemService.SpawnItem(weapon, weapon.MaxUpgrade, 1, 0);
                }
            }
            else
            {
                foreach (var item in _itemsByCategory[SelectedMassSpawnCategory])
                {
                    _itemService.SpawnItem(item, 0, item.StackSize, 0);
                    
                }
            }
        }
    }
}