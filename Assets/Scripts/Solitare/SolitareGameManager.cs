using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolitareGameManager : MonoBehaviour
{
    [Header("Stats")]
    public bool win = false;
    public bool ready = false;

    [Header("Options")]
    [SerializeField] private GameObject cardPrefab;
    
    [Header("References")]
    [SerializeField] private GameObject tableau;
    [SerializeField] private GameObject stock;
    [SerializeField] private SolitareStockManager stockManager;
    [SerializeField] private SolitareCardSlot[] foundationSlots;

    [SerializeField] private List<SolitareCard> tableauCards = new();

    private void Start()
    {
        DrawCards();
        //stockManager.ShuffleCards();
        SetCards();

        ready = true;
    }

    private void Update()
    {
        if (win)
        {
            return;
        }

        foreach (var item in foundationSlots)
        {
            if (!item.full)
            {
                return;
            }
        }

        win = true;
    }

    private void DrawCards()
    {

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            for (int rankValue = 1; rankValue <= 13; rankValue++) 
            {
                GameObject cardObject = Instantiate(cardPrefab);

                cardObject.name = $"{suit}_{(Rank)rankValue}";

                SolitareCard card = cardObject.GetComponent<SolitareCard>();
                card.Suit = suit;
                card.Rank = (Rank)rankValue;
                card.UpdateVisual();

                stockManager.AddCard(card);
            }
        }
    }

    private void SetCards()
    {
        for (int columnIndex = 0; columnIndex < tableau.transform.childCount; columnIndex++)
        {
            var column = tableau.transform.GetChild(columnIndex);

            for (int j = 0; j < columnIndex + 1; j++)
            {
                var card = stockManager.GetTopCard();
                card.transform.SetParent(column.transform, false);
                column.GetComponent<SolitareCardSlot>().AddCard(card);
                stockManager.RemoveCard(card);
                tableauCards.Add(card);

                if (j + 1 < columnIndex + 1)
                {
                    card.Flip();
                }
            }
        }
    }

    public bool IsAllCardsOpen()
    {
        foreach (var tableauCard in tableauCards)
        {
            if (tableauCard.Hide)
            {
                return false;
            }
        }

        return true;
    }
}
