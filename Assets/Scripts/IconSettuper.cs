using Assets.Scripts.Enums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconSettuper : MonoBehaviour
{
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private Color[] colors;

    [SerializeField] private Vector2Int iconUpPos = new(5, 51);
    [SerializeField] private Vector2Int iconDownPos = new(19, 5);
    
    private SettingsData settings;
    private int iconStartIndex;
    private int colorStartIndex;

    private readonly CardElement[] elementsOrder = new CardElement[]
    {
        CardElement.Fire, CardElement.Water, CardElement.Energy,
        CardElement.Earth, CardElement.Nature, CardElement.Magic
    };

    private void Awake()
    {
        settings = Resources.Load<SettingsData>("Settings");

        iconStartIndex = settings.iconStartIndex;
        colorStartIndex = settings.colorStartIndex;
    }

    public Sprite GenerateSprite(Sprite sprite, CardElement element)
    {
        Texture2D cardTex = sprite.texture;
        Rect cardRect = sprite.rect;

        int elementIndex = Array.IndexOf(elementsOrder, element);
        int iconIndex = (iconStartIndex * 12) + elementIndex * 2;
        int colorIndex = (colorStartIndex * 6) + elementIndex;

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

        Color[] pixels = newTex.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a < 1f)
            {
                pixels[i] = Color.clear;
                continue;
            }

            if (IsColorApproximatelyMain(pixels[i]))
            {
                pixels[i] = colors[colorIndex];
            }
            else if (IsColorApproximatelyAccent(pixels[i]))
            {
                pixels[i] = colors[colorIndex] * .9f;
            }
        }

        newTex.SetPixels(0, 0, newTex.width, newTex.height, pixels);

        newTex.filterMode = FilterMode.Point;
        newTex.Compress(false);
        newTex.Apply();

        Sprite newSprite = Sprite.Create(newTex,
            new Rect(0, 0, newTex.width, newTex.height),
            sprite.pivot / new Vector2(cardRect.width, cardRect.height),
            sprite.pixelsPerUnit);

        return newSprite;
    }

    private bool IsColorApproximatelyMain(Color color)
    {
        float tolerance = 0.01f; 
        return Mathf.Abs(color.r - 1f) < tolerance
               && Mathf.Abs(color.g - 1f) < tolerance
               && Mathf.Abs(color.b - 1f) < tolerance
               && color.a > 0.9f; 
    }

    private bool IsColorApproximatelyAccent(Color color)
    {
        float tolerance = 0.01f;
        float target = 191f / 255f; 
        return Mathf.Abs(color.r - target) < tolerance
               && Mathf.Abs(color.g - target) < tolerance
               && Mathf.Abs(color.b - target) < tolerance
               && color.a > 0.9f;
    }

    public bool AddIconStartIndex()
    {
        int index = iconStartIndex;

        iconStartIndex++;

        int maxStartIndex = (iconSprites.Length / 12) - 1;

        if (iconStartIndex > maxStartIndex)
        {
            iconStartIndex = maxStartIndex;
        }

        settings.iconStartIndex = iconStartIndex;

        if (index == iconStartIndex)
        {
            return false;
        }

        return true;
    }

    public bool RemoveIconStartIndex()
    {
        int index = iconStartIndex;

        iconStartIndex--;

        if (iconStartIndex < 0)
        {
            iconStartIndex = 0;
        }

        settings.iconStartIndex = iconStartIndex;

        if (index == iconStartIndex)
        {
            return false;
        }

        return true;
    }

    public bool AddColorStartIndex()
    {
        int index = colorStartIndex;

        colorStartIndex++;

        int maxStartIndex = (colors.Length / 6) - 1;

        if (colorStartIndex > maxStartIndex)
        {
            colorStartIndex = maxStartIndex;
        }

        settings.colorStartIndex = colorStartIndex;

        if (index == colorStartIndex)
        {
            return false;
        }

        return true;
    }

    public bool RemoveColorStartIndex()
    {
        int index = colorStartIndex;

        colorStartIndex--;

        if (colorStartIndex < 0)
        {
            colorStartIndex = 0;
        }

        settings.colorStartIndex = colorStartIndex;

        if (index == colorStartIndex)
        {
            return false;
        }

        return true;
    }

    public int GetIconIndex()
    {
        return iconStartIndex;
    }

    public int GetColorIndex()
    {
        return colorStartIndex;
    }
}