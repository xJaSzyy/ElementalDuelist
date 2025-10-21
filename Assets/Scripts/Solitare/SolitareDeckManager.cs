using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolitareDeckManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject deck;
    
    [SerializeField] private List<SolitareCard> deckCards = new();
    [SerializeField] private SolitareCardSlot slot;

    public void AddCard(SolitareCard card)
    {
        deckCards.Add(card);
        card.transform.SetParent(deck.transform, false);
        card.transform.localPosition = Vector3.zero;
        card.Flip();
    }

    public void RemoveCard(SolitareCard card)
    {
        card.Flip();
        deckCards.Remove(card);
    }

    public SolitareCard GetRandomCard()
    {
        System.Random rnd = new System.Random();

        var card = deckCards[rnd.Next(0, deckCards.Count)];
        deckCards.Remove(card);

        return card;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (deckCards.Count == 0)
        {
            var cards = slot.RemoveAndGetAllCards();
            foreach (var item in cards)
            {
                AddCard(item);
            }
        }
        else
        {
            var cardOnTop = deckCards[deckCards.Count - 1];
            cardOnTop.transform.SetParent(slot.transform, false);
            cardOnTop.transform.localPosition = Vector3.zero;
            RemoveCard(cardOnTop);
            slot.AddCard(cardOnTop);
        }
    }
}
