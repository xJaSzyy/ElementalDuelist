using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SolitareGameManager : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private GameObject cardPrefab;
    
    [Header("References")]
    [SerializeField] private GameObject table;
    [SerializeField] private GameObject deck;
    [SerializeField] private SolitareDeckManager deckManager;
    [SerializeField] private SolitareCardSlot[] foundationSlots;

    [SerializeField] private List<SolitareCard> tableCards = new();

    private void Start()
    {
        DrawCards();
        SetCards();
    }

    private void Update()
    {
        foreach (var item in foundationSlots)
        {
            if (!item.full)
            {
                return;
            }
        }

        Debug.Log("Win");
    }

    private void DrawCards()
    {

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            for (int rankValue = 1; rankValue <= 13; rankValue++) 
            {
                GameObject cardObject = Instantiate(cardPrefab);

                SolitareCard card = cardObject.GetComponent<SolitareCard>();
                card.Suit = suit;
                card.Rank = (Rank)rankValue;
                card.UpdateVisual();

                deckManager.AddCard(card);
            }
        }
    }

    private void SetCards()
    {
        for (int columnIndex = 0; columnIndex < table.transform.childCount; columnIndex++)
        {
            var column = table.transform.GetChild(columnIndex);

            for (int j = 0; j < columnIndex + 1; j++)
            {
                var card = deckManager.GetRandomCard();
                card.transform.SetParent(column.transform, false);
                column.GetComponent<SolitareCardSlot>().AddCard(card);
                deckManager.RemoveCard(card);
                tableCards.Add(card);

                if (j + 1 < columnIndex + 1)
                {
                    card.Flip();
                }
            }
        }
    }
}
