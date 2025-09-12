using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public CardType type;
    public int value;

    [SerializeField] private Sprite backSprite;

    [HideInInspector] public Sprite frontSprite;
    /*[HideInInspector]*/ public string pos = "hand";
    [HideInInspector] public Side side;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pos.Contains("hand"))
        {
            transform.parent.parent.GetComponent<SlotsManager>().RemoveSlot(this);
        }
    }
}

public enum Side
{
    Player = 0,
    Opponent = 1
}

public enum CardType
{
    Fire = 0,
    Water = 1,
    Earth = 2,
    Air = 3
}