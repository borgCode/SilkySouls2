using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SilkySouls2.enums;
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
        private readonly EzStateService _ezStateService;

        public EnemyViewModel(EnemyService enemyService, HotkeyManager hotkeyManager, EzStateService ezStateService,
            GameStateService gameStateService)
        {
            _enemyService = enemyService;
            _hotkeyManager = hotkeyManager;
            _ezStateService = ezStateService;


            gameStateService.Subscribe(GameState.Loaded, OnGameStateLoaded);
            gameStateService.Subscribe(GameState.NotLoaded, OnGameStateNotLoaded);

            RegisterHotkeys();

            _availableForlornAreas = new ObservableCollection<ForlornArea>(ForlornArea.All);

            if (_availableForlornAreas.Count > 0)
            {
                SelectedForlornArea = _availableForlornAreas[0];
            }
        }

        private void RegisterHotkeys()
        {
            _hotkeyManager.RegisterAction("DisableAi", () => { IsAllDisableAiEnabled = !IsAllDisableAiEnabled; });
        }

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
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

        private bool _isPigSummonsEnabled;

        public bool IsPigSummonsEnabled
        {
            get => _isPigSummonsEnabled;
            set
            {
                if (SetProperty(ref _isPigSummonsEnabled, value))
                {
                    if (_isPigSummonsEnabled)
                    {
                        IsSkellySummonsEnabled = false;
                        IsVelstadtSummonEnabled = false;
                        _enemyService.ToggleElanaSummons(_isPigSummonsEnabled, 0);
                    }
                    else
                    {
                        _enemyService.ToggleElanaSummons(_isPigSummonsEnabled);
                    }
                }
            }
        }

        private bool _isSkellySummonsEnabled;

        public bool IsSkellySummonsEnabled
        {
            get => _isSkellySummonsEnabled;
            set
            {
                if (SetProperty(ref _isSkellySummonsEnabled, value))
                {
                    if (_isSkellySummonsEnabled)
                    {
                        IsPigSummonsEnabled = false;
                        IsVelstadtSummonEnabled = false;
                        _enemyService.ToggleElanaSummons(_isSkellySummonsEnabled, 0x5);
                    }
                    else
                    {
                        _enemyService.ToggleElanaSummons(_isSkellySummonsEnabled);
                    }
                }
            }
        }

        private bool _isVelstadtSummonEnabled;

        public bool IsVelstadtSummonEnabled
        {
            get => _isVelstadtSummonEnabled;
            set
            {
                if (SetProperty(ref _isVelstadtSummonEnabled, value))
                {
                    if (_isVelstadtSummonEnabled)
                    {
                        IsPigSummonsEnabled = false;
                        IsSkellySummonsEnabled = false;
                        _enemyService.ToggleElanaSummons(_isVelstadtSummonEnabled, 0x62);
                    }
                    else
                    {
                        _enemyService.ToggleElanaSummons(_isVelstadtSummonEnabled);
                    }
                }
            }
        }


        private ObservableCollection<ForlornArea> _availableForlornAreas;

        public ObservableCollection<ForlornArea> AvailableForlornAreas
        {
            get => _availableForlornAreas;
            private set => SetProperty(ref _availableForlornAreas, value);
        }

        private ForlornArea _selectedForlornArea;

        public ForlornArea SelectedForlornArea
        {
            get => _selectedForlornArea;
            set
            {
                if (!SetProperty(ref _selectedForlornArea, value)) return;
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
                if (SelectedForlornArea == null || !IsGuaranteedSpawnEnabled) return;
                _enemyService.UpdateForlornIndex(_selectedForlornIndex + 1);
                _enemyService.ToggleForlornSpawn(true,
                    SelectedForlornArea.FunctionId,
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
                if (SelectedForlornArea != null)
                {
                    _enemyService.ToggleForlornSpawn(_isGuaranteedSpawnEnabled,
                        SelectedForlornArea.FunctionId,
                        SelectedForlornIndex + 1);
                }
            }
        }

        public IEnumerable<string> ForlornIndexes
        {
            get
            {
                if (SelectedForlornArea?.Spawns == null)
                    return Enumerable.Empty<string>();
                return SelectedForlornArea.Spawns
                    .Select((spawn, i) => $"{i + 1}: {spawn.LocationName}")
                    .ToArray();
            }
        }

        public void SpawnForlorn()
        {
            var overrideCommand = SelectedForlornArea.Spawns[SelectedForlornIndex].OverridePositionCommand;
            var areaId = SelectedForlornArea.AreaId;
            var areaIndex = SelectedForlornArea.AreaIndex;

            var generateCommand = SelectedForlornArea.Spawns[SelectedForlornIndex].GenerateNpcCommand;

            Task.Run(() =>
            {
                _ezStateService.ExecuteEzStateEventCommand(overrideCommand, areaId, areaIndex);
                _ezStateService.ExecuteEzStateEventCommand(generateCommand, areaId, areaIndex);
            });
        }


        public void TryApplyOneTimeFeatures()
        {
            if (IsAllDisableAiEnabled) _enemyService.ToggleDisableAi(true);
            IsScholar = GameVersion.Current.Edition == GameEdition.Scholar;
        }

        private void OnGameStateLoaded()
        {
            AreOptionsEnabled = true;
            if (IsPigSummonsEnabled) _enemyService.ToggleElanaSummons(IsPigSummonsEnabled, 0);
            if (IsSkellySummonsEnabled) _enemyService.ToggleElanaSummons(_isSkellySummonsEnabled, 0x5);
            if (IsVelstadtSummonEnabled) _enemyService.ToggleElanaSummons(_isVelstadtSummonEnabled, 0x62);
        }


        private void OnGameStateNotLoaded()
        {
            AreOptionsEnabled = false;
        }
    }
}