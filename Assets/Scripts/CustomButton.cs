using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float duration = 0.05f;
    [SerializeField] private float scaleFactor = 1.1f;
    [SerializeField] private float pressScaleFactor = 0.9f;
    [SerializeField] private Button button;
    
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Press();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
        if (RectTransformUtility.RectangleContainsScreenPoint(button.transform as RectTransform, eventData.position, eventData.pressEventCamera))
        {
            Click();
        }
    }

    public void Select(bool keyboard = false)
    {
        LeanTween.scale(gameObject, originalScale * scaleFactor, duration).setEaseInOutQuad();
        button.GetComponent<Image>().color = new Color32(212, 193, 105, 255);

        if (!keyboard)
        {
            InputController inputController = FindAnyObjectByType<InputController>();
            if (inputController != null)
            {
                inputController.Reselect(this);
            }
        }
    }

    public void Deselect()
    {
        LeanTween.scale(gameObject, originalScale, duration).setEaseInOutQuad();
        button.GetComponent<Image>().color = Color.white;
    }

    public void Press()
    {
        LeanTween.scale(gameObject, originalScale * pressScaleFactor, duration).setEaseInOutQuad();
    }

    public void Release()
    {
        LeanTween.scale(gameObject, originalScale * scaleFactor, duration).setEaseInOutQuad();
    }

    public void Click()
    {
        button.onClick.Invoke();
    }
}