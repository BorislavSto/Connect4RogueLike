using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set; }

    [SerializeField] private Vector2 boardSize;
    
    private bool animationPlaying;
    private CurrentTurn currentTurn;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        { 
            instance = this;
        }
    }

    private void Start()
    {
        SetFirstTurn(CurrentTurn.Player);
        BoardManager.instance.StartBoard(boardSize);
    }

    public void SwitchTurn()
    {
        StartCoroutine(SwitchTurnCo());
    }

    private IEnumerator SwitchTurnCo()
    {
        while (animationPlaying)
            yield return null;

        
        currentTurn = currentTurn == CurrentTurn.Player ? CurrentTurn.AI : CurrentTurn.Player;
        EventManager.InvokeTurnSwitch(currentTurn);
    }
    
    public void GameOver(PieceOwner owner)
    {
        EventManager.InvokeGameOver(currentTurn);
    }
    
    private void SetFirstTurn(CurrentTurn firstTurn)
    {
        currentTurn = firstTurn;
        EventManager.InvokeTurnSwitch(currentTurn);
    }
    
    public CurrentTurn GetCurrentTurn() => currentTurn;
}

public enum CurrentTurn
{
    Player,
    AI,
}
