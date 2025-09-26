using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private IconSettuper iconSettuper;
    [SerializeField] private Sprite cleanCardSprite;
    [SerializeField] private List<Sprite> colorSprites;

    [SerializeField] private Image iconPreview;
    [SerializeField] private Image colorPreview;

    [SerializeField] private Button nextIconButton;
    [SerializeField] private Button prevIconButton;
    
    [SerializeField] private Button nextColorButton;
    [SerializeField] private Button prevColorButton;

    [SerializeField] private float amplitude = 50f;
    [SerializeField] private float duration = 2f; 

    private readonly CardElement[] elementsOrder = new CardElement[]
    {
        CardElement.Fire, CardElement.Water, CardElement.Energy,
        CardElement.Earth, CardElement.Nature, CardElement.Magic
    };

    private int currentIndex = 0;
    private CardElement currentElement = CardElement.Fire;
    private Vector3 startIconPostion;

    private void OnEnable()
    {
        nextIconButton.onClick.AddListener(NextIconButton_Click);
        prevIconButton.onClick.AddListener(PrevIconButton_Click);
        nextColorButton.onClick.AddListener(NextColorButton_Click);
        prevColorButton.onClick.AddListener(PrevColorButton_Click);

        startIconPostion = iconPreview.transform.position;

        LeanTween.moveY(iconPreview.gameObject, iconPreview.transform.position.y + amplitude, duration)
                 .setEaseInOutSine() 
                 .setLoopPingPong();

        colorPreview.sprite = colorSprites[iconSettuper.GetColorIndex()];
        StartCoroutine(ChangeIconPreview());
    }

    private void OnDisable()
    {
        nextIconButton.onClick.RemoveListener(NextIconButton_Click);
        prevIconButton.onClick.RemoveListener(PrevIconButton_Click);
        nextColorButton.onClick.RemoveListener(NextColorButton_Click);
        prevColorButton.onClick.RemoveListener(PrevColorButton_Click);

        LeanTween.cancel(iconPreview.gameObject);

        iconPreview.transform.position = startIconPostion;

        StopAllCoroutines();
    }

    private void NextIconButton_Click()
    {
        if (iconSettuper.AddIconStartIndex())
        {
            StopAllCoroutines();
            StartCoroutine(ChangeIconPreview());
        }
    }

    private void PrevIconButton_Click()
    {
        if (iconSettuper.RemoveIconStartIndex())
        {
            StopAllCoroutines();
            StartCoroutine(ChangeIconPreview());
        }
    }

    private void NextColorButton_Click()
    {
        if (iconSettuper.AddColorStartIndex())
        {
            StopAllCoroutines();
            colorPreview.sprite = colorSprites[iconSettuper.GetColorIndex()];
            StartCoroutine(ChangeIconPreview());
        }
    }

    private void PrevColorButton_Click()
    {
        if (iconSettuper.RemoveColorStartIndex())
        {
            StopAllCoroutines();
            colorPreview.sprite = colorSprites[iconSettuper.GetColorIndex()];
            StartCoroutine(ChangeIconPreview());
        }
    }

    IEnumerator ChangeIconPreview()
    {
        while (true)
        {
            currentElement = elementsOrder[currentIndex];
            iconPreview.sprite = iconSettuper.GenerateSprite(cleanCardSprite, currentElement);

            currentIndex = (currentIndex + 1) % elementsOrder.Length;

            yield return new WaitForSeconds(1f);
        }
    }
}
