using System;

public static class EventManager
{
    public static event Action<CurrentTurn> TurnSwitch;
    public static event Action<CurrentTurn> GameOver;
    public static event Action<int> UpdateCards;

    public static void InvokeTurnSwitch(CurrentTurn currentTurn)
    {
        TurnSwitch?.Invoke(currentTurn);
    }    

    public static void InvokeGameOver(CurrentTurn winningTurn)
    {
        GameOver?.Invoke(winningTurn);
    }
    
    public static void InvokeUpdateCards(int amount)
    {
        UpdateCards?.Invoke(amount);
    }
}