using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    public CardSide side;
    
    private List<Card> cards = new();

    private void UpdateCardPositions()
    {
        if (cards.Count == 0)
        {
            return;
        }

        float spacing = 1f / cards.Count;
        float firstCardPos = .5f - (cards.Count - 1) * spacing / 2;

        Spline spline = splineContainer.Spline;

        for (int i = 0; i < cards.Count; i++)
        {
            float p = firstCardPos + i * spacing;
            Vector3 splinePos = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            LeanTween.move(cards[i].gameObject, splinePos, .25f).setEase(LeanTweenType.easeInOutSine);
            LeanTween.rotate(cards[i].gameObject, rotation.eulerAngles, .25f).setEase(LeanTweenType.easeInOutSine);

            cards[i].SetSortingOrder(i);
            cards[i].UpdatePosition(splinePos, rotation);
        }
    }

    public void AddCard(Card newCard)
    {
        cards.Add(newCard);
        newCard.transform.SetParent(transform, true);
        newCard.position = CardPosition.Hand;
        newCard.side = side;
        newCard.stopRaised = true;
        LeanTween.scale(newCard.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                newCard.stopRaised = false;
                UpdateCardPositions();
            });
    }

    public void RemoveCard(Card removeCard)
    {
        removeCard.position = CardPosition.Table;
        cards.Remove(removeCard);
        UpdateCardPositions();
    }

    public int GetCardsCount()
    {
        return cards.Count;
    }

    public bool IsEmpty()
    {
        return cards.Count == 0;
    }

    public bool CanBeat(Card card)
    {
        foreach (Card item in cards)
        {
            if (item.element.Beats(card.element))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanCombined(Card card)
    {
        foreach (Card item in cards)
        {
            if (item.element.Combined(card.element))
            {
                return true;
            }
        }

        return false;
    }
}
