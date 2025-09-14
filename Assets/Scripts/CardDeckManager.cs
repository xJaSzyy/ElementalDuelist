using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject cardDiscard;

    [SerializeField] private HandManager playerHand;
    [SerializeField] private HandManager opponentHand;

    [SerializeField] private TableManager playerTable;
    [SerializeField] private TableManager opponentTable;

    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private Text turnText;

    [SerializeField] private List<Card> cards = new();
    
    private CardSide currentSide;

    public void Start()
    {
        SpawnCards();
        FillHands();
    }

    private void Update()
    {
        turnText.text = currentSide.ToString() + " turn";
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
                
            string[] parts = sprite.name.Split('_');

            if (Enum.TryParse(parts[0], true, out CardElement element))
            {
                card.element = element;
            }

            if (Enum.TryParse(parts[1], true, out CardType type))
            {
                card.type = type;
            }

            card.value = Convert.ToInt32(parts[2]);

            cards.Add(card);
        }
    }

    private void Shuffle<T>(List<T> cards)
    {
        currentSide = (UnityEngine.Random.value < 0.5f) ? CardSide.Player : CardSide.Opponent;

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
        Shuffle(cards);

        while (playerHand.GetCardsCount() < startCardsNumber && cards.Count > 0)
        {
            Card card = cards[0];
            card.Rotate();
            card.side = CardSide.Player;
            playerHand.AddCard(card);
            cards.RemoveAt(0);
        }

        while (opponentHand.GetCardsCount() < startCardsNumber && cards.Count > 0)
        {
            Card card = cards[0];
            card.Rotate();
            card.side = CardSide.Opponent;
            opponentHand.AddCard(card);
            cards.RemoveAt(0);
        }
    }

    public void OnCardClick(Card card)
    {
        if (card.side != currentSide)
        {
            return;
        }

        if (card.position == CardPosition.Hand)
        {
            TableManager table = (card.side == CardSide.Player) ? playerTable : opponentTable;
            HandManager hand = (card.side == CardSide.Player) ? playerHand : opponentHand;

            TableManager table2 = (table == playerTable) ? opponentTable : playerTable;


            if (table.GetCardsCount() == 0 && table2.IsFull())
            {
                if (!card.element.Beats(table2.GetCards()[0].element))
                {
                    return;
                }

                Debug.Log($"{card.element} beats {table2.GetCards()[0].element}");
            }

            hand.RemoveCard(card);
            table.AddCard(card);
        }
    }

    public void TryDetermineWinner()
    {
        if (playerTable.IsFull() && opponentTable.IsFull())
        {
            int playerPower = playerTable.GetCardsPower();
            int opponentPower = opponentTable.GetCardsPower();

            List<Card> playerTableCards = playerTable.GetCards();
            List<Card> opponentTableCards = opponentTable.GetCards();

            if (playerPower != opponentPower)
            {
                HandManager hand = (playerPower > opponentPower) ? playerHand : opponentHand;
                CardSide side = (playerPower > opponentPower) ? CardSide.Player : CardSide.Opponent;

                foreach (Card item in playerTableCards)
                {
                    item.side = side;
                    hand.AddCard(item);
                }
                foreach (Card item in opponentTableCards)
                {
                    item.side = side;
                    hand.AddCard(item);
                }

                currentSide = side;
            }
            else
            {
                foreach (Card item in playerTableCards)
                {
                    item.side = CardSide.None;
                    item.Rotate();
                    item.gameObject.transform.SetParent(cardDiscard.transform);
                    item.gameObject.transform.localPosition = Vector3.zero;
                }
                foreach (Card item in opponentTableCards)
                {
                    item.side = CardSide.None;
                    item.Rotate();
                    item.gameObject.transform.SetParent(cardDiscard.transform);
                    item.gameObject.transform.localPosition = Vector3.zero;
                }
            }

            playerTable.RemoveCards();
            opponentTable.RemoveCards();
            FillHands();
        }
    }

    public void MoveTurn()
    {
        currentSide = currentSide == CardSide.Player ? CardSide.Opponent : CardSide.Player;
    }
}

public static class CardElementExtensions
{
    public static bool Beats(this CardElement attacker, CardElement defender)
    {
        if (attacker == defender)
        {
            return true;
        }

        return (attacker == CardElement.Water && defender == CardElement.Fire)
            || (attacker == CardElement.Fire && defender == CardElement.Air)
            || (attacker == CardElement.Air && defender == CardElement.Earth)
            || (attacker == CardElement.Earth && defender == CardElement.Water);
    }
}
