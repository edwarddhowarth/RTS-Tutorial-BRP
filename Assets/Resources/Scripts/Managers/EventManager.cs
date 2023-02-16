using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * EventName for each unique Trigger
 * Event can pass generic object which must be casted on use
 * Ensure Casts are correct and comment on listener methods of what they take in
 */

public enum EventName
{
    UpdateResourceTexts,
    CheckBuildingButtons,
    HoverBuildingButton,
    UnhoverBuildingButton,
    SelectUnit,
    DeselectUnit,
    PlaceBuildingOff
}

[System.Serializable]
public class TypedEvent: UnityEvent<object> {}

public class EventManager : MonoBehaviour
{
    private Dictionary<string, TypedEvent> _typedEvents;
    private Dictionary<string, UnityEvent> _events;
    private static EventManager _eventManager;

    public static EventManager instance
    {
        get
        {
            if(!_eventManager)
            {
                _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!_eventManager)
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                else
                    _eventManager.Init();
            }
            return _eventManager;
        }
    }

    void Init()
    {
        if(_events == null)
        {
            _events = new Dictionary<string, UnityEvent>();
            _typedEvents = new Dictionary<string, TypedEvent>();
        }
    }



    
    //Enum Test
    public static void AddListener(EventName eventName, UnityAction listener)
    {
        UnityEvent evt = null;
        if(instance._events.TryGetValue(eventName.ToString(), out evt))
        {
            evt.AddListener(listener);
        }
        else
        {
            evt = new UnityEvent();
            evt.AddListener(listener);
            instance._events.Add(eventName.ToString(), evt);
        }
    }
    
    public static void AddListener(EventName eventName, UnityAction<object> listener)
    {
        TypedEvent evt = null;
        if(instance._typedEvents.TryGetValue(eventName.ToString(), out evt))
        {
            evt.AddListener(listener);
        }
        else
        {
            evt = new TypedEvent();
            evt.AddListener(listener);
            instance._typedEvents.Add(eventName.ToString(), evt);
        }
    }

    
    //Enum Test
    public static void RemoveListener(EventName eventName, UnityAction listener)
    {
        if (_eventManager == null) return;
        UnityEvent evt = null;
        if (instance._events.TryGetValue(eventName.ToString(), out evt))
            evt.RemoveListener(listener);
    }
    
    public static void RemoveListener(EventName eventName, UnityAction<object> listener)
    {
        if (_eventManager == null) return;
        TypedEvent evt = null;
        if (instance._typedEvents.TryGetValue(eventName.ToString(), out evt))
            evt.RemoveListener(listener);
    }



    //Enum Test
    public static void TriggerEvent(EventName eventName)
    {
        UnityEvent evt = null;
        if (instance._events.TryGetValue(eventName.ToString(), out evt))
            evt.Invoke();
    }
    
    public static void TriggerEvent(EventName eventName, object data)
    {
        TypedEvent evt = null;
        if (instance._typedEvents.TryGetValue(eventName.ToString(), out evt))
            evt.Invoke(data);
    }


}
