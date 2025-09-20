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

        FillHand(playerHand, CardSide.Player, maxHandCards);
        FillHand(opponentHand, CardSide.Opponent, maxHandCards);
    }

    private void FillHand(HandManager hand, CardSide side, int maxHand)
    {
        while (hand.GetCardsCount() < maxHand && cards.Count > 0)
        {
            Card card = cards[0];
            card.transform.position = cardDeck.transform.position;
            card.side = side;
            card.Rotate();
            hand.AddCard(card);
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
        TableManager table2 = currentSide == CardSide.Player ? opponentTable : playerTable;
        HandManager hand2 = currentSide == CardSide.Player ? opponentHand : playerHand;

        if (table.GetMainCard() == null && card.side == firstSide) // First card && First side
        {
            if (table.SetMainCard(card))
            {
                hand.RemoveCard(card);

                if (!hand2.CanBeat(card))
                {
                    hand2.AddCard(card);

                    FillHand(hand2, hand2.side, hand2.GetCardsCount() + 1);
                    FillHand(hand, hand.side, maxHandCards);

                    firstSide = hand.side;
                    currentSide = firstSide;

                    Debug.Log("NOT BEATS");
                }
                else
                {
                    SwapCurrentSide();
                }
            }
        }
        else if (table.GetMainCard() == null && card.side != firstSide)
        {
            if (card.element.Beats(table2.GetMainCard().element)) // First card && Second side
            {
                if (table.SetMainCard(card))
                {
                    hand.RemoveCard(card);

                    if (hand.IsEmpty() || hand2.IsEmpty())
                    {
                        StartCoroutine(DetermineWinnerCoroutine(false));
                    }
                    else if (!hand.CanCombined(card))
                    {
                        var card2 = table2.GetMainCard();
                        hand.AddCard(card);
                        hand.AddCard(card2);

                        FillHand(hand, hand.side, hand.GetCardsCount() + 2);
                        FillHand(hand2, hand2.side, maxHandCards);

                        table2.RemoveCards();

                        firstSide = hand2.side;
                        currentSide = firstSide;

                        Debug.Log("NOT COMBINED");
                    }
                }
            }
        }
        else if (table.GetMainCard() != null && card.side != firstSide) // Second card && Second side
        {
            if (card.element.Combined(table.GetMainCard().element))
            {
                if (table.SetExtraCard(card))
                {
                    hand.RemoveCard(card);

                    if (!hand2.CanCombined(table2.GetMainCard()))
                    {
                        var main1 = table.GetMainCard();
                        var main2 = table2.GetMainCard();
                        var extra1 = table.GetExtraCard();

                        hand2.AddCard(main1);
                        hand2.AddCard(main2);
                        CardToResetStack(extra1);

                        FillHand(hand2, hand2.side, hand2.GetCardsCount() + 2);
                        FillHand(hand, hand.side, maxHandCards);

                        table.RemoveCards();
                        table2.RemoveCards();

                        firstSide = hand.side;
                        currentSide = firstSide;

                        Debug.Log("NOT COMBINED");
                    }
                    else
                    {
                        SwapCurrentSide();
                    }
                }
            }
        }
        else if (table.GetMainCard() != null && card.side == firstSide) // Second card && First side
        {
            if (card.element.Combined(table.GetMainCard().element))
            {
                if (table.SetExtraCard(card))
                {
                    hand.RemoveCard(card);
                    StartCoroutine(DetermineWinnerCoroutine(true));
                }
            }
        }
    }

    private void DetermineWinner(bool fullHand = true)
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
            CardSide winnerSide = playerTotalValue > opponentTotalValue ? CardSide.Player : CardSide.Opponent;
            TableManager loserTable = playerTotalValue > opponentTotalValue ? opponentTable : playerTable;
            HandManager loserHand = playerTotalValue > opponentTotalValue ? opponentHand : playerHand;
            HandManager winnerHand = playerTotalValue > opponentTotalValue ? playerHand : opponentHand;

            loserHand.AddCard(main1);
            loserHand.AddCard(main2);

            if (fullHand)
            {
                CardToResetStack(extra1);
                CardToResetStack(extra2);
            }

            Debug.Log($"Winner {winnerSide} & {playerTotalValue} > {opponentTotalValue} {playerTotalValue > opponentTotalValue}");
            FillHand(loserHand, loserSide, loserHand.GetCardsCount() + 2);
            FillHand(winnerHand, winnerSide, maxHandCards);

            firstSide = winnerSide;
            currentSide = firstSide;
        }
        else
        {
            CardToResetStack(main1);
            CardToResetStack(main2);

            if (fullHand)
            {
                CardToResetStack(extra1);
                CardToResetStack(extra2);
            }

            FillHand(playerHand, CardSide.Player, maxHandCards);
            FillHand(opponentHand, CardSide.Opponent, maxHandCards);

            firstSide = firstSide == CardSide.Player ? CardSide.Opponent : CardSide.Player;
            currentSide = firstSide;
        }

        if (playerHand.IsEmpty())
        {
            Debug.Log("PLAYER WIN");
        }
        else if (opponentHand.IsEmpty())
        {
            Debug.Log("OPPONENT WIN");
        }

        playerTable.RemoveCards();
        opponentTable.RemoveCards();
    }

    IEnumerator DetermineWinnerCoroutine(bool fullHand = true)
    {
        yield return new WaitForSeconds(.25f);
        DetermineWinner(fullHand);
    }

    private void CardToResetStack(Card card)
    {
        card.position = CardPosition.ResetStack;

        LeanTween.move(card.gameObject, resetStack.transform.position, 0.25f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scale(card.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.rotate(card.gameObject, new Vector3(0f, 180f, 0f), 0.25f).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                card.transform.SetParent(resetStack.transform, false);
                card.transform.localPosition = Vector3.zero;
                card.transform.rotation = Quaternion.identity;
                card.Rotate();
            });
    }

    private void SwapCurrentSide()
    {
        currentSide = currentSide == CardSide.Player ? CardSide.Opponent : CardSide.Player;
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
