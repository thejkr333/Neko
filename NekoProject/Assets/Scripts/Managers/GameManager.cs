using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    static DataSaving DataSavingInstance;
    public Action SaveGameAction;

    public bool Cheating;

    [SerializeField] ItemData[] itemData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(Instance);
            return;
        }

        DataSavingInstance = new DataSaving();

        if (Cheating)
        {
            DataSavingInstance.Cheat();
        }
        else
        {
            if (DataSavingInstance.IsThereSaveFiles()) DataSavingInstance.LoadData();
        }
    }
    public void NewGame()
    {
        DataSavingInstance.EraseSaveFiles();
    }

    public void SaveGame()
    {
        SaveGameAction?.Invoke();
        DataSavingInstance.SaveData();
    }

    public Dictionary<Items, bool> GetItemsInfo()
    {
        return DataSavingInstance.ItemsOwned;
    }
    public void SetItemsInfo(ref Dictionary<Items, bool> itemsInfo)
    {
        DataSavingInstance.ItemsOwned  = itemsInfo;
    }

    public Sprite GetItemSprite(Items item)
    {
        for (int i = 0; i < itemData.Length; i++)
        {
            if (itemData[i].ID == item) return itemData[i].Sprite;
        }

        Debug.LogError("Item not in list");
        return null;
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
