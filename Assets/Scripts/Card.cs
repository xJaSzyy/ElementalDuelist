using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public int value;
    public CardType type;
    public CardElement element;

    public CardPosition position;
    public CardSide side;

    [HideInInspector] public Sprite frontSprite;
    [HideInInspector] public Sprite backSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        Flip();
    }

    private void Flip()
    {
        float yRotation = transform.rotation.eulerAngles.y;

        if (yRotation > 90f && yRotation < 270f)
        {
            image.sprite = backSprite;
        }
        else
        {
            image.sprite = frontSprite;
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


    public void OnPointerClick(PointerEventData eventData)
    {
        CardDeckManager.Instance.OnCardClick(this);
    }
}