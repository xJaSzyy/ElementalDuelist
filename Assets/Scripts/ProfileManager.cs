using Assets.Scripts.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private IconSettuper iconSettuper;
    [SerializeField] private Sprite cleanCardSprite;
    [SerializeField] private Image iconPreview;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

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
        nextButton.onClick.AddListener(NextButton_Click);
        prevButton.onClick.AddListener(PrevButton_Click);

        LeanTween.moveY(iconPreview.gameObject, transform.position.y + amplitude, duration)
                 .setEaseInOutSine() 
                 .setLoopPingPong();

        StartCoroutine(ChangeIconPreview());
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(NextButton_Click);
        prevButton.onClick.RemoveListener(PrevButton_Click);

        LeanTween.cancel(gameObject);

        StopAllCoroutines();
    }

    private void NextButton_Click()
    {
        StopAllCoroutines();
        iconSettuper.AddIconStartIndex();
        StartCoroutine(ChangeIconPreview());
    }

    private void PrevButton_Click()
    {
        StopAllCoroutines();
        iconSettuper.RemoveIconStartIndex();
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
