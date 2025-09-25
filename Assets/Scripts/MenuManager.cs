using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button profileButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        playButton.onClick.AddListener(PlayButton_Click);
        profileButton.onClick.AddListener(ProfileButton_Click);
        settingsButton.onClick.AddListener(SettingsButton_Click);
        exitButton.onClick.AddListener(ExitButton_Click);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(PlayButton_Click);
        profileButton.onClick.RemoveListener(ProfileButton_Click);
        settingsButton.onClick.RemoveListener(SettingsButton_Click);
        exitButton.onClick.RemoveListener(ExitButton_Click);
    }

    private void PlayButton_Click()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void ProfileButton_Click()
    {
        Debug.Log("Open profile");
    }

    private void SettingsButton_Click()
    {

        Debug.Log("Open settings");
    }

    private void ExitButton_Click()
    {
        Application.Quit();
    }
}
