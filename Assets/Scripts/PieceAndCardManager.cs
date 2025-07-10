using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PieceAndCardManager : MonoBehaviour
{
    [SerializeField] private RectTransform handArea;
    [SerializeField] private DeckSystem deckSystem;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<GameObject> handCards = new();

    public int startingHandSize = 4;
    public int handLimit = 5;

    private void Start()
    {
        deckSystem.InitializeDeck();
        StartCoroutine(InitialDraw());
        DrawCard();
    }

    private void DrawCard()
    {
        DrawCardToHand();
    }
    
    private IEnumerator InitialDraw()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCardToHand();
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void DrawCardToHand()
    {
        if (handCards.Count >= handLimit) return;

        PieceAndCardData data;  
        
        bool deckIsEmpty = deckSystem.DrawCard(out data);
        if (deckIsEmpty) return;

        GameObject cardObj = Instantiate(cardPrefab, handArea);
        PieceCardUI cardUI = cardObj.GetComponent<PieceCardUI>();
        cardUI.manager = this;
        cardUI.Setup(data);

        handCards.Add(cardObj);
        LayoutHand();
    }

    private void LayoutHand()
    {
        float spacing = 100f;
        float startX = -((handCards.Count - 1) * spacing) / 2f;

        for (int i = 0; i < handCards.Count; i++)
        {
            RectTransform rt = handCards[i].GetComponent<RectTransform>();
            Vector2 target = new Vector2(startX + i * spacing, 0f);
            rt.DOAnchorPos(target, 0.4f).SetEase(Ease.OutQuad);
            handCards[i].gameObject.name = i.ToString();
            handCards[i].GetComponent<PieceCardUI>().SetupPos(target);
        }
    }
    
    public void RemoveCard(GameObject cardGO)
    {
        PieceAndCardData data = cardGO.GetComponent<PieceCardUI>().pieceAndCardData;
        deckSystem.DiscardCard(data);
        handCards.Remove(cardGO);
        Destroy(cardGO);
        LayoutHand();
    }
}