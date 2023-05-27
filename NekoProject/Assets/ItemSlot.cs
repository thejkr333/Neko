using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public Action<ItemSlot> Clicked;
    public Boosters BoosterInSlot;
    public Image Image;

    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }
}
