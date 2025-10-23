using Assets.Scripts.Enums;
using System.Collections;
using UnityEngine;

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
        while (!gameManager.win)
        {
            bool movedCard = false;

            foreach (var foundationSlot in foundationSlots)
            {
                if (foundationSlot.full)
                    continue;

                var foundationTopCard = foundationSlot.GetTopCard();

                foreach (var tableauSlot in tableauSlots)
                {
                    var tableauTopCard = tableauSlot.GetTopCard();

                    if (foundationTopCard == null)
                    {
                        if (tableauTopCard != null && tableauTopCard.Rank == Rank.Ace)
                        {

                            tableauTopCard.PlaceInSlot(foundationSlot);

                            movedCard = true;

                            Debug.Log($"Moved: {tableauTopCard.name}");
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
                        tableauTopCard.PlaceInSlot(foundationSlot);

                        movedCard = true;

                        Debug.Log($"Moved: {tableauTopCard.name}");
                        yield return new WaitForSeconds(animationSpeed);
                        break;
                    }
                }

                if (movedCard)
                    break;

                var wasteTopCard = waste.GetTopCard();

                if (foundationTopCard == null)
                {
                    if (wasteTopCard != null && wasteTopCard.Rank == Rank.Ace)
                    {

                        wasteTopCard.PlaceInSlot(foundationSlot);

                        movedCard = true;

                        Debug.Log($"Moved: {wasteTopCard.name}");
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
                    wasteTopCard.PlaceInSlot(foundationSlot);

                    Debug.Log($"Moved: {wasteTopCard.name}");

                    movedCard = true;
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
}
