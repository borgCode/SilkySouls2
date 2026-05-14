// 

using System;
using System.Collections.Generic;
using System.Windows.Threading;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;

namespace SilkySouls2.Services;

public class GameTickService : IGameTickService
{
    private readonly DispatcherTimer _timer;
    private readonly List<Action> _subscribers = new();
    private readonly List<Action> _tickBuffer = new();
    
    public GameTickService(IStateService stateService)
    {
        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(64) };
        _timer.Tick += OnTick;
    }

    
    public void Subscribe(Action callback)
    {
        if (!_subscribers.Contains(callback))
            _subscribers.Add(callback);
    }
    public void Unsubscribe(Action callback) => _subscribers.Remove(callback);
    

    private void OnGameLoaded() => _timer.Start();
    
    private void OnGameNotLoaded() => _timer.Stop();

    private void OnTick(object sender, EventArgs e)
    {
        _tickBuffer.Clear();
        _tickBuffer.AddRange(_subscribers);
        foreach (var subscriber in _tickBuffer)
            subscriber();
    }
}