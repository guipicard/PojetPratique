using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;

                    //Le nom de l'event (ON_KEK_CHOSE)
                               //Action, c'est l'event
                                       //C'est des param�tres, gr�ce au dictionnaire, on peut envoyer n param�tres
                                       //Gr�ce au mot cl� object, je peux envoyer n'importe lequel type de param�tre
    private Dictionary<string, Action<Dictionary<string, object>>> events;

    //ON_SPELL_CAST
    //En param�tre, avoir SpellId = 0, 1, 2, 3

    public static EventManager GetInstance()
    { return instance; }

    void Start()
    {
        instance = this;
        events = new Dictionary<string, Action<Dictionary<string, object>>>();
    }

    public void StartListening(string actionName, Action<Dictionary<string, object>> eventParams)
    {
        events.Add(actionName, eventParams);
    }

    public void StopListening()
    {
        Dictionary<string, object> paramsAssociatedWithEvent = new Dictionary<string, object>();
        paramsAssociatedWithEvent.Add("SpellId", gameObject.transform);

        Transform test = paramsAssociatedWithEvent["SpellId"] as Transform;

        TriggerEvent("ON_SPELL_CAST", null);
    }

    public void TriggerEvent(string eventToTrigger, Dictionary<string, object> eventParams)
    {
        events[eventToTrigger]?.Invoke(eventParams);
    }
}


