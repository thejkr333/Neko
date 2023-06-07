using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public enum Controllers { KbMouse, Controller}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DataSaving DataSaving;
    public Action SaveGameAction;

    public bool Cheating;

    [Header("Data")]
    [SerializeField] ItemData[] itemData;
    [SerializeField] BoosterData[] boosterData;

    [Header("BOOSTERS")]
    public SerializedDictionary<Boosters, bool> EquippedBoosters = new();
    public Action ExtraHealthOn;

    public Controllers currentScheme;
    public Action EnablePlayerInput, DisablePlayerInput; 
    public Action EnablePixieInput, DisablePixieInput; 
    public Action EnableUIInput, DisableUIInput;

    public Action ControllerConected;

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

        DataSaving = new DataSaving();

        if (Cheating)
        {
            DataSaving.Cheat();
        }
        else
        {
            if (DataSaving.IsThereSaveFiles()) DataSaving.LoadData();
        }

        foreach (Boosters booster in Enum.GetValues(typeof(Boosters)))
        {
            if (booster == Boosters.None) continue;

            bool _value = false;
            foreach (var boosterEquipped in DataSaving.BoostersEquipped)
            {
                if (booster == boosterEquipped)
                {
                    _value = true;
                    break;
                }
            }
            EquippedBoosters.Add(booster, _value);
        }
    }

    public void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == "Controller")
        {
            currentScheme = Controllers.Controller;
            ControllerConected?.Invoke();
        }
        else
        {
            currentScheme = Controllers.KbMouse;
        }
    }

    public void EnablePlayerInputs() => EnablePlayerInput?.Invoke();
    public void DisablePlayerInputs() => DisablePlayerInput?.Invoke();

    public void EnablePixieInputs() => EnablePixieInput?.Invoke();
    public void DisablePixieInputs() => DisablePixieInput?.Invoke();   
    public void EnableUIInputs() => EnableUIInput?.Invoke();
    public void DisableUIInputs() => DisableUIInput?.Invoke();
    
    public void NewGame() => DataSaving.EraseSaveFiles();

    public void SaveGame()
    {
        SaveGameAction?.Invoke();
        DataSaving.SaveData();
    }

    public void LoadGame()
    {
        SceneManager.sceneLoaded += LoadSceneAssetsAndPlayer;
        LoadScene("BosqueTurquesa");
    }

    void LoadSceneAssetsAndPlayer(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name != "BosqueTurquesa") return;

        Transform _player = FindObjectOfType<PlayerController>().transform;
        _player.transform.position = new Vector3(DataSaving.LastPlayerPosX, DataSaving.LastPlayerPosY, 0);

        SceneManager.sceneLoaded -= LoadSceneAssetsAndPlayer;
    }

    public SerializedDictionary<Items, bool> GetItemsInfo()
    {
        return DataSaving.ItemsOwned;
    }
    public void SetItemsInfo(ref SerializedDictionary<Items, bool> itemsInfo)
    {
        DataSaving.ItemsOwned = itemsInfo;
    }

    public SerializedDictionary<Boosters, bool> GetUnlockedBoostersInfo()
    {
        return DataSaving.BoostersOwned;
    }

    public void EquipBooster(Boosters booster)
    {
        EquippedBoosters[booster] = true;
        if (booster == Boosters.ExtraHealth) ExtraHealthOn?.Invoke();
    }

    public void UnequipBooster(Boosters booster)
    {
        EquippedBoosters[booster] = false;
    }

    public Boosters[] GetEquippedBoosters()
    {
        return DataSaving.BoostersEquipped;
    }

    public void SetBoostersInfo(ref SerializedDictionary<Boosters, bool> boostersInfo, Boosters[] equippedBoosters)
    {
        DataSaving.BoostersOwned = boostersInfo;
        DataSaving.BoostersEquipped = equippedBoosters;
    }

    public Sprite GetItemSprite(Items item)
    {
        for (int i = 0; i < itemData.Length; i++)
        {
            if (itemData[i].ID == item) return itemData[i].Sprite;
        }

        Debug.LogError(item + " not in list");
        return null;
    }

    public Sprite GetBoosterSprite(Boosters booster)
    {
        for (int i = 0; i < boosterData.Length; i++)
        {
            if (boosterData[i].ID == booster) return boosterData[i].Sprite;
        }

        Debug.LogError(booster + " not in list");
        return null;
    }

    public Boosters GetBoosterIDFromSprite(Sprite boosterSprite)
    {
        for (int i = 0; i < boosterData.Length; i++)
        {
            if (boosterData[i].Sprite == boosterSprite) return boosterData[i].ID;
        }

        Debug.LogError(boosterSprite + " not in list");
        return Boosters.x2Damage;
    }
    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void LoadScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }

    internal void PlayerDied()
    {
        DisablePlayerInputs();
        DisablePixieInputs();
        DisableUIInputs();
    }
}

[System.Serializable]
public class DataSaving
{
    public int Money;
    public SerializedDictionary<Items, bool> ItemsOwned;
    public SerializedDictionary<Boosters, bool> BoostersOwned;
    public Boosters[] BoostersEquipped = new Boosters[3];
    public float LastPlayerPosX, LastPlayerPosY;

    public DataSaving ()
    {
        Money = 0;
        ItemsOwned = new();
        foreach (Items item in Enum.GetValues(typeof(Items)))
        {
            ItemsOwned.Add(item, false);
        }
        BoostersOwned = new();
        foreach (Boosters booster in Enum.GetValues(typeof(Boosters)))
        {
            if (booster == Boosters.None) continue;

            BoostersOwned.Add(booster, false);
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

        foreach (var booster in BoostersOwned.Keys.ToList())
        {
            BoostersOwned[booster] = true;
        }

        for (int i = 0; i < BoostersEquipped.Length; i++)
        {
            BoostersEquipped[i] = Boosters.None;
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
            PlayerPrefs.SetInt(item.ToString(), value);
        }

        foreach(var booster in BoostersOwned.Keys)
        {
            int value = BoostersOwned[booster] ? 1 : 0;
            PlayerPrefs.SetInt(booster.ToString(), value);
        }

        for (int i = 0; i < BoostersEquipped.Length; i++)
        {
            PlayerPrefs.SetInt("BoosterEquipped" + i, (int)BoostersEquipped[i]);
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
            ItemsOwned[item] = PlayerPrefs.GetInt(item.ToString()) == 1;
        }

        foreach (var booster in BoostersOwned.Keys.ToList())
        {
            BoostersOwned[booster] = PlayerPrefs.GetInt(booster.ToString()) == 1;
        }

        for (int i = 0; i < BoostersEquipped.Length; i++)
        {
            BoostersEquipped[i] = (Boosters)PlayerPrefs.GetInt("BoosterEquipped" + i);
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
