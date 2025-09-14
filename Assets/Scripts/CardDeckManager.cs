using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeckManager : MonoBehaviour
{
    #region Singleton
    private static CardDeckManager instance;

    public static CardDeckManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<CardDeckManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new(typeof(CardDeckManager).Name);
                    instance = singletonObject.AddComponent<CardDeckManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] private int startCardsNumber = 4;

    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private List<Sprite> cardFrontSprites;

    [SerializeField] private GameObject cardDeck;

    [SerializeField] private HandManager playerHand;
    [SerializeField] private HandManager opponentHand;

    [SerializeField] private TableManager playerTable;
    [SerializeField] private TableManager opponentTable;

    [SerializeField] private GameObject cardPrefab;

    public List<Card> cards = new();

    public void Start()
    {
        SpawnCards();
        Shuffle(cards);
        FillHands();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shuffle(cards);
        }
    }

    private void SpawnCards()
    {
        foreach (Sprite sprite in cardFrontSprites)
        {
            GameObject cardGameObject = Instantiate(cardPrefab, cardDeck.transform);
            cardGameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 180, 0);

            Card card = cardGameObject.GetComponent<Card>();
            card.frontSprite = sprite;
            card.backSprite = cardBackSprite;

            cards.Add(card);
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

    private void FillHands()
    {
        for (int i = 0; i < startCardsNumber; i++)
        {
            Card card = cards[i];
            card.Rotate();
            card.side = CardSide.Player;
            playerHand.AddCard(card);
            cards.RemoveAt(i);
        }

        for (int i = 0; i < startCardsNumber; i++)
        {
            Card card = cards[i];
            card.Rotate();
            card.side = CardSide.Opponent;
            opponentHand.AddCard(card);
            cards.RemoveAt(i);
        }
    }

    public void OnCardClick(Card card)
    {
        if (card.position == CardPosition.Hand)
        {
            HandManager hand = card.side == CardSide.Player ? playerHand : opponentHand;
            TableManager table = card.side == CardSide.Player ? playerTable : opponentTable;

            hand.RemoveCard(card);
            table.AddCard(card);
        }
    }
}
