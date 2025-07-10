using System;

public static class EventManager
{
    public static event Action<CurrentTurn> TurnSwitch;

    public static void InvokeTurnSwitch(CurrentTurn currentTurn)
    {
        TurnSwitch?.Invoke(currentTurn);
    }
}