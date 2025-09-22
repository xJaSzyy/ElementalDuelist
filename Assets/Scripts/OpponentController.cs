using Assets.Scripts.Enums;
using System;
using System.Collections;
using UnityEngine;

public class OpponentController : MonoBehaviour
{
    [SerializeField] private HandManager hand;
    [SerializeField] private TableManager table;
    [SerializeField] private TableManager playerTable;
    [SerializeField] private float duration = .5f;

    private void Update()
    {
        if (GameManager.Instance.currentSide == CardSide.Opponent && 
            hand.GetCardsCount() > 0)
        {
            Action();
        }
    }

    public void Action()
    {
        StartCoroutine(ActionCoroutine());
    }

    IEnumerator ActionCoroutine()
    {
        yield return new WaitForSeconds(duration);

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
        GameManager.Instance.ClickOnCard(cardToTable);
    }
}
