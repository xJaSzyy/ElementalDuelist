using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new();

    private int maxCards = 2;

    public void AddCard(Card card)
    {
        if (cards.Count == maxCards)
        {
            Debug.Log($"Max {maxCards} cards on table!");
            return;
        }

        card.gameObject.transform.SetParent(transform);
        card.position = CardPosition.Table;

        cards.Add(card);

        CardDeckManager.Instance.TryDetermineWinner();

        if (IsFull())
        {
            CardDeckManager.Instance.MoveTurn();
        }
    }

    public List<Card> GetCards()
    {
        return cards;
    }

    public void RemoveCards()
    {
        cards.Clear();
    }

    public bool IsFull()
    {
        return cards.Count == maxCards;
    }

    public int GetCardsPower()
    {
        int power = 0;
        foreach (var item in cards)
        {
            power += item.value;
        }

        return power;
    }
}
