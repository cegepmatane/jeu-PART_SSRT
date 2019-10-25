using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager {

    private static Dictionary<string, UnityEvent> m_Events = new Dictionary<string, UnityEvent>();


    public static void AddListner(string a_EventName, UnityAction a_Action)
    {
        UnityEvent t_UnityEvent = null;

        m_Events.TryGetValue(a_EventName, out t_UnityEvent);

        if (t_UnityEvent == null)
        {
            t_UnityEvent = new UnityEvent();
            m_Events.Add(a_EventName, t_UnityEvent);
        }

        t_UnityEvent.AddListener(a_Action);
    }

    public static void RemoveListner(string a_EventName, UnityAction a_Action)
    {
        UnityEvent t_UnityEvent = null;

        m_Events.TryGetValue(a_EventName, out t_UnityEvent);

        if (t_UnityEvent != null)
        {
            //remove action
            t_UnityEvent.RemoveListener(a_Action);
        }
    }

    public static void TriggerEvent(string a_EventName)
    {
        UnityEvent t_UnityEvent = null;

        m_Events.TryGetValue(a_EventName, out t_UnityEvent);

        if (t_UnityEvent != null)
        {
            //trigger action
            t_UnityEvent.Invoke();
        }
        else
        {
            Debug.LogError("Tryed to trigger non existing event");
        }
    }
}
