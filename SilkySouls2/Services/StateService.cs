using System;
using System.Collections.Generic;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class StateService(IMemoryService memoryService) : IStateService
    {
        private readonly Dictionary<State, List<Action>> _eventHandlers = new();
        private readonly Dictionary<State, List<Action<object[]>>> _eventHandlersWithArgs = new();
        private const int InGameStep = 30;

        public bool IsGameLoaded() =>
            memoryService.Read<int>(memoryService.ReadPointer(GameManagerImp.Base) + GameManagerImp.Step) == InGameStep;
        

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