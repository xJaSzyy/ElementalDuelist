using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button playButton;
    [SerializeField] private Button profileButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Profile Panel")]
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private Button backProfileButton;
    
    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button backSettingsButton;

    private void Awake()
    {
        buttons.SetActive(true);
        profilePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(PlayButton_Click);
        profileButton.onClick.AddListener(ProfileButton_Click);
        settingsButton.onClick.AddListener(SettingsButton_Click);
        exitButton.onClick.AddListener(ExitButton_Click);

        backProfileButton.onClick.AddListener(BackButton_Click);

        backSettingsButton.onClick.AddListener(BackButton_Click);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(PlayButton_Click);
        profileButton.onClick.RemoveListener(ProfileButton_Click);
        settingsButton.onClick.RemoveListener(SettingsButton_Click);
        exitButton.onClick.RemoveListener(ExitButton_Click);

        backProfileButton.onClick.RemoveListener(BackButton_Click);

        backSettingsButton.onClick.RemoveListener(BackButton_Click);
    }

    private void PlayButton_Click()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void ProfileButton_Click()
    {
        buttons.SetActive(false);
        profilePanel.SetActive(true);
    }

    private void SettingsButton_Click()
    {
        buttons.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void ExitButton_Click()
    {
        Application.Quit();
    }

    private void BackButton_Click()
    {
        settingsPanel.SetActive(false);
        profilePanel.SetActive(false);
        buttons.SetActive(true);
    }
}
