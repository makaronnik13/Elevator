using System;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorButton : MonoBehaviour
{
    private Text __floorText;
    private Text _floorText
    {
        get
        {
            if (!__floorText)
            {
                __floorText = GetComponentInChildren<Text>();
            }
            return __floorText;
        }
    }


    public void InitButton(int floor, Action<int> elevatorButtonClicked)
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        _floorText.text = (floor+1).ToString();
        GetComponent<Button>().onClick.AddListener(() => 
        {
            elevatorButtonClicked(floor);
            _floorText.color = Color.green;
        });
    }



    public void ResetButton()
    {
        _floorText.color = Color.white;
    }
}