using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElevatorShaftView : MonoBehaviour
{
    public RectTransform Cabin;
    public GameObject ShaftPanelPrefab;
    public RectTransform FloorsParent;

    private float __floorSize = 0;
    private float _floorSize
    {
        get
        {
            if (__floorSize == 0)
            {
                __floorSize = ShaftPanelPrefab.GetComponent<RectTransform>().sizeDelta.y;
            }
            return __floorSize;
        }
    }
    private List<FloorPanel> _panels = new List<FloorPanel>();
    private Vector2 _pointPosition;

    public void Init(int floors, Action<int, FloorPanel.Direction> onFloorPanelClicked)
    {
        for(int i = 0; i<floors; i++)
        {
            FloorPanel newPanel = Instantiate(ShaftPanelPrefab, Vector3.zero, Quaternion.identity, FloorsParent).GetComponent<FloorPanel>();
            newPanel.InitFloor(i, onFloorPanelClicked, i == floors - 1, i == 0);
            _panels.Add(newPanel);
            newPanel.transform.SetAsFirstSibling();
        }
        Cabin.SetAsLastSibling();
        Cabin.sizeDelta = _floorSize * Vector2.one;
        ChangePosition(0);
        FloorsParent.localPosition = Vector2.up*floors*_floorSize;
    }
    public void ChangePosition(float v)
    {        
        Cabin.anchoredPosition = new Vector2(Cabin.anchoredPosition.x, _floorSize*(v+0.5f));
    }
    public void ResetButton(int i, FloorPanel.Direction direction)
    {
        _panels[i].ResetButton(direction);
    }

    public void PlaceHumanInside(Transform humanTransform)
    {
        humanTransform.transform.SetParent(Cabin);
    }

    public void SetCabinState(bool v)
    {
        Cabin.GetComponent<Animator>().SetBool("Open", v);
    }
}
