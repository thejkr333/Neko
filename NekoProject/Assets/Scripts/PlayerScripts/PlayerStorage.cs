using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStorage : MonoBehaviour
{
    public int Coins { get; private set; }
    public Dictionary<Items, bool> ItemsUnlockedInfo = new();
    public Dictionary<Boosters, bool> BoostersUnlockInfo = new();
    Boosters[] equippedBoosters = new Boosters[3];

    public Action<Items> ItemUnlocked;
    public Action<Boosters> BoosterUnlocked;
    public Action<Boosters> BoosterEquipped;

    private void Start()
    {
        GameManager.Instance.SaveGameAction += UpdateDataToGameManager;
        ItemsUnlockedInfo = GameManager.Instance.GetItemsInfo();
        BoostersUnlockInfo = GameManager.Instance.GetUnlockedBoostersInfo();
        equippedBoosters = GameManager.Instance.GetEquippedBoosters();

        foreach (var item in ItemsUnlockedInfo.Keys)
        {
            if (ItemsUnlockedInfo[item]) ItemUnlocked?.Invoke(item);
        }
        foreach (var booster in BoostersUnlockInfo.Keys)
        {
            if (BoostersUnlockInfo[booster]) BoosterUnlocked?.Invoke(booster);
        }
        for (int i = 0; i < equippedBoosters.Length; i++)
        {
            if (equippedBoosters[i] == Boosters.None) continue;

            BoosterEquipped?.Invoke(equippedBoosters[i]);
        }
    }

    public void AddCoins(int amount = 1)
    {
        Coins += amount;
    }

    public void SubstractCoins(int amount = 1)
    {
        Coins -= amount;
    }

    void UnlockItem(Items item)
    {
        ItemsUnlockedInfo[item] = true;
        ItemUnlocked?.Invoke(item);
    }

    public void UnlockBooster(Boosters booster)
    {
        BoostersUnlockInfo[booster] = true; 
        BoosterUnlocked?.Invoke(booster);
    }

    void UpdateDataToGameManager()
    {
        GameManager.Instance.SetItemsInfo(ref ItemsUnlockedInfo);
        GameManager.Instance.SetBoostersInfo(ref BoostersUnlockInfo, equippedBoosters);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Item item))
        {
            UnlockItem(item.ID);
            Destroy(item.gameObject);
        }
        else if(collision.transform.TryGetComponent(out Booster booster))
        {
            UnlockBooster(booster.ID);
            Destroy(item.gameObject);
        }
    }
}
