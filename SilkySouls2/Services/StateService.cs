using System;
using System.Collections.Generic;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class StateService(IMemoryService memoryService) : IStateService
    {
        private readonly Dictionary<State, List<Action>> _eventHandlers = new();
        private readonly Dictionary<State, List<Action<object[]>>> _eventHandlersWithArgs = new();

        public bool IsGameLoaded()
        {
            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                return memoryService.Read<nint>(
                    memoryService.Read<nint>(GameManagerImp.Base) + GameManagerImp.PlayerCtrl) != IntPtr.Zero;
            }

            return (nint)memoryService.Read<int>(
                memoryService.Read<int>(GameManagerImp.Base) + GameManagerImp.PlayerCtrl) != IntPtr.Zero;
        }

        public bool IsLoadingScreen()
        {
            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                return memoryService.Read<byte>(
                    memoryService.Read<nint>(GameManagerImp.Base) + GameManagerImp.LoadingFlag) == 1;
            }

            return memoryService.Read<byte>(
                memoryService.Read<int>(GameManagerImp.Base) + GameManagerImp.LoadingFlag) == 1;
        }
        
        public void Publish(State eventType)
        {
            if (_eventHandlers.ContainsKey(eventType))
            {
                foreach (var handler in _eventHandlers[eventType])
                    handler.Invoke();
            }
        }
        
        public void Subscribe(State eventType, Action handler)
        {
            if (!_eventHandlers.ContainsKey(eventType))
                _eventHandlers[eventType] = new List<Action>();
   
            _eventHandlers[eventType].Add(handler);
        }
        
        public void Unsubscribe(State eventType, Action handler)
        {
            if (_eventHandlers.ContainsKey(eventType))
                _eventHandlers[eventType].Remove(handler);
        }
        
        public void Publish(State eventType, params object[] args)
        {
            if (_eventHandlersWithArgs.ContainsKey(eventType))
            {
                foreach (var handler in _eventHandlersWithArgs[eventType])
                    handler.Invoke(args);
            }
        }
        
        public void Subscribe(State eventType, Action<object[]> handler)
        {
            if (!_eventHandlersWithArgs.ContainsKey(eventType))
                _eventHandlersWithArgs[eventType] = new List<Action<object[]>>();
   
            _eventHandlersWithArgs[eventType].Add(handler);
        }

        
    }
}