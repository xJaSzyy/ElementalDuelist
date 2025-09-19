using System;
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

public static class CardElementExtensions
{
    public static bool Beats(this CardElement element, CardElement other)
    {
        return (element, other) switch
        {
            (CardElement.Water, CardElement.Water) => true,
            (CardElement.Fire, CardElement.Fire) => true,
            (CardElement.Air, CardElement.Air) => true,
            (CardElement.Earth, CardElement.Earth) => true,
            (CardElement.Water, CardElement.Fire) => true,
            (CardElement.Fire, CardElement.Air) => true,
            (CardElement.Air, CardElement.Earth) => true,
            (CardElement.Earth, CardElement.Water) => true,
            _ => false
        };
    }
}