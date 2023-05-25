using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected virtual void Start()
    {
        StopHighLight();
    }
    public abstract void StartHighLight();
    public abstract void StopHighLight();
    public abstract void Interact();
}
