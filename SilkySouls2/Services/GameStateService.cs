using System;
using System.Collections.Generic;
using SilkySouls2.enums;

namespace SilkySouls2.Services
{
    public class GameStateService
    {
        private readonly Dictionary<GameState, List<Action>> _eventHandlers = new Dictionary<GameState, List<Action>>();
        
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

        public void Unsubscribe(GameState eventType, Action handler)
        {
            if (_eventHandlers.ContainsKey(eventType))
                _eventHandlers[eventType].Remove(handler);
        }
    }
}