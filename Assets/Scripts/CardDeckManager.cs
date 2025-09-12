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

    [Header("Settings")]
    [SerializeField] private float deckAnimationTime = .25f;

    [Header("Deck")]
    [SerializeField] private GameObject cardDeck;
    [SerializeField] private List<Card> deckCards = new();
    [SerializeField] private List<Sprite> cardSprites = new();

    [Header("Hand")]
    [SerializeField] private SlotsManager playerSlotsManager;
    [SerializeField] private SlotsManager opponentSlotsManager;

    [Header("Table")]
    public GameObject PlayerTable;
    public GameObject OpponentTable;

    private Dictionary<(Side side, SlotType type), Card> cardsOnTable = new();

    private void Start()
    {
        FillCardSprites();
        ShuffleDeck();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShuffleDeck();
            DealCards();
        }
    }

    private void ShuffleDeck()
    {
        GetDeckCards();

        deckCards = Shuffle(deckCards);
    }

    private void DealCards()
    {
        for (int i = 0; i < 4; i++)
        {
            playerSlotsManager.AddSlot(deckCards[i], true);
        }

        for (int i = 0; i < 4; i++)
        {
            opponentSlotsManager.AddSlot(deckCards[i + 4], true);
        }
    }

    private void FillCardSprites()
    {
        int c = 10;
        for (int i = 0; i < cardDeck.transform.childCount; i++)
        {
            Transform child = cardDeck.transform.GetChild(i);
            var card = child.GetComponent<Card>();
            card.frontSprite = cardSprites[i];
            card.type = i < 10 ? CardType.Fire : i < 20 ? CardType.Water : i < 30 ? CardType.Earth : CardType.Air;
            card.value = c;
            c--;
            if (c== 0)
            {
                c = 10;
            }
        }
    }

    private void GetDeckCards()
    {
        deckCards.Clear();
        foreach (Transform child in cardDeck.transform)
        {
            deckCards.Add(child.GetComponent<Card>());
        }
    }

    public void UpdateCards()
    {
        GetDeckCards();
    }

    public void SetCardOnTable(Card card, Side side, SlotType type)
    {
        cardsOnTable[(side, type)] = card;

        if (cardsOnTable.ContainsKey((Side.Player, SlotType.Main)) &&
            cardsOnTable.ContainsKey((Side.Player, SlotType.Extra)) &&
            cardsOnTable.ContainsKey((Side.Opponent, SlotType.Main)) &&
            cardsOnTable.ContainsKey((Side.Opponent, SlotType.Extra)) &&
            cardsOnTable[(Side.Player, SlotType.Main)] != null &&
            cardsOnTable[(Side.Player, SlotType.Extra)] != null &&
            cardsOnTable[(Side.Opponent, SlotType.Main)] != null &&
            cardsOnTable[(Side.Opponent, SlotType.Extra)] != null)
        {
            List<Card> cards = new List<Card>
            {
                cardsOnTable[(Side.Player, SlotType.Main)],
                cardsOnTable[(Side.Player, SlotType.Extra)],
                cardsOnTable[(Side.Opponent, SlotType.Main)],
                cardsOnTable[(Side.Opponent, SlotType.Extra)]
            };

            int playerPower = cards[0].value + cards[1].value;
            int opponentPower = cards[2].value + cards[3].value;

            if (playerPower == opponentPower)
            {
                Debug.Log("Draw! Cards fall");
            }
            else
            {
                if (playerPower > opponentPower)
                {
                    cards.ForEach(card => playerSlotsManager.AddSlot(card));
                }
                else
                {
                    cards.ForEach(card => opponentSlotsManager.AddSlot(card));
                }
            }
        }
    }

    public List<T> Shuffle<T>(List<T> list)
    {
        System.Random rnd = new();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }
}
