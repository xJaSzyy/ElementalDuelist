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

    public SolitareCard GetTopCard()
    {
        if (stockCards.Count <= 0)
        {
            return null;
        }

        var card = stockCards[stockCards.Count - 1];
        stockCards.Remove(card);

        return card;
    }

    public void ShuffleCards()
    {
        System.Random rnd = new();
        int n = stockCards.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            SolitareCard value = stockCards[k];
            stockCards[k] = stockCards[n];
            stockCards[n] = value;
        }

        for (int i = 0; i < stockCards.Count; i++)
        {
            stockCards[i].transform.SetSiblingIndex(i);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClick();
    }

    public void PointerClick()
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
            cardOnTop.transform.SetParent(waste.transform);
            LeanTween.move(cardOnTop.gameObject, waste.transform, .3f);
            RemoveCard(cardOnTop);
            waste.AddCard(cardOnTop);
        }
    }

    public int GetStockCardsCount()
    {
        return stockCards.Count;
    }
}
