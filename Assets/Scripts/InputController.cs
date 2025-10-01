using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private List<CustomHorizontalGroup> customGroups;
    [SerializeField] private int selectedCustomGroupIndex = -1;
    [SerializeField] private int selectedCustomButtonIndex = 0;

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
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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
                if (selectedCustomButtonIndex > customGroups[selectedCustomGroupIndex].gameObjects.Count - 1)
                {
                    selectedCustomButtonIndex = customGroups[selectedCustomGroupIndex].gameObjects.Count - 1;
                }
                UpdateCustomGroups();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                customGroups[selectedCustomGroupIndex].gameObjects[selectedCustomButtonIndex].GetComponent<ICustomSelectable>().Press();
            }
            
            if (Input.GetKeyUp(KeyCode.Return))
            {
                customGroups[selectedCustomGroupIndex].gameObjects[selectedCustomButtonIndex].GetComponent<ICustomSelectable>().Release();
                customGroups[selectedCustomGroupIndex].gameObjects[selectedCustomButtonIndex].GetComponent<ICustomSelectable>().Click();
            }
        }
    }

    private void UpdateCustomGroups()
    {
        if (selectedCustomGroupIndex != -1 && customGroups[selectedCustomGroupIndex].gameObjects.Count - 1 < selectedCustomButtonIndex)
        {
            selectedCustomButtonIndex = 0;
        }

        for (int i = 0; i < customGroups.Count; i++)
        {
            customGroups[i].DeselectAll();

            if (i == selectedCustomGroupIndex)
            {
                customGroups[i].gameObjects[selectedCustomButtonIndex].GetComponent<ICustomSelectable>().Select(true);
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

    public void Reselect(ICustomSelectable selectable)
    {
        for (int i = 0; i < customGroups.Count; i++)
        {
            for (int j = 0; j < customGroups[i].gameObjects.Count; j++)
            {
                if (customGroups[i].gameObjects[j].GetComponent<ICustomSelectable>() == selectable)
                {
                    selectedCustomGroupIndex = i;
                    selectedCustomButtonIndex = j;
                    UpdateCustomGroups();

                    return;
                }
            }
        }
    }
}

[Serializable]
public class CustomHorizontalGroup
{
    public List<GameObject> gameObjects;

    public void DeselectAll()
    {
        foreach (GameObject go in gameObjects)
        {
            go.GetComponent<ICustomSelectable>().Deselect();
        }
    }
}