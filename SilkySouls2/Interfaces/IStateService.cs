// 

using System;
using SilkySouls2.enums;

namespace SilkySouls2.Interfaces;

public interface IStateService
{
    void Publish(State eventType);
    void Subscribe(State eventType, Action handler);
    void Unsubscribe(State eventType, Action handler);
    bool IsGameLoaded();
    bool IsLoadingScreen();
}