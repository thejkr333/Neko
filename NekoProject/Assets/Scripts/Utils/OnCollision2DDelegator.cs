////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCollision2DDelegator : MonoBehaviour
{
    public EventSensor enter;
    public EventSensor exit;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        enter.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        exit.Invoke(collision);
    }

    [System.Serializable]
    public class EventSensor : UnityEvent<Collision2D>
    {
    }
}
