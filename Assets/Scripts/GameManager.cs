using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private int maxHandCards;

    [Header("UI")]
    [SerializeField] private TMP_Text sideText;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button restartButton;

    [Header("References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardDeck;
    [SerializeField] private GameObject resetStack;
    [SerializeField] private HandManager playerHand;
    [SerializeField] private HandManager opponentHand;
    [SerializeField] private TableManager playerTable;
    [SerializeField] private TableManager opponentTable;
    [SerializeField] private IconSettuper iconSettuper;

    [SerializeField] private List<CardData> cardDatas = new();
    [SerializeField] private List<Card> cards = new();

    private CardSide firstSide = CardSide.Player;
    [HideInInspector] public CardSide currentSide = CardSide.Player;
    private float elapsedTime = 0f;
    private bool end = false;

    private void Start()
    {
        DrawCards();
        StartCoroutine(Countdown());
    }

    private void Update()
    {
        if (end)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        }

        elapsedTime += Time.deltaTime;

        sideText.text = $"{currentSide} turn";
    }

    private void OnEnable()
    {
        menuButton.onClick.AddListener(MenuButton_Click);
        restartButton.onClick.AddListener(RestartButton_Click);
    }

    private void OnDisable()
    {
        menuButton.onClick.RemoveListener(MenuButton_Click);
        restartButton.onClick.RemoveListener(RestartButton_Click);
    }

    IEnumerator Countdown()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();

            countdownText.rectTransform.localScale = Vector3.zero;
            LeanTween.scale(countdownText.rectTransform, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);

            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "";
        GameStart();
    }

    private void GameStart()
    {
        StartCoroutine(FillHandsCoroutine(maxHandCards, maxHandCards));
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
            card.gameManager = this;
            card.Rotate();
            card.frontSprite = iconSettuper.GenerateSprite(card.frontSprite, card.element);
            
            cards.Add(card);
        }
    }

    IEnumerator FillHandsCoroutine(int maxPlayerHand, int maxOpponentHand)
    {
        Shuffle(cards);

        yield return StartCoroutine(FillHandCoroutine(playerHand, CardSide.Player, maxPlayerHand));
        yield return StartCoroutine(FillHandCoroutine(opponentHand, CardSide.Opponent, maxOpponentHand));
    }

    IEnumerator FillHandCoroutine(HandManager hand, CardSide side, int maxHand)
    {
        while (hand.GetCardsCount() < maxHand && cards.Count > 0)
        {
            Card card = cards[0];
            card.transform.position = cardDeck.transform.position;
            card.side = side;
            hand.AddCard(card);
            cards.RemoveAt(0);

            yield return new WaitForSeconds(.25f);

            card.Rotate();
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
        if (end) { return; }

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

                    int firstMaxHand = (hand.side == CardSide.Player) ? maxHandCards : hand2.GetCardsCount() + 1;
                    int secondMaxHand = (hand.side == CardSide.Player) ? hand2.GetCardsCount() + 1 : maxHandCards;

                    StartCoroutine(FillHandsCoroutine(firstMaxHand, secondMaxHand));

                    firstSide = hand.side;
                    currentSide = firstSide;

                    table.RemoveCards();
                    table2.RemoveCards();

                    DetermineGameEnd();

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

                        int firstMaxHand = (hand2.side == CardSide.Player) ? maxHandCards : hand2.GetCardsCount() + 2;
                        int secondMaxHand = (hand2.side == CardSide.Player) ? hand2.GetCardsCount() + 2 : maxHandCards;

                        StartCoroutine(FillHandsCoroutine(firstMaxHand, secondMaxHand));

                        table.RemoveCards();
                        table2.RemoveCards();

                        firstSide = hand2.side;
                        currentSide = firstSide;

                        DetermineGameEnd();

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

                        int firstMaxHand = (hand.side == CardSide.Player) ? maxHandCards : hand2.GetCardsCount() + 2;
                        int secondMaxHand = (hand.side == CardSide.Player) ? hand2.GetCardsCount() + 2 : maxHandCards;

                        StartCoroutine(FillHandsCoroutine(firstMaxHand, secondMaxHand));

                        table.RemoveCards();
                        table2.RemoveCards();

                        firstSide = hand.side;
                        currentSide = firstSide;

                        DetermineGameEnd();

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

            Debug.Log($"Winner {winnerSide}");

            int firstMaxHand = (winnerSide == CardSide.Player) ? maxHandCards : loserHand.GetCardsCount() + 2;
            int secondMaxHand = (winnerSide == CardSide.Player) ? loserHand.GetCardsCount() + 2 : maxHandCards;

            StartCoroutine(FillHandsCoroutine(firstMaxHand, secondMaxHand));

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

            StartCoroutine(FillHandsCoroutine(maxHandCards, maxHandCards));

            firstSide = firstSide == CardSide.Player ? CardSide.Opponent : CardSide.Player;
            currentSide = firstSide;
        }

        DetermineGameEnd();

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

    private void DetermineGameEnd()
    {
        if (playerHand.IsEmpty())
        {
            end = true;
            sideText.gameObject.SetActive(false);
            pausePanel.SetActive(false);

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            winnerText.text = string.Format("You win in {0:00}:{1:00}", minutes, seconds);
            endPanel.SetActive(true);
        }
        else if (opponentHand.IsEmpty())
        {
            end = true;
            sideText.gameObject.SetActive(false);
            pausePanel.SetActive(false);

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            winnerText.text = string.Format("You lose in {0:00}:{1:00}", minutes, seconds);
            endPanel.SetActive(true);
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

    #region UI

    private void RestartButton_Click()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void MenuButton_Click()
    {
        SceneManager.LoadScene("MenuScene");
    }

    #endregion
}
