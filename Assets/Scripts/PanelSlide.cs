using UnityEngine;
using UnityEngine.UI;

public class PanelSlide : MonoBehaviour
{
    [SerializeField] private RectTransform customizePanel;
    [SerializeField] private Button openCustomizeButton;
    [SerializeField] private Button closeCustomizeButton;
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private RectTransform buttons;

    private Vector2 rightScreenPosition;
    private Vector2 leftScreenPosition;
    private Vector2 onScreenPosition;

    private void OpenCustomize()
    {
        customizePanel.gameObject.SetActive(true);
        LeanTween.move(buttons, leftScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.move(customizePanel, onScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
    }

    private void CloseCustomize()
    {
        LeanTween.move(buttons, onScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.move(customizePanel, rightScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            customizePanel.gameObject.SetActive(false);
        });
    }

    private void OpenSettings()
    {
        settingsPanel.gameObject.SetActive(true);
        LeanTween.move(buttons, leftScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.move(settingsPanel, onScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
    }

    private void CloseSettings()
    {
        LeanTween.move(buttons, onScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.move(settingsPanel, rightScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            settingsPanel.gameObject.SetActive(false);
        });
    }
}
