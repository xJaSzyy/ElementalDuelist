using UnityEngine;

[CreateAssetMenu(menuName = "SO/Card")]
public class CardData : ScriptableObject
{
    public CardElement element;
    public Sprite sprite;
    public int value;
}

public enum CardElement
{
    Fire = 0,
    Water = 1,
    Earth = 2,
    Air = 3
}
