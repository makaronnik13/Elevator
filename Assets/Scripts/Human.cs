using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Human : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Sprite[] variants;

    private RectTransform _dragingFrom;
    private bool _dragging = false;
    public RectTransform DraggingFrom
    {
        get
        {
            return _dragingFrom;
        }
    }

    void Start()
    {
        GetComponent<Image>().sprite = variants[Random.Range(0, variants.Length)];
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_dragging)
        {
            return;
        }

        transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponentInParent<Elevator>().DragHuman(this);
        _dragging = true;
        _dragingFrom = (RectTransform)transform.parent;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_dragging)
        {
            return;
        }
        _dragging = false;

        GetComponent<Image>().raycastTarget = true;
        transform.SetParent(_dragingFrom);
    }

    public void Drop(Transform aim)
    {
        _dragging = false;
        GetComponent<Image>().raycastTarget = true;
        transform.SetParent(aim);
        if (aim.GetComponent<WaitingPlace>())
        {
            Destroy(gameObject);
        }
    }
}
