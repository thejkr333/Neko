using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStorage : MonoBehaviour
{
    public int Coins { get; private set; }
    public Dictionary<Items, bool> ItemsUnlockedInfo = new();
    List<Item> inventory = new();

    private void Start()
    {
        GameManager.Instance.SaveGameAction += UpdateDataToGameManager;
        ItemsUnlockedInfo = GameManager.Instance.GetItemsInfo();
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
            ItemsUnlockedInfo[item.ID] = true;
        }
    }

    void UpdateDataToGameManager()
    {
        GameManager.Instance.SetItemsInfo(ref ItemsUnlockedInfo);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent(out Item item)) return;

        inventory.Add(item);
        RefreshItemDictionary();
        Destroy(item.gameObject);
    }
}
