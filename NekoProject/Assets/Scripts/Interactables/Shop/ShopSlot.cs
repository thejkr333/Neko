////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour , IClickable
{
    public string Description;
    public int Cost;
    public Action<ShopSlot> Clicked;
    public Boosters Booster;

    public void OnSelected()
    {
        Clicked?.Invoke(this);
    }
}
