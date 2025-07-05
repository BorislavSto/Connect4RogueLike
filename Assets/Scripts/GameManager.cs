using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set; }

    public Vector2 boardSize;
    private CurrentTurn currentTurn;
    private bool animationPlaying;

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

    void Update()
    {
        
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
        
        Debug.Log(currentTurn);
    }

    private void SetFirstTurn(CurrentTurn firstTurn)
    {
        currentTurn = firstTurn;
    }
    
    public CurrentTurn GetCurrentTurn() => currentTurn;
}

public enum CurrentTurn
{
    Player,
    AI,
}
