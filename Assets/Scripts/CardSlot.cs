using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public Side side;

    [SerializeField] private SlotType type;
    [SerializeField] private float rotateAnimationTime = 1.25f;
    [SerializeField] private float setAnimationTime = 1f;

    public Card currentCard;

    public void SetCard(Card card, bool rotate = false, bool table = false)
    {
        if (currentCard != null) { return; }

        Vector3 worldPos = card.transform.position;
        card.transform.SetParent(transform);
        card.transform.position = worldPos;
        LeanTween.moveLocal(card.gameObject, Vector3.zero, setAnimationTime).setEase(LeanTweenType.easeInOutQuad);

        RectTransform parentRect = card.transform.parent as RectTransform;
        RectTransform cardRect = card.transform as RectTransform;
        if (parentRect != null && cardRect != null)
        {
            AnimateResize(cardRect, parentRect.rect.size, setAnimationTime);
        }

        card.pos = table ? "table" : "hand";
        Debug.Log(card.type + card.value + table.ToString());
        card.side = side;
        currentCard = card;
        CardDeckManager.Instance.UpdateCards();

        if (rotate)
        {
            float currentY = currentCard.transform.eulerAngles.y;
            float rotateBy = 180f;
            float targetY = (currentY + rotateBy) % 360f;
            LeanTween.rotateY(currentCard.gameObject, targetY, rotateAnimationTime).setEase(LeanTweenType.easeInOutQuad);
        }

        if (table)
        {
            CardDeckManager.Instance.SetCardOnTable(card, side, type);
        }
    }

    public bool RemoveCard(GameObject table)
    {
        if (currentCard == null) { return false; }

        CardSlot finalSlot = null;

        CardSlot mainSlot = table.transform.GetChild(0).GetComponent<CardSlot>();
        CardSlot extraSlot = table.transform.GetChild(1).GetComponent<CardSlot>();

        if (mainSlot.currentCard == null)
        {
            finalSlot = mainSlot;
        }
        else if (extraSlot.currentCard == null)
        {
            finalSlot = extraSlot;
        }
        else
        {
            return false;
        }

        finalSlot.SetCard(currentCard, false, true);

        currentCard.pos = "table";
        currentCard = null;

        CardDeckManager.Instance.UpdateCards();

        return true;
    }

    public void ClearCard()
    {
        if (currentCard != null)
        {
            currentCard.pos = "hand";
            currentCard = null;
        }
    }

    private void AnimateResize(RectTransform rectTransform, Vector2 targetSize, float duration)
    {
        Vector2 startSize = rectTransform.sizeDelta;

        LeanTween.value(gameObject, 0f, 1f, duration).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float t) =>
        {
            rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
        });
    }
}

public enum SlotType
{
    None = 0,
    Main = 1,
    Extra = 2
}
