using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FixScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{

    public ScrollRect MainScroll;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CheckMainScroll();
        if (MainScroll != null)
        {
            MainScroll.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        CheckMainScroll();
        if (MainScroll != null)
        {
            MainScroll.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CheckMainScroll();
        if (MainScroll != null)
        {
            MainScroll.OnEndDrag(eventData);
        }
    }

    public void OnScroll(PointerEventData data)
    {
        CheckMainScroll();
        if (MainScroll != null)
        {
            MainScroll.OnScroll(data);
        }
    }

    private void CheckMainScroll()
    {
        if (MainScroll == null)
        {
            MainScroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
        }
    }
}
