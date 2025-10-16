using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Resolution")]
    [SerializeField] private Button toggleFullscreenButton;
    [SerializeField] private Sprite toggleOnSprite;
    [SerializeField] private Sprite toggleOffSprite;
    [SerializeField] private Button nextResolutionButton;
    [SerializeField] private Button prevResolutionButton;
    [SerializeField] private CustomText resolutionText;
    [SerializeField] private RectTransform resolutionPanel;

    private Resolution[] resolutions;
    private int resolutionIndex = 0;

    private void Awake()
    {
        resolutions = Screen.resolutions;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                resolutionIndex = i;
                break;
            }
        }

        if (Screen.fullScreen)
        {
            toggleFullscreenButton.GetComponent<Image>().sprite = toggleOnSprite;
        }
        else
        {
            toggleFullscreenButton.GetComponent<Image>().sprite = toggleOffSprite;
        }

        UpdateResolution();
    }

    private void OnEnable()
    {
        nextResolutionButton.onClick.AddListener(NextResolutionButton_Click);
        prevResolutionButton.onClick.AddListener(PrevResolutionButton_Click);
        toggleFullscreenButton.onClick.AddListener(ToggleFullscreenButton_Click);
    }

    private void OnDisable()
    {
        nextResolutionButton.onClick.RemoveAllListeners();
        prevResolutionButton.onClick.RemoveAllListeners();
        toggleFullscreenButton.onClick.RemoveAllListeners();
    }

    private void NextResolutionButton_Click()
    {
        resolutionIndex++;
        if (resolutionIndex >= resolutions.Length)
        {
            resolutionIndex = 0;
        }
        UpdateResolution();
    }

    private void PrevResolutionButton_Click()
    {
        resolutionIndex--;
        if (resolutionIndex < 0)
        {
            resolutionIndex = resolutions.Length - 1;
        }
        UpdateResolution();
    }

    private void UpdateResolution()
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        resolutionText.SetText(res.width + "x" + res.height, Color.white);

        UpdatePanelWidth();
    }

    private void UpdatePanelWidth()
    {
        int childCount = resolutionPanel.childCount;
        float totalWidth = 0f;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = resolutionPanel.GetChild(i) as RectTransform;
            if (child != null)
            {
                totalWidth += child.rect.width;
            }
        }

        float totalSpacing = 32f * (childCount - 1);

        float width = totalWidth + totalSpacing;

        Vector2 size = resolutionPanel.sizeDelta;
        size.x = width;
        resolutionPanel.sizeDelta = size;
    }

    private void ToggleFullscreenButton_Click()
    {
        Screen.fullScreen = !Screen.fullScreen;

        if (Screen.fullScreen)
        {
            toggleFullscreenButton.GetComponent<Image>().sprite = toggleOffSprite;
        }
        else
        {
            toggleFullscreenButton.GetComponent<Image>().sprite = toggleOnSprite;
        }
    }
}
