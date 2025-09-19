using Assets.Scripts.Enums;
using System;
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
        if (card.side == CardSide.Player)
        {
            if (opponentTable.GetMainCard() != null && card.element.Beats(opponentTable.GetMainCard().element))
            {
                if (playerTable.SetMainCard(card))
                {
                    playerHand.RemoveCard(card);
                }
            }

            if (opponentTable.GetMainCard() == null)
            {
                if (playerTable.SetMainCard(card))
                {
                    playerHand.RemoveCard(card);

                    if (firstSide == CardSide.Player)
                    {
                        currentSide = CardSide.Opponent;
                    }
                }
                else if (playerTable.SetExtraCard(card))
                {
                    playerHand.RemoveCard(card);
                    currentSide = CardSide.Opponent;
                }
            }
        }
        else if (card.side == CardSide.Opponent)
        {
            if (playerTable.GetMainCard() != null && card.element.Beats(playerTable.GetMainCard().element))
            {
                if (opponentTable.SetMainCard(card))
                {
                    opponentHand.RemoveCard(card);
                }
            }

            if (playerTable.GetMainCard() == null)
            {
                if (opponentTable.SetMainCard(card))
                {
                    opponentHand.RemoveCard(card);

                    if (firstSide == CardSide.Opponent)
                    {
                        currentSide = CardSide.Player;
                    }
                }
                else if (opponentTable.SetExtraCard(card))
                {
                    opponentHand.RemoveCard(card);
                    currentSide = CardSide.Player;
                }
            }
        }
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
