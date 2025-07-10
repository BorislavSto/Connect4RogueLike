using System;
using JetBrains.Annotations;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [HideInInspector]
    public PieceCardUI matchingCard;
    
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
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
                spriteRenderer.sprite = TextureManager.instance.breakCirlcePiece;
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

    public void SetPieceEffects(PieceEffects effects)
    {
        
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
        matchingCard.gameObject.SetActive(true);
        matchingCard.PiecePlayed();
    }
}

// Will contain all the information to for special effects such as destroying other pieces or sideways movement
[Serializable]
public struct PieceEffects
{
    [SerializeField]
    public bool NormalPiece;
}

public enum PieceOwner
{
    Player,
    AI,
}