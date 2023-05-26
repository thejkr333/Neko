using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStorage : MonoBehaviour
{
    public int Coins { get; private set; }
    public Dictionary<Items, bool> ItemsUnlockedInfo = new();
    public Dictionary<Boosters, int> BoostersAmount = new();
    public Action<Items> ItemUnlocked;
    public Action<Boosters, int> BoosterAmountChanged;

    private void Start()
    {
        GameManager.Instance.SaveGameAction += UpdateDataToGameManager;
        ItemsUnlockedInfo = GameManager.Instance.GetItemsInfo();

        foreach (var item in ItemsUnlockedInfo.Keys)
        {
            if (ItemsUnlockedInfo[item]) ItemUnlocked?.Invoke(item);
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

    public void AddBooster(Boosters booster)
    {
        BoostersAmount[booster]++; 
        BoosterAmountChanged?.Invoke(booster, BoostersAmount[booster]);
    }

    public void ConsumeBooster(Boosters booster)
    {
        BoostersAmount[booster]--;
        BoosterAmountChanged?.Invoke(booster, BoostersAmount[booster]);
    }

    void UpdateDataToGameManager()
    {
        GameManager.Instance.SetItemsInfo(ref ItemsUnlockedInfo);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent(out Item item)) return;

        UnlockItem(item.ID);
        Destroy(item.gameObject);
    }
}
