using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElevatorCabin : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GetComponentInParent<Elevator>().Drop(transform);
    }
}
