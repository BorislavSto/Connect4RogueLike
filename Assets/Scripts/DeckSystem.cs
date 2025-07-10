using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
    public List<PieceAndCardData> fullDeck = new List<PieceAndCardData>();
    public Queue<PieceAndCardData> drawPile = new Queue<PieceAndCardData>();
    
    [HideInInspector]
    public List<PieceAndCardData> discardPile = new List<PieceAndCardData>();

    public void InitializeDeck()
    {
        // Fill the drawPile with a shuffled copy
        drawPile = new Queue<PieceAndCardData>(fullDeck.OrderBy(x => Random.value));
    }

    public bool DrawCard(out PieceAndCardData pieceAndCard)
    {
        if (drawPile.Count == 0)
        {
            pieceAndCard = new PieceAndCardData();
            return true;
        }
        
        pieceAndCard = drawPile.Dequeue();
        return false;
    }

    public void ReshuffleDiscardIntoDrawPile()
    {
        drawPile = new Queue<PieceAndCardData>(discardPile.OrderBy(x => Random.value));
        discardPile.Clear();
    }

    public void DiscardCard(PieceAndCardData pieceAndCard)
    {
        discardPile.Add(pieceAndCard);
    }
}