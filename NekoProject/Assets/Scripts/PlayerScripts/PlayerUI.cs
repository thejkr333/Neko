using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{
    PlayerController playerController;
    PlayerStorage playerStorage;

    HealthSystem healthSystem;

    [SerializeField] Image[] healthOrbs;
    [SerializeField] TMP_Text moneyText;

    [Header("MENU")]
    [SerializeField] GameObject menu;
    [SerializeField] GameObject[] menuGameobjects;
    bool menuOn;
    int menuIndex, menuLength;

    [Header("MAP")]
    [SerializeField] GameObject map;
    bool mapOn;

    [Header("SHIELD")]
    [SerializeField] Image shieldCDImg;
    float shieldTimeCD, shieldTimer;

    [Header("ANTMAN")]
    [SerializeField] Image antmanCDImg;
    float antmanTimeCD, antmanTimer;

    [Header("INVENTORY")]
    [SerializeField] GameObject inventory;
    [SerializeField] TMP_Text moneyTextInventory;
    GameObject[] inventorySlots;

    [Header("Boosters")]
    [SerializeField] BoostersUIHandler boostersHandler;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        playerController = GetComponent<PlayerController>();
        playerStorage = GetComponent<PlayerStorage>();  

        //Set starting UI
        map.SetActive(false);
        menu.SetActive(false);

        //Set menu child objects
        for (int i = 1; i < menuGameobjects.Length; i++)
        {
            menuGameobjects[i].SetActive(false);
        }

        //Set inventory slots
        inventorySlots = new GameObject[inventory.transform.childCount];
        for (int i = 0; i < inventory.transform.childCount; i++)
        {
            inventorySlots[i] = inventory.transform.GetChild(i).gameObject;
            inventorySlots[i].SetActive(false);
        }

        playerStorage.ItemUnlocked += AddItemToInventory;
        playerStorage.BoosterUnlocked += AddBoosterToPool;
        playerStorage.BoosterEquipped += EquipBooster;
    }

    private void Start()
    {
        //Set abilities info
        shieldTimeCD = playerController.shieldCD;
        antmanTimeCD = playerController.antmanCD;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthOrbs();
        UpdateMoneyTexts();

        if (Input.GetKeyDown(KeyCode.Escape)) ToggleMenu();
        //if (!menuOn && Input.GetKey(KeyCode.M)) ToggleMap();

        if (menuOn) Menu();
        else
        {
            menu.SetActive(Input.GetKey(KeyCode.M));
            map.SetActive(Input.GetKey(KeyCode.M));
        }

        UpdateShieldCD();
        UpdateAntmanCD();

        Time.timeScale = menuOn ? 0 : 1;
    }

    void UpdateShieldCD()
    {
        shieldTimer = playerController.shieldCDTimer;

        if (shieldTimer <= 0)
        {
            shieldCDImg.fillAmount = 0; 
            return;
        }

        shieldCDImg.fillAmount = 1 - (shieldTimer / shieldTimeCD);
    }

    void UpdateAntmanCD()
    {
        antmanTimer = playerController.antmanTimer;

        if (antmanTimer <= 0)
        {
            antmanCDImg.fillAmount = 0;
            return;
        }

        antmanCDImg.fillAmount = 1 - (antmanTimer / antmanTimeCD);
    }

    void UpdateHealthOrbs()
    {
        for (int i = 0; i < healthSystem.maxHealth; i++)
        {
            if (i < healthSystem.currentHealth) healthOrbs[i].gameObject.SetActive(true);
            else healthOrbs[i].gameObject.SetActive(false);
        }
    }

    void ToggleMap()
    {
        mapOn = !mapOn;

        map.SetActive(mapOn);   
    }

    void ToggleMenu()
    {
        menuOn = !menuOn;

        menu.SetActive(menuOn);

        if(!menuOn)
            for (int i = 0; i < menuLength; i++)
            {
                menuGameobjects[menuIndex].SetActive(false);
            }
    }

    void Menu()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            menuGameobjects[menuIndex].SetActive(false);

            if (menuIndex == 0) menuIndex = menuGameobjects.Length - 1;
            else menuIndex--;
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            menuGameobjects[menuIndex].SetActive(false);

            if (menuIndex == menuGameobjects.Length - 1) menuIndex = 0;
            else menuIndex++;
        }

        if (menuGameobjects[menuIndex] != map && map.activeSelf) map.SetActive(false);
        menuGameobjects[menuIndex].SetActive(true);
    }

    void UpdateMoneyTexts()
    {
        moneyText.text = playerStorage.Coins.ToString();
        moneyTextInventory.text = playerStorage.Coins.ToString();
    }

    void AddItemToInventory(Items item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Image _img = inventorySlots[i].GetComponent<Image>();
            if (_img.sprite != null) continue;

            inventorySlots[i].SetActive(true);
            _img.sprite = GameManager.Instance.GetItemSprite(item);
            return;
        }
    }

    void AddBoosterToPool(Boosters booster)
    {
        boostersHandler.AddBoosterUnequipped(booster);
    }

    void EquipBooster(Boosters booster)
    {
        boostersHandler.AddBoosterEquipped(booster);
    }
}
