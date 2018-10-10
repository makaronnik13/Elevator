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
            onFloorPanelClicked(number, Direction.Up);
            GetButtonImageByDirection(Direction.Up).color = Color.green;
        });

        DownButton.onClick.AddListener(() =>
        {
            onFloorPanelClicked(number, Direction.Down);
            GetButtonImageByDirection(Direction.Down).color = Color.green;
        });

        _downButtonImage.alphaHitTestMinimumThreshold = 0.1f;
        _upButtonImage.alphaHitTestMinimumThreshold = 0.1f;
    }


    public void ResetButton(Direction direction)
    {
        GetButtonImageByDirection(direction).color = Color.white;
    }

    public void ResetDirections()
    {
        ResetButton(Direction.Up);
        ResetButton(Direction.Down);
    }

    private Image GetButtonImageByDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return _upButtonImage;
            case Direction.Down:
                return _downButtonImage;
        }

        return null;
    }
}
