using System;
using System.Collections.Generic;
using SilkySouls2.enums;

namespace SilkySouls2.Services
{
    public class GameStateService
    {
        private readonly Dictionary<GameState, List<Action>> _eventHandlers = new();
        private readonly Dictionary<GameState, List<Action<object[]>>> _eventHandlersWithArgs = new();
        
        public void Publish(GameState eventType)
        {
            if (_eventHandlers.ContainsKey(eventType))
            {
                foreach (var handler in _eventHandlers[eventType])
                    handler.Invoke();
            }
        }
        
        public void Subscribe(GameState eventType, Action handler)
        {
            if (!_eventHandlers.ContainsKey(eventType))
                _eventHandlers[eventType] = new List<Action>();
   
            _eventHandlers[eventType].Add(handler);
        }
        
        public void Publish(GameState eventType, params object[] args)
        {
            if (_eventHandlersWithArgs.ContainsKey(eventType))
            {
                foreach (var handler in _eventHandlersWithArgs[eventType])
                    handler.Invoke(args);
            }
        }
        
        public void Subscribe(GameState eventType, Action<object[]> handler)
        {
            if (!_eventHandlersWithArgs.ContainsKey(eventType))
                _eventHandlersWithArgs[eventType] = new List<Action<object[]>>();
   
            _eventHandlersWithArgs[eventType].Add(handler);
        }

        
    }
}