using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Resolution")]
    [SerializeField] private Button nextResolutionButton;
    [SerializeField] private Button prevResolutionButton;

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
    }

    private void OnEnable()
    {
        nextResolutionButton.onClick.AddListener(NextResolutionButton_Click);
        prevResolutionButton.onClick.AddListener(PrevResolutionButton_Click);
    }

    private void OnDisable()
    {
        nextResolutionButton.onClick.RemoveAllListeners();
        prevResolutionButton.onClick.RemoveAllListeners();
    }

    private void NextResolutionButton_Click()
    {
        resolutionIndex++;
        if (resolutionIndex >= resolutions.Length)
        {
            resolutionIndex = 0;
        }
        SetResolution(resolutionIndex);
    }

    private void PrevResolutionButton_Click()
    {
        resolutionIndex--;
        if (resolutionIndex < 0)
        {
            resolutionIndex = resolutions.Length - 1;
        }
        SetResolution(resolutionIndex);
    }

    private void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        Debug.Log("Set resolution: " + res.width + "x" + res.height);
    }
}
