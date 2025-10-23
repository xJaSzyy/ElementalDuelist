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

                    if (foundationTopCard == null)
                    {
                        if (tableauTopCard != null && tableauTopCard.Rank == Rank.Ace)
                        {
                            tableauTopCard.animationSpeed = animationSpeed;
                            tableauTopCard.transform.SetParent(foundationSlot.gameObject.transform);
                            LeanTween.move(tableauTopCard.gameObject, foundationSlot.gameObject.transform, animationSpeed).setOnComplete(() =>
                            {
                                tableauTopCard.PlaceInSlot(foundationSlot, false);

                                movedCard = true;
                            });

                            yield return new WaitForSeconds(animationSpeed);
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (tableauTopCard != null &&
                        foundationTopCard.Suit == tableauTopCard.Suit &&
                        foundationTopCard.Rank == tableauTopCard.Rank - 1)
                    {
                        tableauTopCard.animationSpeed = animationSpeed;
                        tableauTopCard.transform.SetParent(foundationSlot.gameObject.transform);
                        LeanTween.move(tableauTopCard.gameObject, foundationSlot.gameObject.transform, animationSpeed).setOnComplete(() =>
                        {
                            tableauTopCard.PlaceInSlot(foundationSlot, false);

                            movedCard = true;
                        });

                        yield return new WaitForSeconds(animationSpeed);
                        break;
                    }
                }

                if (movedCard)
                {
                    break;
                }

                var wasteTopCard = waste.GetTopCard();

                if (foundationTopCard == null)
                {
                    if (wasteTopCard != null && wasteTopCard.Rank == Rank.Ace)
                    {
                        wasteTopCard.animationSpeed = animationSpeed;
                        wasteTopCard.transform.SetParent(foundationSlot.gameObject.transform);
                        LeanTween.move(wasteTopCard.gameObject, foundationSlot.gameObject.transform, animationSpeed).setOnComplete(() =>
                        {
                            wasteTopCard.PlaceInSlot(foundationSlot, false);

                            movedCard = true;
                        });
                        yield return new WaitForSeconds(animationSpeed);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                if (wasteTopCard != null &&
                    foundationTopCard.Suit == wasteTopCard.Suit &&
                    foundationTopCard.Rank == wasteTopCard.Rank - 1)
                {
                    wasteTopCard.animationSpeed = animationSpeed;
                    wasteTopCard.transform.SetParent(foundationSlot.gameObject.transform);
                    LeanTween.move(wasteTopCard.gameObject, foundationSlot.gameObject.transform, animationSpeed).setOnComplete(() =>
                    {
                        wasteTopCard.PlaceInSlot(foundationSlot, false);

                        movedCard = true;
                    });
                    yield return new WaitForSeconds(animationSpeed);
                    break;
                }
            }

            if (!movedCard)
            {
                stockManager.PointerClick();
                yield return new WaitForSeconds(animationSpeed);
            }
        }

        inProccess = false;
    }

    /*public void Animation()
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

        float radius = 2f;
        Vector3 center = Vector3.zero;

        List<SolitareCard> allCards = new List<SolitareCard>();
        foreach (var slot in foundationSlots)
        {
            allCards.AddRange(slot.GetCards());
        }

        float angleStep = 360f / allCards.Count;

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
            LeanTween.rotateAround(allCards[i].gameObject, Vector3.forward, 360f, 2f).setRepeat(1).setEaseLinear();
            yield return new WaitForSeconds(animationSpeed);
        }
    }*/
}
