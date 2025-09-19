using Assets.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    
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

            float duration = .25f;

            LeanTween.move(cards[i].gameObject, splinePos, duration).setEase(LeanTweenType.easeInOutSine);

            LeanTween.rotate(cards[i].gameObject, rotation.eulerAngles, duration).setEase(LeanTweenType.easeInOutSine);

            cards[i].UpdatePosition(splinePos, rotation);
        }
    }

    public void AddCard(Card newCard)
    {
        cards.Add(newCard);
        newCard.spriteRenderer.sortingOrder = cards.Count;
        newCard.transform.SetParent(transform, true);
        newCard.position = CardPosition.Hand;
        UpdateCardPositions();
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
}
