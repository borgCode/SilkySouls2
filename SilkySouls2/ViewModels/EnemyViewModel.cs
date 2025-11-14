using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Services;
using SilkySouls2.Utilities;

namespace SilkySouls2.ViewModels
{
    public class EnemyViewModel : BaseViewModel
    {
        private readonly EnemyService _enemyService;
        private readonly HotkeyManager _hotkeyManager;

        public EnemyViewModel(EnemyService enemyService, HotkeyManager hotkeyManager)
        {
            _enemyService = enemyService;
            _hotkeyManager = hotkeyManager;

            RegisterHotkeys();

            _availableForlorns = new ObservableCollection<Forlorn>(Forlorn.All);

            if (_availableForlorns.Count > 0)
            {
                SelectedForlorn = _availableForlorns[0];
            }
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction("DisableAi", () => { IsAllDisableAiEnabled = !IsAllDisableAiEnabled; });
        }

        private bool _isAllDisableAiEnabled;

        public bool IsAllDisableAiEnabled
        {
            get => _isAllDisableAiEnabled;
            set
            {
                if (SetProperty(ref _isAllDisableAiEnabled, value))
                {
                    _enemyService.ToggleDisableAi(_isAllDisableAiEnabled);
                }
            }
        }

        private ObservableCollection<Forlorn> _availableForlorns;

        public ObservableCollection<Forlorn> AvailableForlorns
        {
            get => _availableForlorns;
            private set => SetProperty(ref _availableForlorns, value);
        }

        private Forlorn _selectedForlorn;

        public Forlorn SelectedForlorn
        {
            get => _selectedForlorn;
            set
            {
                if (!SetProperty(ref _selectedForlorn, value)) return;
                CurrentAreaName = value?.AreaName ?? "No Forlorn selected";
                IsForlornAvailable = value != null;
                OnPropertyChanged(nameof(ForlornIndexes));

                Application.Current.Dispatcher.BeginInvoke(new Action(() => { SelectedForlornIndex = 0; }));
            }
        }

        private bool _isScholar;

        public bool IsScholar
        {
            get => _isScholar;
            private set => SetProperty(ref _isScholar, value);
        }


        private bool _isForlornAvailable;

        public bool IsForlornAvailable
        {
            get => _isForlornAvailable;
            private set => SetProperty(ref _isForlornAvailable, value);
        }


        private string _currentAreaName = "No Forlorn in this area";

        public string CurrentAreaName
        {
            get => _currentAreaName;
            private set => SetProperty(ref _currentAreaName, value);
        }

        private int _selectedForlornIndex;

        public int SelectedForlornIndex
        {
            get => _selectedForlornIndex;
            set
            {
                if (!SetProperty(ref _selectedForlornIndex, value)) return;
                if (SelectedForlorn == null || !IsGuaranteedSpawnEnabled) return;
                _enemyService.UpdateForlornIndex(_selectedForlornIndex + 1);
                _enemyService.ToggleForlornSpawn(true,
                    SelectedForlorn.EsdFuncId,
                    _selectedForlornIndex + 1);
            }
        }

        private bool _isGuaranteedSpawnEnabled;

        public bool IsGuaranteedSpawnEnabled
        {
            get => _isGuaranteedSpawnEnabled;
            set
            {
                if (!SetProperty(ref _isGuaranteedSpawnEnabled, value)) return;
                if (SelectedForlorn != null)
                {
                    _enemyService.ToggleForlornSpawn(_isGuaranteedSpawnEnabled,
                        SelectedForlorn.EsdFuncId,
                        SelectedForlornIndex + 1);
                }
            }
        }

        public IEnumerable<string> ForlornIndexes
        {
            get
            {
                if (SelectedForlorn?.SpawnNames == null)
                    return Enumerable.Empty<string>();
                return SelectedForlorn.SpawnNames
                    .Select((name, i) => $"{i + 1}: {name}")
                    .ToArray();
            }
        }
        
        public void TryApplyOneTimeFeatures()
        {
            if (IsAllDisableAiEnabled) _enemyService.ToggleDisableAi(true);
            IsScholar = GameVersion.Current.Edition == GameEdition.Scholar;
        }

    }
}