using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new();

    public void AddCard(Card card)
    {
        card.gameObject.transform.SetParent(transform);
        card.position = CardPosition.Hand;

        cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }

    public int GetCardsCount()
    {
        return cards.Count;
    }
}
