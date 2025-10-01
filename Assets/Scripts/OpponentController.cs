using Assets.Scripts.Enums;
using System;
using System.Collections;
using UnityEngine;

public class OpponentController : MonoBehaviour
{
    [SerializeField] private HandManager hand;
    [SerializeField] private TableManager table;
    [SerializeField] private TableManager playerTable;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float duration = .5f;
    [SerializeField] private float holdTime = 1f;

    private float opponentSideHoldTime = 0f;
    private bool actionPerformed = false;

    private void Update()
    {
        if (hand.GetCardsCount() <= 0)
        {
            return;
        }

        if (gameManager.currentSide == CardSide.Opponent)
        {
            opponentSideHoldTime += Time.deltaTime;

            if (opponentSideHoldTime >= holdTime && !actionPerformed)
            {
                actionPerformed = true;
                PerformAction();
            }
        }
        else
        {
            opponentSideHoldTime = 0f;
            actionPerformed = false;
        }
    }

    private void PerformAction()
    {
        StartCoroutine(ActionCoroutine());
    }

    IEnumerator ActionCoroutine()
    {
        Card mainCard = table.GetMainCard();
        Card extraCard = table.GetExtraCard();

        Card playerMainCard = playerTable.GetMainCard();

        Card cardToTable = hand.GetCards()[0];

        if (playerMainCard != null)
        {
            if (mainCard == null)
            {
                foreach (Card handCard in hand.GetCards())
                {
                    if (handCard.element.Beats(playerMainCard.element))
                    {
                        cardToTable = handCard;
                        break;
                    }
                }
            }
            else if (extraCard == null)
            {
                foreach (Card handCard in hand.GetCards())
                {
                    if (handCard.element.Combined(mainCard.element))
                    {
                        cardToTable = handCard;
                        break;
                    }
                }
            }
        }

        if (cardToTable == null)
        {
            yield break;
        }

        gameManager.ClickOnCard(cardToTable);

        yield return new WaitForSeconds(duration);

        opponentSideHoldTime = 0f;
        actionPerformed = false;
    }
}
