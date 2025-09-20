using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private Sprite backSprite;
    [SerializeField] private float raisedOffset = 1f;
    
    [Header("Other")]
    public Sprite frontSprite;
    public CardPosition position;
    public CardElement element;
    public int value;
    public CardSide side;

    private SpriteRenderer sr;
    private Vector3 originalPosition;
    private Vector3 raisedPosition;
    private Quaternion originalRotation;
    private Quaternion raisedRotation;
    private bool isRaised = false;
    
    [HideInInspector] public bool stopRaised = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Flip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (position != CardPosition.Hand || stopRaised) { return; }

        if (!isRaised)
        {
            RaiseCard();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (position != CardPosition.Hand || stopRaised) { return; }

        if (isRaised)
        {
            LowerCard();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (position != CardPosition.Hand) { return; }

        if (GameManager.Instance.CanClickOnCard(side))
        {
            isRaised = false;
            LeanTween.cancel(gameObject);
            transform.SetPositionAndRotation(originalPosition, originalRotation);
            GameManager.Instance.ClickOnCard(this);
        }
    }

    private void Flip()
    {
        float yRotation = transform.rotation.eulerAngles.y;

        if (yRotation > 90f && yRotation < 270f)
        {
            sr.sprite = backSprite;
        }
        else
        {
            sr.sprite = frontSprite;
        }
    }

    public void Rotate()
    {
        transform.Rotate(0f, 180f, 0f);

        Vector3 euler = transform.rotation.eulerAngles;
        if (euler.y >= 360f)
        {
            euler.y -= 360f;
        }
        else if (euler.y < 0f)
        {
            euler.y += 360f;
        }

        transform.rotation = Quaternion.Euler(euler);
    }

    private void RaiseCard()
    {
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        };

        LeanTween.move(gameObject, raisedPosition, 0.3f).setEaseOutCubic();
        LeanTween.rotate(gameObject, Vector3.zero, 0.3f).setEaseOutCubic();

        isRaised = true;
    }

    private void LowerCard()
    {
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }

        LeanTween.move(gameObject, originalPosition, 0.3f).setEaseInCubic();
        LeanTween.rotate(gameObject, originalRotation.eulerAngles, 0.3f).setEaseInCubic();

        isRaised = false;
    }

    public void UpdatePosition(Vector3 splinePos, Quaternion rotation)
    {
        originalPosition = splinePos;
        originalRotation = rotation;

        raisedPosition = originalPosition + (side == CardSide.Player ? new Vector3(0f, raisedOffset, 0f) : new Vector3(0f, -raisedOffset, 0f));
        raisedRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void SetSortingOrder(int index)
    {
        sr.sortingOrder = index;
    }
}