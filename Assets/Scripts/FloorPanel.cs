using System;
using UnityEngine;
using UnityEngine.UI;

public class FloorPanel : MonoBehaviour
{

    public enum Direction
    {
        Up,
        Down
    }

    public Text FloorNumber;
    public Button UpButton;
    public Button DownButton;
    public GameObject HumanPrefab;
    public Transform WaitingArea;

    private bool __downPressed;
    private bool _downPressed
    {
        get
        {
            return __downPressed;
        }
        set
        {
            __downPressed = value;
            if (__downPressed)
            {
                _downButtonImage.color = Color.green;
            }
            else
            {
                _downButtonImage.color = Color.white;
            }
        }
    }
    private bool __upPressed;
    private bool _upPressed
    {
        get
        {
            return __upPressed;
        }
        set
        {
            __upPressed = value;
            if (__upPressed)
            {
                _upButtonImage.color = Color.green;
            }
            else
            {
                _upButtonImage.color = Color.white;
            }
        }
    }
    private Image __upButtonImage;
    private Image _upButtonImage
    {
        get
        {
            if (!__upButtonImage)
            {
                __upButtonImage = UpButton.targetGraphic as Image;
            }
            return __upButtonImage;
        }
    }
    private Image __downButtonImage;
    private Image _downButtonImage
    {
        get
        {
            if (!__downButtonImage)
            {
                __downButtonImage = DownButton.targetGraphic as Image;
            }
            return __downButtonImage;
        }
    }

    public void InitFloor(int number, Action<int, Direction> onFloorPanelClicked, bool hideUp = false, bool hideDown = false)
    {
        if (hideUp)
        {
            UpButton.gameObject.SetActive(false);
        }
        if (hideDown)
        {
            DownButton.gameObject.SetActive(false);
        }

        FloorNumber.text = (number+1).ToString();
        UpButton.onClick.AddListener(() =>
        {
            if (!_upPressed)
            {
                CreateHuman();
                onFloorPanelClicked(number, Direction.Up);  
                _upPressed = true;
            }
        });

        DownButton.onClick.AddListener(() =>
        {
            if (!_downPressed)
            {
                CreateHuman();
                onFloorPanelClicked(number, Direction.Down);    
                _downPressed = true;
            }
        });
        //_downButtonImage.alphaHitTestMinimumThreshold = 0.1f;
        //_upButtonImage.alphaHitTestMinimumThreshold = 0.1f;
    }
    public void CreateHuman()
    {
        if (WaitingArea.transform.childCount < 2)
        {
            GameObject newHuman = Instantiate(HumanPrefab, Vector3.zero, Quaternion.identity, WaitingArea);
        }
    }
    public void ResetButton(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:       
                _downPressed = false;
                break;
            case Direction.Up:
                _upPressed = false;
                break;
        }   
    }
}
