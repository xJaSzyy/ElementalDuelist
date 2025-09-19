using System;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    [SerializeField] private Vector3 tableCardsScale = new(0.5f, 0.5f, 0.5f);
    [SerializeField] private Transform mainCardPos;
    [SerializeField] private Transform extraCardPos;

    [SerializeField] private Card mainCard;
    [SerializeField] private Card extraCard;

    public bool SetMainCard(Card card)
    {
        if (mainCard != null)
        {
            return false;
        }

        card.transform.SetParent(transform, true);
        LeanTween.move(card.gameObject, mainCardPos.position, .25f);
        LeanTween.rotate(card.gameObject, mainCardPos.rotation.eulerAngles, .25f);
        LeanTween.scale(card.gameObject, tableCardsScale, 0.25f);
        card.SetSortingOrder(0);
        mainCard = card;

        return true;
    }

    public bool SetExtraCard(Card card)
    {
        if (extraCard != null)
        {
            return false;
        }

        card.transform.SetParent(transform, true);
        LeanTween.move(card.gameObject, extraCardPos.position, .25f);
        LeanTween.rotate(card.gameObject, extraCardPos.rotation.eulerAngles, .25f);
        LeanTween.scale(card.gameObject, tableCardsScale, 0.25f);
        card.SetSortingOrder(1);
        extraCard = card;

        return true;
    }

    public bool IsFull()
    {
        return (mainCard != null) && (extraCard != null);
    }

    public Card GetMainCard()
    {
        return mainCard;
    }
    
    public Card GetExtraCard()
    {
        return extraCard;
    }
}
