using Assets.Scripts.Enums;
using System;
using UnityEngine;
using UnityEngine.UI;

public class IconSettuper : MonoBehaviour
{
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private Color[] colors;

    [SerializeField] private int iconStartIndex;
    [SerializeField] private Vector2Int iconUpPos = new(5, 51);
    [SerializeField] private Vector2Int iconDownPos = new(19, 5);

    private readonly CardElement[] elementsOrder = new CardElement[]
    {
        CardElement.Fire, CardElement.Water, CardElement.Energy,
        CardElement.Earth, CardElement.Nature, CardElement.Magic
    };

    public void Setup(Card card)
    {
        Texture2D cardTex = card.frontSprite.texture;
        Rect cardRect = card.frontSprite.rect;

        int elementIndex = Array.IndexOf(elementsOrder, card.element);
        int iconIndex = (iconStartIndex * 12) + elementIndex * 2;

        Texture2D iconUpTex = iconSprites[iconIndex].texture;
        Rect iconUpRect = iconSprites[iconIndex].rect;

        Texture2D iconDownTex = iconSprites[iconIndex + 1].texture;
        Rect iconDownRect = iconSprites[iconIndex + 1].rect;

        int iconWidth = (int)iconUpRect.width;
        int iconHeight = (int)iconUpRect.height;

        Color[] cardPixels = cardTex.GetPixels(
            (int)cardRect.x,
            (int)cardRect.y,
            (int)cardRect.width,
            (int)cardRect.height); 

        Texture2D newTex = new Texture2D((int)cardRect.width, (int)cardRect.height, TextureFormat.ARGB32, false);
        newTex.SetPixels(cardPixels);

        Color[] pixels = newTex.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a < 1f)
            {
                pixels[i] = Color.clear;
                continue;
            }

            if (IsColorApproximatelyWhite(pixels[i]))
            {
                pixels[i] = colors[elementIndex];
            }
        }

        newTex.SetPixels(0, 0, newTex.width, newTex.height, pixels);
        newTex.Apply();

        Color[] iconUpPixels = iconUpTex.GetPixels(
            (int)iconUpRect.x,
            (int)iconUpRect.y,
            iconWidth,
            iconHeight);

        Color[] iconDownPixels = iconDownTex.GetPixels(
            (int)iconDownRect.x,
            (int)iconDownRect.y,
            iconWidth,
            iconHeight);

        void ReplaceBlock(Vector2Int pos, Color[] iconPixels)
        {
            for (int y = 0; y < iconHeight; y++)
            {
                for (int x = 0; x < iconWidth; x++)
                {
                    int newTexX = pos.x + x;
                    int newTexY = pos.y + y;

                    int pixelIndex = newTexY * newTex.width + newTexX;
                    int iconPixelIndex = y * iconWidth + x;

                    if (pixelIndex >= 0 && pixelIndex < newTex.width * newTex.height)
                    {
                        newTex.SetPixel(newTexX, newTexY, iconPixels[iconPixelIndex]);
                    }
                }
            }
        }

        ReplaceBlock(iconUpPos, iconUpPixels);
        ReplaceBlock(iconDownPos, iconDownPixels);

        newTex.filterMode = FilterMode.Point;
        newTex.Compress(false);

        newTex.Apply();

        Sprite newSprite = Sprite.Create(newTex,
            new Rect(0, 0, newTex.width, newTex.height),
            card.frontSprite.pivot / new Vector2(cardRect.width, cardRect.height),
            card.frontSprite.pixelsPerUnit);

        card.frontSprite = newSprite;
    }

    private bool IsColorApproximatelyWhite(Color color)
    {
        float tolerance = 0.01f; 
        return Mathf.Abs(color.r - 1f) < tolerance
               && Mathf.Abs(color.g - 1f) < tolerance
               && Mathf.Abs(color.b - 1f) < tolerance
               && color.a > 0.9f; 
    }
}