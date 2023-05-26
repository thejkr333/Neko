using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAttendant : Interactable
{
    [SerializeField] GameObject canvasTienda;
    protected override void Start()
    {
        base.Start();   
        canvasTienda.SetActive(false);
    }
    public override void Interact(Transform player)
    {
        canvasTienda.SetActive(true);
    }
}
