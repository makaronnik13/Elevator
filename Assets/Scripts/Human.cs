using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Human : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private bool _inCabin = false;
    public Sprite[] variants;
    private RectTransform _dragingFrom;
    private bool _dragging = false;

    void Start()
    {
        GetComponent<Image>().sprite = variants[Random.Range(0,variants.Length)];
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_dragging)
        {
            return;
        }
       
        transform.position +=  new Vector3(eventData.delta.x, eventData.delta.y,0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!GetComponentInParent<Elevator>().Behaviour.DoorOpen)
        {
            return;
        }
        _dragging = true;
        _dragingFrom = (RectTransform)transform.parent;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_dragging)
        {
            return;
        }
        _dragging = false;
        float localX = _dragingFrom.InverseTransformPoint(eventData.position).x;
        if (localX<-((RectTransform)transform).rect.width/2f)
        {
            GetComponentInParent<Elevator>().PlaceHumanInside(this);
            _inCabin = true;
        }
        else
        {
            transform.SetParent(_dragingFrom);
        }       
    }
}
