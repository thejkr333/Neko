using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoostersUIHandler : MonoBehaviour
{
    [SerializeField] ItemSlot[] equippedBoostersPosition;
    [SerializeField] ItemSlot[] nonEquippedBoostersPosition;

    [SerializeField] GameObject highLightBorder;

    ItemSlot firstSelectedSlot;
    bool firstClicked;
    public Action<Boosters> BoosterEquipped, BoosterUnequipped;

    private void Start()
    {
        for (int i = 0; i < equippedBoostersPosition.Length; i++)
        {
            equippedBoostersPosition[i].Clicked += Click;
            if(equippedBoostersPosition[i].Image.sprite == null) equippedBoostersPosition[i].Image.color = new Color(1, 1, 1, 0);
        }

        for (int i = 0; i < nonEquippedBoostersPosition.Length; i++)
        {
            nonEquippedBoostersPosition[i].Clicked += Click;
            if (nonEquippedBoostersPosition[i].Image.sprite == null) nonEquippedBoostersPosition[i].Image.color = new Color(1, 1, 1, 0);
        }
    }

    private void OnEnable()
    {
        ResetClickHandler();
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
        Sprite _tempSprite = firstSelectedSlot.Image.sprite;
        firstSelectedSlot.Image.sprite = lastSelectedItem.Image.sprite;
        lastSelectedItem.Image.sprite = _tempSprite;

        if (firstSelectedSlot.Image.sprite == null) firstSelectedSlot.Image.color = new Color(1, 1, 1, 0);
        else firstSelectedSlot.Image.color = new Color(1, 1, 1, 1);

        if (lastSelectedItem.Image.sprite == null) lastSelectedItem.Image.color = new Color(1, 1, 1, 0);
        else lastSelectedItem.Image.color = new Color(1, 1, 1, 1);

        Boosters _tempBooster = firstSelectedSlot.BoosterInSlot;
        firstSelectedSlot.BoosterInSlot = lastSelectedItem.BoosterInSlot;
        lastSelectedItem.BoosterInSlot = _tempBooster;

        //Si los dos items vienen uno del pool y otro de equipped
        if (equippedBoostersPosition.Contains(firstSelectedSlot) && nonEquippedBoostersPosition.Contains(lastSelectedItem))
        {
            BoosterEquipped?.Invoke(firstSelectedSlot.BoosterInSlot);
            BoosterUnequipped?.Invoke(lastSelectedItem.BoosterInSlot);
        }
        else if (equippedBoostersPosition.Contains(lastSelectedItem) && nonEquippedBoostersPosition.Contains(firstSelectedSlot))
        {
            BoosterEquipped?.Invoke(lastSelectedItem.BoosterInSlot);
            BoosterUnequipped?.Invoke(firstSelectedSlot.BoosterInSlot);
        }

        ResetClickHandler();
    }

    public void AddBoosterUnequipped(Boosters booster)
    {
        for (int i = 0; i < nonEquippedBoostersPosition.Length; i++)
        {
            if (nonEquippedBoostersPosition[i].Image.sprite != null) continue;

            nonEquippedBoostersPosition[i].BoosterInSlot= booster;
            nonEquippedBoostersPosition[i].Image.sprite = GameManager.Instance.GetBoosterSprite(booster);
            nonEquippedBoostersPosition[i].Image.color = new Color(1, 1, 1, 1);

            Debug.Log("alpha de " + nonEquippedBoostersPosition[i].name+ " " +nonEquippedBoostersPosition[i].Image.color.a);
            return;
        }
    }

    public void AddBoosterEquipped(Boosters booster)
    {
        for (int i = 0; i < equippedBoostersPosition.Length; i++)
        {
            if (equippedBoostersPosition[i].Image.sprite != null) continue;

            equippedBoostersPosition[i].BoosterInSlot = booster;
            equippedBoostersPosition[i].Image.sprite = GameManager.Instance.GetBoosterSprite(booster);
            equippedBoostersPosition[i].Image.color = new Color(1, 1, 1, 1);
            return;
        }
    }

    private void ResetClickHandler()
    {
        firstClicked = false;
        firstSelectedSlot = null;
        highLightBorder.SetActive(false);
    }
}
