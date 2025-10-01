using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ICustomSelectable
{
    [Header("Settings")]
    [SerializeField] private Sprite backSprite;
    [SerializeField] private float raisedOffset = 1f;
    [SerializeField] private float shadowOffset = 100f;
    [SerializeField] private float duration = 0.05f;
    [SerializeField] private float scaleFactor = 1.1f;
    [SerializeField] private float pressScaleFactor = 0.9f;

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
    private Camera mainCamera;
    private Transform shadow;
    private Vector3 originalScale;

    [HideInInspector] public bool hidden = false;
    [HideInInspector] public bool stopRaised = false;
    [HideInInspector] public GameManager gameManager;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        shadow = transform.GetChild(0).transform;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        Flip();
        Shadow();
    }

    private void Shadow()
    {
        if (position == CardPosition.Deck || position == CardPosition.ResetStack)
        {
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
            {
                shadow.gameObject.SetActive(true);
            }
            else
            {
                shadow.gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            shadow.gameObject.SetActive(true);
        }

        Vector2 viewportCenter = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));

        float distance = transform.position.x - viewportCenter.x;

        float t = Mathf.Clamp01(Mathf.Abs(distance / (Screen.width / 2f)));
        float offsetX = Mathf.Lerp(0f, -Mathf.Sign(distance) * shadowOffset, t);

        Vector3 shadowPos = shadow.localPosition;
        shadowPos.x = offsetX;
        shadow.localPosition = shadowPos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Press();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
        Click();
    }

    private void Flip()
    {
        if (hidden)
        {
            sr.sprite = backSprite;
            return;
        }

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

        LeanTween.move(gameObject, raisedPosition, 0.25f).setEaseOutBack();
        LeanTween.rotate(gameObject, Vector3.zero, 0.25f).setEaseOutBack();

        isRaised = true;
    }

    private void LowerCard()
    {
        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
        }

        LeanTween.move(gameObject, originalPosition, 0.25f).setEaseInBack();
        LeanTween.rotate(gameObject, originalRotation.eulerAngles, 0.25f).setEaseInBack();

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

    public void Select(bool keyboard = false)
    {
        if (position != CardPosition.Hand || stopRaised || side != CardSide.Player) { return; }

        if (!isRaised)
        {
            RaiseCard();
        }
    }

    public void Deselect()
    {
        if (position != CardPosition.Hand || stopRaised || side != CardSide.Player) { return; }

        if (isRaised)
        {
            LowerCard();
        }
    }

    public void Press()
    {
        LeanTween.scale(gameObject, originalScale * pressScaleFactor, duration).setEaseInOutQuad();
    }

    public void Release()
    {
        LeanTween.scale(gameObject, originalScale * scaleFactor, duration).setEaseInOutQuad();
    }

    public void Click()
    {
        if (position != CardPosition.Hand || side != CardSide.Player) { return; }

        if (gameManager.CanClickOnCard(side))
        {
            isRaised = false;
            LeanTween.cancel(gameObject);
            transform.SetPositionAndRotation(originalPosition, originalRotation);
            gameManager.ClickOnCard(this);
        }

        LeanTween.scale(gameObject, originalScale, duration).setEaseInOutQuad();
    }
}