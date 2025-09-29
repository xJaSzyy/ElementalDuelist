using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float duration = 0.05f;
    [SerializeField] private float scaleFactor = 1.1f;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Анимация плавного увеличения с помощью LeanTween
        LeanTween.scale(gameObject, originalScale * scaleFactor, duration).setEaseInOutQuad();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Анимация плавного возврата к исходному масштабу
        LeanTween.scale(gameObject, originalScale, duration).setEaseInOutQuad();
    }
}
