using Assets.Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new();
                instance = gameObject.AddComponent<GameManager>();
            }

            return instance;
        }
    }
    #endregion Singleton

    [Header("Options")]
    [SerializeField] private int maxHandCards;

    [Header("UI")]
    [SerializeField] private TMP_Text sideText;

    [Header("References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardDeck;
    [SerializeField] private GameObject resetStack;
    [SerializeField] private HandManager playerHand;
    [SerializeField] private HandManager opponentHand;
    [SerializeField] private TableManager playerTable;
    [SerializeField] private TableManager opponentTable;

    [SerializeField] private List<CardData> cardDatas = new();
    [SerializeField] private List<Card> cards = new();

    private CardSide firstSide = CardSide.Player;
    private CardSide currentSide = CardSide.Player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DrawCards();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FillHands();
        }

        sideText.text = $"{currentSide} turn";
    }

    private void DrawCards()
    {
        foreach (CardData cardData in cardDatas)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardDeck.transform);
            Card card = cardObject.GetComponent<Card>();
            card.frontSprite = cardData.sprite;
            card.element = cardData.element;
            card.value = cardData.value;
            card.Rotate();
            
            cards.Add(card);
        }
    }

    private void FillHands()
    {
        Shuffle(cards);

        while (playerHand.GetCardsCount() < maxHandCards && cards.Count > 0)
        {
            Card card = cards[0];
            card.transform.position = cardDeck.transform.position;
            card.side = CardSide.Player;
            card.Rotate();
            playerHand.AddCard(card);
            cards.RemoveAt(0);
        }

        while (opponentHand.GetCardsCount() < maxHandCards && cards.Count > 0)
        {
            Card card = cards[0];
            card.transform.position = cardDeck.transform.position;
            card.side = CardSide.Opponent;
            card.Rotate();
            opponentHand.AddCard(card);
            cards.RemoveAt(0);
        }
    }

    public bool CanClickOnCard(CardSide side)
    {
        if (currentSide != side) { return false; }

        TableManager table = side == CardSide.Player ? playerTable : opponentTable;
        return !table.IsFull();
    }

    public void ClickOnCard(Card card)
    {
        TableManager table = currentSide == CardSide.Player ? playerTable : opponentTable;
        HandManager hand = currentSide == CardSide.Player ? playerHand : opponentHand;

        if (table.GetMainCard() == null && card.side == firstSide)
        {
            Debug.Log("FIRST CARD FIRST SIDE");
            if (table.SetMainCard(card))
            {
                hand.RemoveCard(card);
                currentSide = currentSide == CardSide.Player ? CardSide.Opponent : CardSide.Player;
            }
        }
        else if (table.GetMainCard() == null && card.side != firstSide)
        {
            Debug.Log("FIRST CARD SECOND SIDE");
            TableManager table2 = currentSide == CardSide.Player ? opponentTable : playerTable;
            if (card.element.Beats(table2.GetMainCard().element))
            {
                if (table.SetMainCard(card))
                {
                    hand.RemoveCard(card);
                }
            }
            else
            {
                Debug.Log("NOT BEATS");
            }
        }
        else if (table.GetMainCard() != null && card.side != firstSide)
        {
            Debug.Log("SECOND CARD SECOND SIDE");
            if (card.element.Combined(table.GetMainCard().element))
            {
                if (table.SetExtraCard(card))
                {
                    hand.RemoveCard(card);
                    currentSide = currentSide == CardSide.Player ? CardSide.Opponent : CardSide.Player;
                }
            }
            else
            {
                Debug.Log("NOT COMBINE");
            }
        }
        else if (table.GetMainCard() != null && card.side == firstSide)
        {
            Debug.Log("SECOND CARD FIRST SIDE");
            if (card.element.Combined(table.GetMainCard().element))
            {
                if (table.SetExtraCard(card))
                {
                    hand.RemoveCard(card);
                    StartCoroutine(DetermineWinnerCoroutine());
                }
            }
            else
            {
                Debug.Log("NOT COMBINE");
            }
        }
    }

    private void DetermineWinner()
    {
        int playerTotalValue = playerTable.GetTotalValue();
        int opponentTotalValue = opponentTable.GetTotalValue();

        Card main1 = playerTable.GetMainCard();
        Card main2 = opponentTable.GetMainCard();
        Card extra1 = playerTable.GetExtraCard();
        Card extra2 = opponentTable.GetExtraCard();

        if (playerTotalValue != opponentTotalValue)
        {
            CardSide loserSide = playerTotalValue > opponentTotalValue ? CardSide.Opponent : CardSide.Player;
            TableManager loserTable = playerTotalValue > opponentTotalValue ? opponentTable : playerTable;
            HandManager loserHand = playerTotalValue > opponentTotalValue ? opponentHand : playerHand;

            main1.transform.localScale = Vector3.one;
            main2.transform.localScale = Vector3.one;

            main1.side = loserSide;
            loserHand.AddCard(main1);
            Debug.Log("LOSER TAKE ONE");
            main2.side = loserSide;
            loserHand.AddCard(main2);
            Debug.Log("LOSER TAKE ONE AGAIN");

            CardToResetStack(extra1);
            CardToResetStack(extra2);
        }
        else
        {
            CardToResetStack(main1);
            CardToResetStack(main2);
            CardToResetStack(extra1);
            CardToResetStack(extra2);
        }

        playerTable.RemoveCards();
        opponentTable.RemoveCards();
    }

    IEnumerator DetermineWinnerCoroutine()
    {
        yield return new WaitForSeconds(.25f);
        DetermineWinner();
    }

    private void CardToResetStack(Card card)
    {
        card.position = CardPosition.ResetStack;
        card.transform.localScale = Vector3.one;
        card.transform.SetParent(resetStack.transform, false);
        card.transform.localPosition = Vector3.zero;
        card.transform.rotation = Quaternion.identity;
        card.Rotate();
        Debug.Log("CARD TO RESET");
    }

    private void Shuffle<T>(List<T> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i] as Card;
            if (card != null)
            {
                card.transform.SetSiblingIndex(i);
            }
        }
    }
}
