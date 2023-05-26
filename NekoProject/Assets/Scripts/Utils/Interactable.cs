using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected GameObject highLightGO;
    protected virtual void Start()
    {
        StopHighLight();
    }
    public virtual void StartHighLight()
    {
        highLightGO.SetActive(true);
    }
    public virtual void StopHighLight()
    {
        highLightGO.SetActive(false);
    }
    public abstract void Interact(Transform player);
}
