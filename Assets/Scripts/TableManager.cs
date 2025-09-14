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
    }
}
