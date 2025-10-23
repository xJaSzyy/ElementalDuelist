using Assets.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolitareCardSlot : MonoBehaviour, IDropHandler
{
    [Header("Options")]
    [SerializeField] private SolitareSlotType slotType;

    public List<SolitareCard> cardsInSlot = new();
    public bool full = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (full)
        {
            return;
        }

        SolitareCard card = eventData.pointerDrag.GetComponent<SolitareCard>();
        if (card != null && CanAcceptCard(card))
        {
            card.WasDroppedInSlot = true;
            card.PlaceInSlot(this);
        }
    }

    public virtual bool CanAcceptCard(SolitareCard card)
    {
        if (card == null)
        {
            return false;
        }

        if (slotType == SolitareSlotType.Stock)
        {
            return false;
        }

        if (slotType == SolitareSlotType.Foundation)
        {
            return CanAcceptCardToFoundation(card);
        }

        if (slotType == SolitareSlotType.Tableau)
        {
            return CanAcceptCardToTableau(card);
        }

        return true; 
    }

    private bool CanAcceptCardToFoundation(SolitareCard card)
    {
        if (cardsInSlot.Count == 0)
        {
            return card.Rank == Rank.Ace;
        }
        else
        {
            SolitareCard topCard = cardsInSlot[cardsInSlot.Count - 1];
            return card.Suit == topCard.Suit &&
                   (int)card.Rank == (int)topCard.Rank + 1;
        }
    }

    private bool CanAcceptCardToTableau(SolitareCard card)
    {
        if (cardsInSlot.Count == 0)
        {
            return card.Rank == Rank.King;
        }
        else
        {
            SolitareCard topCard = cardsInSlot[cardsInSlot.Count - 1];
            return IsOppositeColor(card, topCard) &&
                   (int)card.Rank == (int)topCard.Rank - 1;
        }
    }

    private bool IsOppositeColor(SolitareCard card1, SolitareCard card2)
    {
        bool card1IsBlack = card1.Suit == Suit.Clubs || card1.Suit == Suit.Spades;
        bool card2IsBlack = card2.Suit == Suit.Clubs || card2.Suit == Suit.Spades;

        return card1IsBlack != card2IsBlack;
    }

    public void AddCard(SolitareCard card)
    {
        if (!cardsInSlot.Contains(card))
        {
            cardsInSlot.Add(card);
            card.SetCurrentSlot(this);
        }

        if (slotType == SolitareSlotType.Foundation && cardsInSlot.Count == 13)
        {
            full = true;
        }
    }

    public void RemoveCard(SolitareCard card)
    {
        cardsInSlot.Remove(card);
    }

    public void FlipLastCard()
    {
        if (cardsInSlot.Count == 0) { return; }

        if (cardsInSlot[cardsInSlot.Count - 1].Hide)
        {
            cardsInSlot[cardsInSlot.Count - 1].Flip();
        }
    }

    public List<SolitareCard> RemoveAndGetAllCards()
    {
        var cards = new List<SolitareCard>(cardsInSlot);

        foreach (var item in cards)
        {
            RemoveCard(item);
        }

        return cards;
    }

    public SolitareCard GetTopCard()
    {
        if (cardsInSlot.Count <= 0)
        {
            return null;
        }

        var card = cardsInSlot[cardsInSlot.Count - 1];
        return card;
    }
}

public enum SolitareSlotType
{
    Stock,
    Tableau,
    Foundation
}