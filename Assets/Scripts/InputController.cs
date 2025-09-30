using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private List<CustomHorizontalGroup> customGroups;
    private int selectedCustomGroupIndex = -1;
    private int selectedCustomButtonIndex = 0;

    
    private InputMode currentInputMode = InputMode.Keyboard;

    private Vector3 lastMousePosition;
    private bool hasClearedSelection = false;

    private void Start()
    {
        UpdateCustomGroups();
        lastMousePosition = Input.mousePosition;
    }

    private void Update()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            if (currentInputMode != InputMode.Mouse)
            {
                currentInputMode = InputMode.Mouse;
                if (!hasClearedSelection)
                {
                    selectedCustomGroupIndex = -1;  
                    UpdateCustomGroups();
                    hasClearedSelection = true;
                }
            }
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (currentInputMode != InputMode.Mouse)
            {
                currentInputMode = InputMode.Mouse;
                if (!hasClearedSelection)
                {
                    selectedCustomGroupIndex = -1;  
                    UpdateCustomGroups();
                    hasClearedSelection = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentInputMode != InputMode.Keyboard)
            {
                currentInputMode = InputMode.Keyboard;
                hasClearedSelection = false;
                if (selectedCustomGroupIndex == -1)
                {
                    selectedCustomGroupIndex = 0;
                }
                UpdateCustomGroups();
            }
        }

        if (currentInputMode == InputMode.Keyboard)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedCustomGroupIndex--;
                if (selectedCustomGroupIndex < 0)
                {
                    selectedCustomGroupIndex = 0;
                }
                UpdateCustomGroups();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedCustomGroupIndex++;
                if (selectedCustomGroupIndex > customGroups.Count - 1)
                {
                    selectedCustomGroupIndex = customGroups.Count - 1;
                }
                UpdateCustomGroups();
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedCustomButtonIndex--;
                if (selectedCustomButtonIndex < 0)
                {
                    selectedCustomButtonIndex = 0;
                }
                UpdateCustomGroups();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedCustomButtonIndex++;
                if (selectedCustomButtonIndex > customGroups[selectedCustomGroupIndex].buttons.Count - 1)
                {
                    selectedCustomButtonIndex = customGroups[selectedCustomGroupIndex].buttons.Count - 1;
                }
                UpdateCustomGroups();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                customGroups[selectedCustomGroupIndex].buttons[selectedCustomButtonIndex].Press();
            }
            
            if (Input.GetKeyUp(KeyCode.Return))
            {
                customGroups[selectedCustomGroupIndex].buttons[selectedCustomButtonIndex].Release();
                customGroups[selectedCustomGroupIndex].buttons[selectedCustomButtonIndex].Click();
            }
        }
    }

    private void UpdateCustomGroups()
    {
        if (selectedCustomGroupIndex != -1 && customGroups[selectedCustomGroupIndex].buttons.Count - 1 < selectedCustomButtonIndex)
        {
            selectedCustomButtonIndex = 0;
        }

        for (int i = 0; i < customGroups.Count; i++)
        {
            customGroups[i].DeselectAll();

            if (i == selectedCustomGroupIndex)
            {
                customGroups[i].buttons[selectedCustomButtonIndex].Select();
            }
        }
    }

    public void SetCustomHorizontalGroups(List<CustomHorizontalGroup> newCustomGroups)
    {
        customGroups = newCustomGroups;

        selectedCustomGroupIndex = 0;

        if (currentInputMode == InputMode.Mouse)
        {
            selectedCustomGroupIndex = -1;
            hasClearedSelection = true;
        }

        UpdateCustomGroups();
    }
}

[Serializable]
public class CustomHorizontalGroup
{
    public List<CustomButton> buttons;

    public void DeselectAll()
    {
        foreach (CustomButton button in buttons)
        {
            button.Deselect();
        }
    }
}