using Godot;
using System;

public interface IHoldable{
    Player Player {get; set;}
    void ReactToActionPress(float delta);
    void ReactToActionHold(float delta);
    void PickupPreAction();
    void PostDepletionAction(float delta);
    int NumActionCallsToDepleted { get;}
    int CurrentNumActionCalls {get; set;}
    int NumActionCallsLeft { get;}
    bool IsDepleted{get;set;}
    bool IsBeingHeld{get;set;}
    Texture UIDisplayIcon{get;}
}