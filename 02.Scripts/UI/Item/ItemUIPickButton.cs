using HTH.InputHandlers;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUIPickButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        ItemController.instance.Pick();
    }
}
