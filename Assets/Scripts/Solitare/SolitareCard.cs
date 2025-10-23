using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SolitareCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Suit Suit { get; set; }
    public Rank Rank { get; set; }
    public bool WasDroppedInSlot { get; set; } = false;

    [Header("State")]
    public bool Hide = false;
    public bool IsDraggable = true;
    public bool IsFlipped = false;

    [Header("Options")]
    public float animationSpeed = .3f;

    [Header("References")]
    [SerializeField] private TMP_Text[] rankTexts;
    [SerializeField] private TMP_Text[] suitTexts;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite backSprite;

    public List<SolitareCard> childCards = new();

    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private SolitareCardSlot currentSlot;
    private Image image;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        currentSlot = GetComponentInParent<SolitareCardSlot>();
        image = GetComponent<Image>();
    }

    public void UpdateVisual()
    {
        int rank = Convert.ToInt32(Rank);

        string rankText;

        if (rank <= 10 && rank >= 2)
        {
            rankText = rank.ToString();
        }
        else
        {
            rankText = Rank.ToString().ToCharArray()[0].ToString();
        }

        string icon;
        Color color;

        if (Suit == Suit.Clubs)
        {
            icon = "♣";
            color = Color.black;
        }
        else if (Suit == Suit.Diamonds)
        {
            icon = "♦";
            color = Color.red;
        }
        else if (Suit == Suit.Hearts)
        {
            icon = "♥";
            color = Color.red;
        }
        else
        {
            icon = "♠";
            color = Color.black;
        }

        foreach (var item in suitTexts)
        {
            item.text = icon;
            item.color = color;
        }

        foreach (var item in rankTexts)
        {
            item.text = rankText;
            item.color = color;
        }
    }

    public void Flip()
    {
        IsFlipped = !IsFlipped;

        LeanTween.scaleX(gameObject, IsFlipped ? -1 : 1, animationSpeed).setOnUpdate(FlipUpdate);

        Hide = IsFlipped;
        IsDraggable = !IsFlipped;
    }

    private void FlipUpdate(float val)
    {
        bool flip = transform.localScale.x < 0f;

        foreach (var item in rankTexts)
        {
            item.gameObject.SetActive(!flip);
        }
        foreach (var item in suitTexts)
        {
            item.gameObject.SetActive(!flip);
        }

        image.sprite = flip ? backSprite : defaultSprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDraggable || Hide) return;

        originalPosition = transform.position;
        originalParent = transform.parent;

        childCards.Clear();
        if (currentSlot != null)
        {
            int currentIndex = currentSlot.cardsInSlot.IndexOf(this);
            if (currentIndex >= 0)
            {
                for (int i = currentIndex + 1; i < currentSlot.cardsInSlot.Count; i++)
                {
                    childCards.Add(currentSlot.cardsInSlot[i]);
                }
            }

            currentSlot.RemoveCard(this);
            foreach (var childCard in childCards)
            {
                currentSlot.RemoveCard(childCard);
            }
        }

        PrepareCardsForDrag();
    }

    private void PrepareCardsForDrag()
    {
        LeanTween.alphaCanvas(canvasGroup, 0.6f, animationSpeed);
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(transform.root);

        LeanTween.scale(gameObject, Vector3.one * 1.1f, animationSpeed).setEaseInOutSine();

        foreach (var card in childCards)
        {
            LeanTween.alphaCanvas(card.canvasGroup, 0.6f, animationSpeed);
            card.canvasGroup.blocksRaycasts = false;
            card.transform.SetParent(transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDraggable || Hide) return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint))
        {
            transform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable || Hide) return;

        RestoreCardsAfterDrag();

        if (!WasDroppedInSlot)
        {
            ReturnToOriginalPosition();
        }

        WasDroppedInSlot = false;
    }

    private void RestoreCardsAfterDrag()
    {
        LeanTween.alphaCanvas(canvasGroup, 1f, animationSpeed);
        canvasGroup.blocksRaycasts = true;

        LeanTween.scale(gameObject, Vector3.one, animationSpeed).setEaseInOutSine();

        foreach (var card in childCards)
        {
            LeanTween.alphaCanvas(card.canvasGroup, 1f, animationSpeed);
            LeanTween.scale(card.gameObject, Vector3.one, animationSpeed).setEaseInOutSine();
            card.canvasGroup.blocksRaycasts = true;
        }
    }


    public void PlaceInSlot(SolitareCardSlot newSlot, bool child = true)
    {
        if (newSlot == null) return;

        if (currentSlot != null)
        {
            currentSlot.RemoveCard(this);
            foreach (var childCard in childCards)
            {
                currentSlot.RemoveCard(childCard);
            }
            currentSlot.FlipLastCard();
        }

        currentSlot = newSlot;
        transform.SetParent(newSlot.gameObject.transform, true);
        newSlot.AddCard(this);
        transform.localPosition = Vector3.zero;

        if (child)
        {
            foreach (var childCard in childCards)
            {
                childCard.transform.SetParent(newSlot.gameObject.transform, true);
                newSlot.AddCard(childCard);
                childCard.SetCurrentSlot(newSlot);

                childCard.transform.localPosition = Vector3.zero;
            }
        }

        transform.localPosition = Vector3.zero;
    }

    public void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
        transform.SetParent(originalParent);

        currentSlot = originalParent.GetComponent<SolitareCardSlot>();
        if (currentSlot != null)
        {
            currentSlot.AddCard(this);
            foreach (var childCard in childCards)
            {
                childCard.transform.SetParent(originalParent, true);
                currentSlot.AddCard(childCard);
                childCard.SetCurrentSlot(currentSlot);
            }
        }
    }

    public void SetCurrentSlot(SolitareCardSlot slot)
    {
        currentSlot = slot;
    }
}