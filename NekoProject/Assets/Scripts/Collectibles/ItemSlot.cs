////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ItemSlot : MonoBehaviour, IClickable
{
    public Action<ItemSlot> Clicked;
    public Boosters Booster;
    public Image Image;


    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    public void OnSelected()
    {
        Clicked?.Invoke(this);
    }
}
