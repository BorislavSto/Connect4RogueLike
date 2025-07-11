using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    private void SetFirstTurn(CurrentTurn firstTurn)
    {
        currentTurn = firstTurn;
        EventManager.InvokeTurnSwitch(currentTurn);
    }
    
    public void SwitchTurn()
    {
        StartCoroutine(SwitchTurnCo());
    }
    
    public void GameOver(PieceOwner owner)
    {
        EventManager.InvokeGameOver(currentTurn);
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("BattleScene");
    }
    
    private IEnumerator SwitchTurnCo()
    {
        while (animationPlaying)
            yield return null;

        
        currentTurn = currentTurn == CurrentTurn.Player ? CurrentTurn.AI : CurrentTurn.Player;
        EventManager.InvokeTurnSwitch(currentTurn);
    }
    
    public CurrentTurn GetCurrentTurn() => currentTurn;
}
