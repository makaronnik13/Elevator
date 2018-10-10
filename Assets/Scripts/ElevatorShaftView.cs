using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElevatorShaftView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform Cabin;
    public float FloorSize;
    public GameObject ShaftPanelPrefab;
    private bool moving = false;

    private Vector2 _pointPosition;

    public void Init(int floors)
    {
        for(int i = floors; i> 0; i--)
        {
            GameObject newShaftPanel = Instantiate(ShaftPanelPrefab, Vector3.zero, Quaternion.identity, transform);
            newShaftPanel.GetComponentInChildren<Text>().text = i.ToString();
        }
        Cabin.SetAsLastSibling();
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, floors*FloorSize);
        GetComponent<RectTransform>().anchoredPosition = Vector2.up * floors/2f*FloorSize;
    }

    public void ChangePosition(float v)
    {        
        Cabin.anchoredPosition = new Vector2(Cabin.anchoredPosition.x, FloorSize*(v+0.5f));
        if (!moving)
        {
            Vector2 needPosition = new Vector2(transform.localPosition.x, Cabin.sizeDelta.y  - Screen.height / 2f - Cabin.anchoredPosition.y + GetComponent<RectTransform>().sizeDelta.y / 2f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, needPosition, Time.deltaTime * 3f);
        }
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
}
