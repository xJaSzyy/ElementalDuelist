using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SolitareCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Suit Suit { get; set; }
    public Rank Rank { get; set; }
    public bool WasDroppedInSlot { get; set; } = false;

    [Header("State")]
    public bool Hide = true;
    public bool IsDraggable = false;
    public bool IsFlipped = true;

    [Header("Options")]
    public float animationSpeed = .3f;

    [Header("References")]
    [SerializeField] private TMP_Text[] rankTexts;
    [SerializeField] private TMP_Text[] suitTexts;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private GameObject cover;

    public List<SolitareCard> childCards = new();

    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private SolitareCardSlot currentSlot;
    private Image coverImage;
    private RectTransform coverRectTransform;

    private Vector3 lastPosition = Vector3.zero;
    private float targetAngle = 0f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.interactable = !Hide;
        canvasGroup.blocksRaycasts = !Hide;

        currentSlot = GetComponentInParent<SolitareCardSlot>();
        coverImage = cover.GetComponent<Image>();
        coverRectTransform = cover.GetComponent<RectTransform>();

        FlipUpdate(0);
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

        canvasGroup.interactable = !Hide;
        canvasGroup.blocksRaycasts = !Hide;

        UpdateChilds();
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

        coverImage.sprite = flip ? backSprite : defaultSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsDraggable || Hide)
        {
            return;
        }

        var hintManager = FindAnyObjectByType<HintManager>();
        var slot = hintManager.GetAvailableSlot(this);

        if (slot == null)
        {
            return;
        }

        if (slot.slotType == SolitareSlotType.Foundation && childCards.Count > 0)
        {
            return;
        }

        var to = slot.cardsInSlot.Count > 0 ? 
            slot.cardsInSlot[^1].gameObject.transform : 
            slot.gameObject.transform;

        LeanTween.move(gameObject, to, animationSpeed)
            .setOnComplete(() => {
                PlaceInSlot(slot);
            });

        foreach (var childCard in childCards)
        {
            LeanTween.move(childCard.gameObject, to, animationSpeed);
        }

        WasDroppedInSlot = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDraggable || Hide)
        {
            return;
        }

        originalPosition = transform.position;
        originalParent = transform.parent;

        PrepareCardsForDrag();
    }

    public void UpdateChilds()
    {
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

            /*currentSlot.RemoveCard(this);
            foreach (var childCard in childCards)
            {
                currentSlot.RemoveCard(childCard);
            }*/
        }
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
        if (!IsDraggable || Hide)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint))
        {
            transform.position = worldPoint;

            float dragDelta = worldPoint.x - lastPosition.x;

            MoveCover(dragDelta);
            foreach (var childCard in childCards)
            {
                childCard.MoveCover(dragDelta);
            }

            lastPosition = worldPoint;
        }
    }

    public void MoveCover(float dragDelta)
    {
        if (Math.Abs(dragDelta) > .1f)
        {
            targetAngle += dragDelta > 0 ? -15f : 15f;
            targetAngle = Mathf.Clamp(targetAngle, -30f, 30f);

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            coverRectTransform.localRotation = Quaternion.Slerp(
                coverRectTransform.localRotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }
        else
        {
            coverRectTransform.localRotation = Quaternion.Slerp(
                coverRectTransform.localRotation,
                Quaternion.identity,
                Time.deltaTime * 10f
            );
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable || Hide)
        {
            return;
        }

        RestoreCardsAfterDrag();

        if (!WasDroppedInSlot)
        {
            ReturnToOriginalPosition();
        }

        ResetCover();
        foreach (var childCard in childCards)
        {
            childCard.ResetCover();
        }

        WasDroppedInSlot = false;
    }

    public void ResetCover()
    {
        coverRectTransform.localPosition = Vector3.zero;
        coverRectTransform.localRotation = Quaternion.identity;
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
        if (newSlot == null)
        {
            return;
        }

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
        transform.SetParent(newSlot.gameObject.transform);
        newSlot.AddCard(this);
        transform.localPosition = Vector3.zero;

        if (child)
        {
            foreach (var childCard in childCards)
            {
                childCard.transform.SetParent(newSlot.gameObject.transform);
                newSlot.AddCard(childCard);
                childCard.SetCurrentSlot(newSlot);

                childCard.transform.localPosition = Vector3.zero;
            }
        }

        transform.localPosition = Vector3.zero;

        foreach (var newSlotCard in newSlot.cardsInSlot)
        {
            newSlotCard.UpdateChilds();
        }
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

    public void Move(Transform start, Transform end, float speed = .1f)
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        transform.SetParent(canvas.transform, false);

        transform.position = start.position;

        LeanTween.move(gameObject, end.position, speed)
            .setEaseOutBounce()
            .setOnComplete(() =>
            {
                transform.SetParent(end, true); 
                transform.localPosition = Vector3.zero; 
                transform.localRotation = Quaternion.identity;
            });
    }
}