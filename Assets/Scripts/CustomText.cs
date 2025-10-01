using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomText : MonoBehaviour
{
    [SerializeField] private Image[] slots;
    [SerializeField] private Sprite[] numbers;
    [SerializeField] private Sprite[] letters;
    [SerializeField] private Sprite colon;
    [SerializeField] private Sprite error;

    public void SetText(string text, Color32 color)
    {
        foreach (Image slot in slots)
        {
            slot.enabled = true;
            slot.color = Color.white;
        }

        int slotIndex = 0;

        foreach (char c in text)
        {
            Sprite sprite = error;
            if (char.IsDigit(c))
            {
                sprite = numbers[Convert.ToInt32(c.ToString())];
                
            }
            else if (char.IsLetter(c))
            {
                int letterIndex = char.ToUpper(c) - 'A';
                sprite = letters[letterIndex];
            }
            else if (c == ':')
            {
                sprite = colon;
            }
            else if (c == ' ')
            {
                slots[slotIndex].enabled = false;
                slotIndex++;
                continue;
            }

            slots[slotIndex].sprite = sprite;
            slots[slotIndex].color = color;
            slots[slotIndex].SetNativeSize();
            slotIndex++;
        }

        while (slotIndex < slots.Length)
        {
            slots[slotIndex].enabled = false;
            slotIndex++;
        }

        float totalWidth = 0f;
        int length = 0;

        foreach (var slot in slots)
        {
            if (slot.enabled)
            {
                RectTransform rt = slot.GetComponent<RectTransform>();
                totalWidth += rt.rect.width;
                length++;
            }
        }

        float gapsWidth = 8f * (length - 1);
        float totalWidthWithGaps = totalWidth + gapsWidth;

        RectTransform parent = slots[0].transform.parent.GetComponent<RectTransform>();
        parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidthWithGaps);
    }
}
