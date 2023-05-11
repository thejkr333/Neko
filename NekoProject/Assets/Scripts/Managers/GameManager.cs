using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    static DataSaving DataSaving;
    public Action SaveGameAction;

    public bool Cheating;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(Instance);
        }

        DataSaving = new DataSaving();

        if (Cheating)
        {
            DataSaving.Cheat();
        }
        else
        {
            if (DataSaving.IsThereSaveFiles()) DataSaving.LoadData();
        }
    }

    public void SaveGame()
    {
        SaveGameAction?.Invoke();
    }

    public Dictionary<Items, bool> GetItemsInfo()
    {
        return DataSaving.ItemsOwned;
    }
    public void SetItemsInfo(ref Dictionary<Items, bool> itemsInfo)
    {
        DataSaving.ItemsOwned  = itemsInfo;
    }

    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void LoadScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }
}

public class DataSaving
{
    public int Money;
    public Dictionary<Items, bool> ItemsOwned;
    public float LastPlayerPosX, LastPlayerPosY;

    public DataSaving ()
    {
        Money = 0;
        ItemsOwned = new();
        foreach (Items item in Enum.GetValues(typeof(Items)))
        {
            ItemsOwned.Add(item, false);
        }
        LastPlayerPosX = 0;
        LastPlayerPosY = 0;
    }

    public void Cheat()
    {
        Money = 10000;
        LastPlayerPosX = 0;
        LastPlayerPosY = 0;

        foreach (var item in ItemsOwned.Keys.ToList())
        {
            ItemsOwned[item] = true;
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(nameof(Money), Money);
        PlayerPrefs.SetFloat(nameof(LastPlayerPosX), LastPlayerPosX);
        PlayerPrefs.SetFloat(nameof(LastPlayerPosY), LastPlayerPosY);

        foreach (var item in ItemsOwned.Keys)
        {
            int value = ItemsOwned[item] ? 1 : 0;
            PlayerPrefs.SetInt(nameof(item), value);
        }

        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        Money = PlayerPrefs.GetInt(nameof(Money));
        LastPlayerPosX = PlayerPrefs.GetFloat(nameof(LastPlayerPosX));
        LastPlayerPosY = PlayerPrefs.GetFloat(nameof(LastPlayerPosY));

        foreach (var item in ItemsOwned.Keys.ToList())
        {
            ItemsOwned[item] = PlayerPrefs.GetInt(nameof(item)) == 1;
        }   
    }

    public bool IsThereSaveFiles()
    {
        return PlayerPrefs.HasKey(nameof(Money));
    }

    public void EraseSaveFiles()
    {
        PlayerPrefs.DeleteAll();
    }
}
