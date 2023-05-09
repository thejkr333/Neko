using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Items { }
public class PlayerStorage : MonoBehaviour
{
    public int Coins { get; private set; }
    public Dictionary<Items, bool> ItemsUnlockedInfo = new();
    List<Item> inventory;

    void Awake()
    {
        //foreach (Items item in Enum.GetValues(typeof(Items)))
        //{
        //    ItemsUnlockedInfo.Add(item, false);
        //}
        //RefreshItemDictionary();
    }

    public void AddCoins(int amount = 1)
    {
        Coins += amount;
    }

    public void SubstractCoins(int amount = 1)
    {
        Coins -= amount;
    }

    void RefreshItemDictionary()
    {
        foreach (var item in inventory)
        {
            ItemsUnlockedInfo[item.thisItem] = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent(out Item item)) return;

        inventory.Add(item);
        RefreshItemDictionary();
        Destroy(item.gameObject);
    }
}
