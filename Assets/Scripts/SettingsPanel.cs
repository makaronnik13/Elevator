using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Elevator Elevator;
    public InputField FloorsInput;
    public Button StartButton;

    private void Start()
    {
        StartButton.onClick.AddListener(StartGame);
        StartButton.interactable = false;
        FloorsInput.onValueChanged.AddListener(OnInputChanged);
    }

    private void OnInputChanged(string text)
    {
        int r;
        StartButton.interactable = int.TryParse(text, out r) && r>0;
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
        Elevator.Launch(int.Parse(FloorsInput.text));
    }

}
