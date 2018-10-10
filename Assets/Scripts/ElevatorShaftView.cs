using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElevatorShaftView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform Cabin;

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
    public GameObject ShaftPanelPrefab;
    public RectTransform FloorsParent;

    private bool moving = false;
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
        /*
        if (!moving)
        {
            Vector2 needPosition = new Vector2(transform.localPosition.x, Cabin.sizeDelta.y  - Screen.height / 2f - Cabin.anchoredPosition.y + GetComponent<RectTransform>().sizeDelta.y / 2f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, needPosition, Time.deltaTime * 3f);
        }*/
    }

    public void OnDrag(PointerEventData eventData)
    {      
        transform.localPosition += Vector3.up * Mathf.Clamp(eventData.delta.y, -GetComponent<RectTransform>().rect.height/2, GetComponent<RectTransform>().rect.height/2);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StopCoroutine(ShowCabin(2f));
        moving = true;
        _pointPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(ShowCabin(2f));
    }

    private IEnumerator ShowCabin(float v)
    {
        yield return new WaitForSeconds(v);
        moving = false;
    }

    public void ResetButton(int i, FloorPanel.Direction direction)
    {
        _panels[i].ResetButton(direction);
    }
}
