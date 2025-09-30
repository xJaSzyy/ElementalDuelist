using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private float duration = .5f;

    [Header("Buttons")]
    [SerializeField] private RectTransform buttons;
    [SerializeField] private RectTransform loadingScreen;
    [SerializeField] private Button playButton;
    [SerializeField] private Button customizeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Profile Panel")]
    [SerializeField] private RectTransform customizePanel;
    [SerializeField] private Button backCustomizeButton;
    
    [Header("Settings Panel")]
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private Button backSettingsButton;

    private Vector2 rightScreenPosition;
    private Vector2 leftScreenPosition;
    private Vector2 onScreenPosition;

    private void Awake()
    {
        buttons.gameObject.SetActive(true);
        customizePanel.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(false);

        onScreenPosition = customizePanel.anchoredPosition;
        leftScreenPosition = onScreenPosition - new Vector2(Screen.width, 0);
        rightScreenPosition = onScreenPosition + new Vector2(Screen.width, 0);

        customizePanel.anchoredPosition = rightScreenPosition;
        settingsPanel.anchoredPosition = rightScreenPosition;
        loadingScreen.anchoredPosition = rightScreenPosition;
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(PlayButton_Click);
        customizeButton.onClick.AddListener(CustomizeButton_Click);
        settingsButton.onClick.AddListener(SettingsButton_Click);
        exitButton.onClick.AddListener(ExitButton_Click);

        backCustomizeButton.onClick.AddListener(BackCustomizeButton_Click);
        backSettingsButton.onClick.AddListener(BackSettingsButton_Click);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(PlayButton_Click);
        customizeButton.onClick.RemoveListener(CustomizeButton_Click);
        settingsButton.onClick.RemoveListener(SettingsButton_Click);
        exitButton.onClick.RemoveListener(ExitButton_Click);

        backCustomizeButton.onClick.RemoveListener(BackCustomizeButton_Click);
        backSettingsButton.onClick.RemoveListener(BackSettingsButton_Click);
    }

    private void PlayButton_Click()
    {
        loadingScreen.gameObject.SetActive(true);
        LeanTween.move(buttons, leftScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.move(loadingScreen, onScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            SceneManager.LoadScene("GameScene");
        });
    }

    private void CustomizeButton_Click()
    {
        SlideIn(customizePanel);
    }

    private void SettingsButton_Click()
    {
        SlideIn(settingsPanel);
    }

    private void ExitButton_Click()
    {
        Application.Quit();
    }

    private void BackCustomizeButton_Click()
    {
        SlideOut(customizePanel);
    }

    private void BackSettingsButton_Click()
    {
        SlideOut(settingsPanel);
    }

    private void SlideIn(RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        LeanTween.move(buttons, leftScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.move(rect, onScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
    }

    private void SlideOut(RectTransform rect)
    {
        LeanTween.move(buttons, onScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.move(rect, rightScreenPosition, duration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            rect.gameObject.SetActive(false);
        });
    }
}
