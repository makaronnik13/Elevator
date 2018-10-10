using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Human : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Sprite[] variants;
    private RectTransform _dragingFrom;

    void Start()
    {
        GetComponent<Image>().sprite = variants[Random.Range(0,variants.Length)];
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 resultPosition = _dragingFrom.InverseTransformPoint(eventData.position);

        float minx = -_dragingFrom.rect.width / 2f - ((RectTransform)transform).rect.width * 1.5f;
        float maxx = _dragingFrom.rect.width / 2f - ((RectTransform)transform).rect.width / 2f;
        float xPosition = Mathf.Clamp(resultPosition.x, minx, maxx);

        resultPosition = new Vector3(xPosition, 0, resultPosition.z);
        
        transform.position = _dragingFrom.TransformPoint(resultPosition);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragingFrom = (RectTransform)transform.parent;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(_dragingFrom);
    }
}
