using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MainTextObject;
    [SerializeField] private TextMeshProUGUI CardsTextObject;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float visibleDuration = 1f;
    
    private Tween currentTween;
    
    private void OnEnable()
    {
        EventManager.TurnSwitch += OnTurnSwitch;
        EventManager.GameOver += OnGameOver;
        EventManager.UpdateCards += OnUpdateCards;
    }
    
    private void OnDisable()
    {
        EventManager.TurnSwitch -= OnTurnSwitch;
        EventManager.GameOver -= OnGameOver;
        EventManager.UpdateCards -= OnUpdateCards;
    }

    private void OnUpdateCards(int amount)
    {
        CardsTextObject.text = $"Cards in Deck: \n {amount}";
    }

    private void OnTurnSwitch(CurrentTurn currentTurn)
    {
        string message = currentTurn.ToString();

        switch (currentTurn)
        {
            case CurrentTurn.Player:
                message = "Your turn";
                break;
            case CurrentTurn.AI:
                message = "Enemy's turn";
                break;
            default:
                break;
        }
        
        ShowMessage(message);
    }

    private void OnGameOver(CurrentTurn winningTurn)
    {
        string message = winningTurn.ToString();

        switch (winningTurn)
        {
            case CurrentTurn.Player:
                message = "You WIN!";
                break;
            case CurrentTurn.AI:
                message = "Enemy wins";
                break;
            default:
                break;
        }
        
        ShowMessage(message);
    }
    
    private void ShowMessage(string message)
    {
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();

        MainTextObject.text = message;
        MainTextObject.alpha = 0f;

        // Fade In, wait, then fade out
        currentTween = MainTextObject.DOFade(1f, fadeDuration)
            .OnComplete(() =>
            {
                currentTween = DOVirtual.DelayedCall(visibleDuration, () =>
                {
                    currentTween = MainTextObject.DOFade(0f, fadeDuration);
                });
            });
    }
}