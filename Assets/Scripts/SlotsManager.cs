using Unity.VisualScripting;
using UnityEngine;

public class SlotsManager : MonoBehaviour
{
    [SerializeField] private Side side;

    [SerializeField] private GameObject slotPrefab;

    public void AddSlot(Card card, bool rotate = false)
    {
        if (card.transform.parent.TryGetComponent<CardSlot>(out var cardSlot))
        {
            cardSlot.ClearCard();
        }

        GameObject slotGameObject = Instantiate(slotPrefab, transform);
        CardSlot slot = slotGameObject.GetComponent<CardSlot>();
        slot.side = side;
        slot.SetCard(card, rotate);
    }

    public void RemoveSlot(Card card)
    {
        GameObject table = side == Side.Player ? CardDeckManager.Instance.PlayerTable : CardDeckManager.Instance.OpponentTable;

        var cardSlot = card.transform.parent.GetComponent<CardSlot>();
        if (cardSlot.RemoveCard(table))
        {
            Destroy(cardSlot.gameObject);
        }
    }
}
