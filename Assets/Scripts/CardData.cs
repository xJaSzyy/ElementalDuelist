using Assets.Scripts.Enums;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Card")]
public class CardData : ScriptableObject
{
    public CardElement element;
    public Sprite sprite;
    public int value;
}