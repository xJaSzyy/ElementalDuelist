using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
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

    [Header("References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardDeck;
    [SerializeField] private HandManager playerHand;
    [SerializeField] private HandManager opponentHand;
    [SerializeField] private TableManager playerTable;
    [SerializeField] private TableManager opponentTable;

    [SerializeField] private List<CardData> cardDatas = new();
    [SerializeField] private List<Card> cards = new();

    private CardSide currentSide;

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
    }

    private void DrawCards()
    {
        foreach (CardData cardData in cardDatas)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardDeck.transform);
            Card card = cardObject.GetComponent<Card>();
            card.frontSprite = cardData.sprite;
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
        TableManager table = side == CardSide.Player ? playerTable : opponentTable;
        return !table.IsFull();
    }

    public void ClickOnCard(Card card)
    {
        if (card.side == CardSide.Player)
        {
            if (playerTable.SetMainCard(card) || playerTable.SetExtraCard(card))
            {
                playerHand.RemoveCard(card);
            }
        }
        else if (card.side == CardSide.Opponent)
        {
            if (opponentTable.SetMainCard(card) || opponentTable.SetExtraCard(card))
            {
                opponentHand.RemoveCard(card);
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
