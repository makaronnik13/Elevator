using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorView : MonoBehaviour
{
    public Text FloorText;
    public Transform ButtonsContent;
    public GameObject ButtonPrefab;
    public GameObject ShadowMask;

    private List<ElevatorButton> _buttons = new List<ElevatorButton>();

    public bool LightUpButtons
    {
        set
        {
            ShadowMask.SetActive(!value);
        }
    }

    public void GenerateElevatorButtons(int floors, Action<int> elevatorButtonClicked)
    {
        for (int i = 0; i < floors; i++)
        {
            ElevatorButton newButton = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity, ButtonsContent).GetComponent<ElevatorButton>();
            newButton.InitButton(i, elevatorButtonClicked);
            _buttons.Add(newButton);
        }
    }

    public void ResetButton(int i)
    {
        _buttons[i].ResetButton();
    }

    public void SetFloor(float position)
    {
        FloorText.text = (Mathf.RoundToInt(position)+1).ToString();
    }

    public void OnStopButtonPressed()
    {
        GetComponentInParent<Elevator>().Behaviour.Pause();
    }
}
