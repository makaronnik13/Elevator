using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorsView : MonoBehaviour
{
    public GameObject FloorPanelPrefab;
    public Transform FloorPanelsContent;
    private List<FloorPanel> _panels = new List<FloorPanel>();

	public void GenerateFloorButtons(int floors, Action<int, FloorPanel.Direction> onFloorPanelClicked)
    {
        for (int i = 0; i<floors;i++)
        {
            FloorPanel newPanel = Instantiate(FloorPanelPrefab, Vector3.zero, Quaternion.identity, FloorPanelsContent).GetComponent<FloorPanel>();
            newPanel.InitFloor(i, onFloorPanelClicked, i == floors - 1, i == 0);
            _panels.Add(newPanel);
        }
    }

    public void ResetButton(int i, FloorPanel.Direction direction)
    {
        _panels[i].ResetButton(direction);
    }
}
