using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostersUIHandler : MonoBehaviour
{
    [SerializeField] ItemSlot[] equippedBoostersPosition;
    [SerializeField] ItemSlot[] nonEquippedBoostersPosition;

    [SerializeField] GameObject highLightBorder;

    ItemSlot firstSelectedSlot;
    bool firstClicked;

    private void Awake()
    {
        for (int i = 0; i < equippedBoostersPosition.Length; i++)
        {
            equippedBoostersPosition[i].Clicked += Click;
        }

        for (int i = 0; i < nonEquippedBoostersPosition.Length; i++)
        {
            nonEquippedBoostersPosition[i].Clicked += Click;
        }
    }

    private void OnEnable()
    {
        firstClicked = false;
        firstSelectedSlot = null;
    }

    void Click(ItemSlot itemClicked)
    {
        if(!firstClicked)
        {
            firstSelectedSlot = itemClicked;
            HighLight(itemClicked.transform);
            firstClicked = true;
        }
        else
        {
            Swap(itemClicked);
        }
    }

    void HighLight(Transform highLightPos)
    {
        highLightBorder.SetActive(true);
        highLightBorder.transform.position = highLightPos.position;
    }

    void Swap(ItemSlot lastSelectedItem)
    {

    }
}
