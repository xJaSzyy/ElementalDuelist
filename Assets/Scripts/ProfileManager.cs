using Assets.Scripts.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private IconSettuper iconSettuper;
    [SerializeField] private Sprite cleanCardSprite;

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

    private void OnEnable()
    {
        nextIconButton.onClick.AddListener(NextIconButton_Click);
        prevIconButton.onClick.AddListener(PrevIconButton_Click);
        nextColorButton.onClick.AddListener(NextColorButton_Click);
        prevColorButton.onClick.AddListener(PrevColorButton_Click);

        LeanTween.moveY(iconPreview.gameObject, transform.position.y + amplitude, duration)
                 .setEaseInOutSine() 
                 .setLoopPingPong();

        StartCoroutine(ChangeIconPreview());
    }

    private void OnDisable()
    {
        nextIconButton.onClick.RemoveListener(NextIconButton_Click);
        prevIconButton.onClick.RemoveListener(PrevIconButton_Click);
        nextColorButton.onClick.RemoveListener(NextColorButton_Click);
        prevColorButton.onClick.RemoveListener(PrevColorButton_Click);

        LeanTween.cancel(gameObject);

        StopAllCoroutines();
    }

    private void NextIconButton_Click()
    {
        StopAllCoroutines();
        iconSettuper.AddIconStartIndex();
        StartCoroutine(ChangeIconPreview());
    }

    private void PrevIconButton_Click()
    {
        StopAllCoroutines();
        iconSettuper.RemoveIconStartIndex();
        StartCoroutine(ChangeIconPreview());
    }

    private void NextColorButton_Click()
    {
        StopAllCoroutines();
        iconSettuper.AddColorStartIndex();
        StartCoroutine(ChangeIconPreview());
    }

    private void PrevColorButton_Click()
    {
        StopAllCoroutines();
        iconSettuper.RemoveColorStartIndex();
        StartCoroutine(ChangeIconPreview());
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
