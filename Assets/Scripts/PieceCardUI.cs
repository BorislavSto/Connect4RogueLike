using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PieceCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private GameObject piecePrefab;
    [HideInInspector] public PieceAndCardManager manager;
    
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image spriteImage;
    
    private Piece matchingPiece;
    private Vector3 originalScale;
    private float originalY;
    private Vector2 originalPosition;
    
    public PieceAndCardData pieceAndCardData { get; private set; }

    private RectTransform rect;
    private bool inPlay;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Setup(PieceAndCardData data)
    {
        titleText.text = data.title;
        descriptionText.text = data.description;
        spriteImage.sprite = data.art;
        pieceAndCardData = data;
    }

    public void SetupPos(Vector2 position)
    {
        originalScale = rect.localScale;
        originalY = position.y;
        originalPosition = position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inPlay)
            return;
        
        // Scale and lift on hover
        rect.DOScale(originalScale * 1.1f, 0.2f);
        rect.DOAnchorPosY(originalY + 30f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inPlay)
            return;
        
        // Reset scale and position
        rect.DOScale(originalScale, 0.2f);
        rect.DOAnchorPosY(originalY, 0.2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inPlay || (GameManager.instance.GetCurrentTurn() != CurrentTurn.Player))
            return;
        
        inPlay = true;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!inPlay)
            return;
        
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rect.parent, eventData.position, eventData.pressEventCamera, out pos);
        rect.anchoredPosition = pos;

        if (ZonesHanlder.instance.IsInPlayZone(eventData))
            ShowPiece();
    }

    // Don't spawn a piece right then and there, one should be made with the card, when moving they should appear disappear etc.
    // Would save on instantiating and so on, only destroy the card if the piece is played
    private void ShowPiece()
    {
        if (!matchingPiece)
            SpawnPiece();
        
        matchingPiece.gameObject.SetActive(true);
        matchingPiece.ShowPiece();
        gameObject.SetActive(false);
    }

    public void PiecePlayed()
    {
        manager.RemoveCard(this.gameObject);
        Destroy(gameObject);
    }
    
    private void SpawnPiece()
    {
        GameObject pieceObj = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity);
        Piece piece = pieceObj.GetComponent<Piece>();
        piece.SetupPiece(pieceAndCardData.owner, this, pieceAndCardData.effect);
        piece.SetPieceEffects(pieceAndCardData.effect);

        matchingPiece = piece;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!inPlay)
            return;

        // Return to original hand position
        rect.DOScale(originalScale, 0.3f);
        rect.DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => { inPlay = false; });
    }
}

[System.Serializable]
public struct PieceAndCardData
{
    public string title;
    public string description;
    public Sprite art;
    public PieceEffects effect;
    public PieceOwner owner;
}