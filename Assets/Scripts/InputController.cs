using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    [SerializeField] private CustomButton[] buttons;
    private int selectedIndex = 0;

    private enum InputMode { Keyboard, Mouse }
    private InputMode currentInputMode = InputMode.Keyboard;

    private Vector3 lastMousePosition;
    private bool hasClearedSelection = false;

    private void Start()
    {
        UpdateButtons();
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
                    selectedIndex = -1;  
                    UpdateButtons();
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
                    selectedIndex = -1;  
                    UpdateButtons();
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
                if (selectedIndex == -1)
                {
                    selectedIndex = 0;
                }
                UpdateButtons();
            }
        }

        if (currentInputMode == InputMode.Keyboard)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex = 0;
                }
                UpdateButtons();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex++;
                if (selectedIndex > buttons.Length - 1)
                {
                    selectedIndex = buttons.Length - 1;
                }
                UpdateButtons();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                buttons[selectedIndex].Press();
            }
            
            if (Input.GetKeyUp(KeyCode.Return))
            {
                buttons[selectedIndex].Release();
                buttons[selectedIndex].Click();
            }
        }
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedIndex)
            {
                buttons[i].Select();
            }
            else
            {
                buttons[i].Deselect();
            }
        }
    }

    public void SetButtons(CustomButton[] newButtons)
    {
        buttons = new CustomButton[newButtons.Length];
        for (int i = 0; i < newButtons.Length; i++)
        {
            buttons[i] = newButtons[i];
        }
        selectedIndex = 0;

        if (currentInputMode == InputMode.Mouse)
        {
            selectedIndex = -1;
            hasClearedSelection = true;
        }

        UpdateButtons();
    }
}
