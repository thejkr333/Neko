using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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
        GameManager.Instance.ControllerConected += ControllerConected;
    }
    private void OnDisable() => GameManager.Instance.ControllerConected -= ControllerConected;
    void ControllerConected()
    {
        if (EventSystem.current.alreadySelecting) return;

        EventSystem.current.SetSelectedGameObject(nonEquippedBoostersPosition[0].gameObject);
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

        Boosters _tempBooster = firstSelectedSlot.Booster;
        firstSelectedSlot.Booster = lastSelectedItem.Booster;
        lastSelectedItem.Booster = _tempBooster;

        //Si los dos items vienen uno del pool y otro de equipped
        if (equippedBoostersPosition.Contains(firstSelectedSlot) && nonEquippedBoostersPosition.Contains(lastSelectedItem))
        {
            GameManager.Instance.EquipBooster(firstSelectedSlot.Booster);
            GameManager.Instance.UnequipBooster(lastSelectedItem.Booster);
        }
        else if (equippedBoostersPosition.Contains(lastSelectedItem) && nonEquippedBoostersPosition.Contains(firstSelectedSlot))
        {
            GameManager.Instance.EquipBooster(lastSelectedItem.Booster);
            GameManager.Instance.UnequipBooster(firstSelectedSlot.Booster);
        }

        ResetClickHandler();
    }

    public void AddBoosterUnequipped(Boosters booster)
    {
        for (int i = 0; i < nonEquippedBoostersPosition.Length; i++)
        {
            if (nonEquippedBoostersPosition[i].Image.sprite != null) continue;

            nonEquippedBoostersPosition[i].Booster= booster;
            nonEquippedBoostersPosition[i].Image.sprite = GameManager.Instance.GetBoosterSprite(booster);
            nonEquippedBoostersPosition[i].Image.color = new Color(1, 1, 1, 1);
            return;
        }
    }

    public void AddBoosterEquipped(Boosters booster)
    {
        for (int i = 0; i < equippedBoostersPosition.Length; i++)
        {
            if (equippedBoostersPosition[i].Image.sprite != null) continue;

            equippedBoostersPosition[i].Booster = booster;
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
        EventSystem.current.SetSelectedGameObject(nonEquippedBoostersPosition[0].gameObject);
    }
}
