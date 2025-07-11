using UnityEngine;

public class Piece : MonoBehaviour
{
    [HideInInspector] public PieceCardUI matchingCard;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public bool inPlay;
    public bool isDragging;
    
    public PieceOwner pieceOwner;
    public PieceEffects pieceEffects;
    
    private PieceAndCardManager pieceAndCardManager;
    
    public void SetupPiece(PieceOwner owner, PieceCardUI card, PieceEffects effect)
    {
        pieceOwner = owner;
        isDragging = false;
        inPlay = false;
        matchingCard = card;
        pieceEffects = effect;
        
        if (owner == PieceOwner.Player)
        {
            if (pieceEffects.NormalPiece)
            {
                spriteRenderer.sprite = TextureManager.instance.simplePlayerPiece;
            }
            else
            {
                switch (effect.Effect)
                {
                    case SpecialEffect.DestroyAround:
                        spriteRenderer.sprite = TextureManager.instance.breakCirlcePiece;
                        break;
                    default:
                        spriteRenderer.sprite = TextureManager.instance.breakCirlcePiece;
                        break;
                }
            }
        }

        if (owner == PieceOwner.AI)
        {
            if (pieceEffects.NormalPiece)
                spriteRenderer.sprite = TextureManager.instance.simpleAIPiece;
        }
    }

    private void Update()
    {
        if (!isDragging)
            return;
        
        if (!ZonesHanlder.instance.IsInPlayZone())
            ShowCard();
    }

    public void ShowPiece()
    {
        StartDragging();
    }

    private void ShowCard()
    {
        isDragging = false;
        matchingCard.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    
    private void StartDragging()
    {
        isDragging = true;
        
        PieceMouseControls mouseControls = gameObject.GetComponent<PieceMouseControls>();
        mouseControls.StartDragging();
    }

    public void SetPieceInPlay()
    {
        inPlay = true;
        if (matchingCard == null)
            return;
        
        matchingCard.gameObject.SetActive(true);
        matchingCard.PiecePlayed();
    }
}
