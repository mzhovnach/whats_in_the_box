using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using TObject.Shared;
//EventManager
public class EventManager
{
	public static event EventController.MethodContainer OnTransitToScreenEvent;
	public void CallOnTransitToScreenEvent(EventData ob = null) { if (OnTransitToScreenEvent != null) OnTransitToScreenEvent(ob); }
    public static event EventController.MethodContainer OnShowPopupWindowEvent;
    public void CallOnShowPopupWindowEvent(EventData ob = null) { if (OnShowPopupWindowEvent != null) OnShowPopupWindowEvent(ob); }
    public static event EventController.MethodContainer OnHidePopupWindowEvent;
    public void CallOnHidePopupWindowEvent(EventData ob = null) { if (OnHidePopupWindowEvent != null) OnHidePopupWindowEvent(ob); }

	public static event EventController.MethodContainer OnBoxesDataChangedEvent;
	public void CallOnBoxesDataChangedEvent(EventData ob = null) { if (OnBoxesDataChangedEvent != null) OnBoxesDataChangedEvent(ob); }

    public static event EventController.MethodContainer OnInternetBecomeAvailableAfterRetryEvent;
    public void CallOnInternetBecomeAvailableAfterRetryEvent(EventData ob = null) { if (OnInternetBecomeAvailableAfterRetryEvent != null) OnInternetBecomeAvailableAfterRetryEvent(ob); }

    public static event EventController.MethodContainer OnShowTutorialEvent;
    public void CallOnShowTutorialEvent(EventData ob = null) { if (OnShowTutorialEvent != null) OnShowTutorialEvent(ob); } 
}
