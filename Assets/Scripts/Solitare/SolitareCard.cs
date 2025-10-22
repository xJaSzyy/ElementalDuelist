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
    public bool hide = false;
    public bool IsDraggable = true;

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
        bool isFlipped = transform.localScale.x < 0;
        isFlipped = !isFlipped;
        Vector3 scale = transform.localScale;
        scale.x = isFlipped ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
        
        foreach (var item in rankTexts)
        {
            item.gameObject.SetActive(!isFlipped);
        }
        foreach (var item in suitTexts)
        {
            item.gameObject.SetActive(!isFlipped);
        }

        image.sprite = isFlipped ? backSprite : defaultSprite;

        hide = isFlipped;
        IsDraggable = !isFlipped;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDraggable || hide) return;

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
        LeanTween.alphaCanvas(canvasGroup, 0.6f, 0.3f);
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(transform.root);

        LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.3f).setEaseInOutSine();

        foreach (var card in childCards)
        {
            LeanTween.alphaCanvas(card.canvasGroup, 0.6f, 0.3f);
            card.canvasGroup.blocksRaycasts = false;
            card.transform.SetParent(transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDraggable || hide) return;

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
        if (!IsDraggable || hide) return;

        RestoreCardsAfterDrag();

        if (!WasDroppedInSlot)
        {
            ReturnToOriginalPosition();
        }

        WasDroppedInSlot = false;
    }

    private void RestoreCardsAfterDrag()
    {
        LeanTween.alphaCanvas(canvasGroup, 1f, 0.3f);
        canvasGroup.blocksRaycasts = true;

        LeanTween.scale(gameObject, Vector3.one, 0.3f).setEaseInOutSine();

        foreach (var card in childCards)
        {
            LeanTween.alphaCanvas(card.canvasGroup, 1f, 0.3f);
            LeanTween.scale(card.gameObject, Vector3.one, 0.3f).setEaseInOutSine();
            card.canvasGroup.blocksRaycasts = true;
        }
    }


    public void PlaceInSlot(SolitareCardSlot newSlot)
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

        foreach (var childCard in childCards)
        {
            childCard.transform.SetParent(newSlot.gameObject.transform, true);
            newSlot.AddCard(childCard);
            childCard.SetCurrentSlot(newSlot);

            childCard.transform.localPosition = Vector3.zero;
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