using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolitareStockManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject stock;
    
    [SerializeField] private List<SolitareCard> stockCards = new();
    [SerializeField] private SolitareCardSlot waste;

    public void AddCard(SolitareCard card)
    {
        stockCards.Add(card);
        card.transform.SetParent(stock.transform, false);
        card.transform.localPosition = Vector3.zero;
        card.Flip();
    }

    public void RemoveCard(SolitareCard card)
    {
        card.Flip();
        stockCards.Remove(card);
    }

    public SolitareCard GetRandomCard()
    {
        System.Random rnd = new();

        var card = stockCards[rnd.Next(0, stockCards.Count)];
        stockCards.Remove(card);

        return card;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (stockCards.Count == 0)
        {
            var cards = waste.RemoveAndGetAllCards();
            cards.Reverse();
            foreach (var item in cards)
            {
                AddCard(item);
            }
        }
        else
        {
            var cardOnTop = stockCards[stockCards.Count - 1];
            cardOnTop.transform.SetParent(transform.parent);
            LeanTween.move(cardOnTop.gameObject, waste.transform, .3f);
            RemoveCard(cardOnTop);
            waste.AddCard(cardOnTop);
        }
    }
}
