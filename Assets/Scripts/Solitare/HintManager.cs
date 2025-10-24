using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private bool inProccess = false;
    [SerializeField] private float animationSpeed =.3f;

    [Header("References")]
    [SerializeField] private SolitareGameManager gameManager;
    [SerializeField] private SolitareStockManager stockManager;
    [SerializeField] private SolitareCardSlot[] tableauSlots;
    [SerializeField] private SolitareCardSlot[] foundationSlots;
    [SerializeField] private SolitareCardSlot waste;

    private void Update()
    {
        if (inProccess || !gameManager.ready)
        {
            return;
        }

        if (gameManager.IsAllCardsOpen() || Input.GetKeyDown(KeyCode.F))
        {
            inProccess = true;
            StartCoroutine(AutoWin());
        }
    }

    private IEnumerator AutoWin()
    {
        stockManager.animationSpeed = animationSpeed;
        var waitForAnimation = new WaitForSeconds(animationSpeed);

        while (!gameManager.win)
        {
            bool movedCard = false;

            foreach (var foundationSlot in foundationSlots)
            {
                if (foundationSlot.full)
                {
                    continue;
                }

                var foundationTopCard = foundationSlot.GetTopCard();

                foreach (var tableauSlot in tableauSlots)
                {
                    var tableauTopCard = tableauSlot.GetTopCard();
                    if (tableauTopCard == null)
                    {
                        continue;
                    }

                    if (IsValidFoundationMove(foundationTopCard, tableauTopCard))
                    {
                        yield return StartCoroutine(MoveCardToFoundation(tableauTopCard, foundationSlot));
                        movedCard = true;
                        break;
                    }
                }

                if (movedCard)
                {
                    break;
                }

                var wasteTopCard = waste.GetTopCard();
                if (wasteTopCard != null && IsValidFoundationMove(foundationTopCard, wasteTopCard))
                {
                    yield return StartCoroutine(MoveCardToFoundation(wasteTopCard, foundationSlot));
                    movedCard = true;
                    break;
                }
            }

            if (!movedCard)
            {
                stockManager.PointerClick();
                yield return waitForAnimation;
            }
        }

        SlotsDisable();
        yield return StartCoroutine(PlayAnimation());

        inProccess = false;
    }

    private bool IsValidFoundationMove(SolitareCard foundationCard, SolitareCard movingCard)
    {
        if (foundationCard == null)
        {
            return movingCard.Rank == Rank.Ace;
        }

        return foundationCard.Suit == movingCard.Suit &&
               foundationCard.Rank == movingCard.Rank - 1;
    }

    private IEnumerator MoveCardToFoundation(SolitareCard card, SolitareCardSlot foundationSlot)
    {
        card.animationSpeed = animationSpeed;
        card.transform.SetParent(foundationSlot.gameObject.transform);

        bool animationCompleted = false;

        LeanTween.move(card.gameObject, foundationSlot.gameObject.transform, animationSpeed)
            .setOnComplete(() =>
            {
                card.PlaceInSlot(foundationSlot, false);
                animationCompleted = true;
            });

        yield return new WaitUntil(() => animationCompleted);
    }

    private void SlotsDisable()
    {
        foreach (var item in tableauSlots)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in foundationSlots)
        {
            item.GetComponent<Image>().enabled = false;
        }

        waste.gameObject.SetActive(false);
        stockManager.gameObject.SetActive(false);
    }

    public IEnumerator PlayAnimation()
    {
        float radius = 2f;
        Vector3 center = Vector3.zero;

        List<SolitareCard> allCards = new();
        foreach (var slot in foundationSlots)
        {
            allCards.AddRange(slot.GetCards());
        }

        for (int i = 0; i < allCards.Count; i++)
        {
            if (i % 2 == 1)
            {
                allCards[i].gameObject.SetActive(false);
                continue;
            }

            float angle = i * (360f / allCards.Count);
            Vector3 targetPos = center + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0);
            LeanTween.move(allCards[i].gameObject, targetPos, 1f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.rotateAround(allCards[i].gameObject, Vector3.forward, 360f, 1f).setRepeat(1).setEaseLinear();
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}
