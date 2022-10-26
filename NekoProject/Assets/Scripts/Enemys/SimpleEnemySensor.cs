using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEnemySensor : MonoBehaviour
{
    public EventSensor enter;
    public EventSensor exit;

    /// <summary>
    /// Evento de trigger enter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        enter.Invoke(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        exit.Invoke(collision);
    }

    [System.Serializable]
    public class EventSensor : UnityEvent<Collider2D>
    {
    }
}
