using System;
using UnityEngine;

public class TableManager : MonoBehaviour
{
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
        card.transform.position = mainCardPos.position;
        card.transform.rotation = mainCardPos.rotation;
        card.spriteRenderer.sortingOrder = 0;
        card.transform.localScale = new Vector3(.5f, .5f, .5f);
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
        card.transform.position = extraCardPos.position;
        card.transform.rotation = extraCardPos.rotation;
        card.spriteRenderer.sortingOrder = 1;
        card.transform.localScale = new Vector3(.5f, .5f, .5f);
        extraCard = card;

        return true;
    }

    public bool IsFull()
    {
        return (mainCard != null) && (extraCard != null);
    }
}
