using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class PlayerUI : MonoBehaviour, NekoInput.IMenuActions, NekoInput.IPlayerUIActions
{
    PlayerController playerController;
    PlayerStorage playerStorage;

    HealthSystem healthSystem;

    [SerializeField] Image[] healthOrbs;
    [SerializeField] TMP_Text moneyText;

    [Header("MENU")]
    [SerializeField] GameObject menu;
    bool menuOpen;

    [Header("INVENTORY")]
    [SerializeField] GameObject inventory;
    [SerializeField] GameObject[] inventoryGameobjects;
    bool inventoryOpen;
    int inventoryIndex;

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
    [SerializeField] GameObject items;
    [SerializeField] TMP_Text moneyTextItems;
    GameObject[] itemSlots;

    [Header("Boosters")]
    [SerializeField] BoostersUIHandler boostersHandler;

    private NekoInput controlsInput;

    private void Awake()
    {
        controlsInput = new NekoInput();
        controlsInput.PlayerUI.SetCallbacks(this);
        controlsInput.Menu.SetCallbacks(this);

        healthSystem = GetComponent<HealthSystem>();
        playerController = GetComponent<PlayerController>();
        playerStorage = GetComponent<PlayerStorage>();  

        //Set starting UI
        map.SetActive(false);
        inventory.SetActive(false);
        menu.SetActive(false);

        //Set menu child objects
        for (int i = 1; i < inventoryGameobjects.Length; i++)
        {
            inventoryGameobjects[i].SetActive(false);
        }

        //Set inventory slots
        itemSlots = new GameObject[items.transform.childCount];
        for (int i = 0; i < items.transform.childCount; i++)
        {
            itemSlots[i] = items.transform.GetChild(i).gameObject;
            itemSlots[i].SetActive(false);
        }

        playerStorage.ItemUnlocked += AddItemToInventory;
        playerStorage.BoosterUnlocked += AddBoosterToPool;
        playerStorage.BoosterEquipped += EquipBooster;
    }

    private void Start()
    {
        GameManager.Instance.EnableUIInput += OnEnableUIInputs;
        GameManager.Instance.DisableUIInput += OnDisableUIInputs;

        //Set abilities info
        shieldTimeCD = playerController.shieldCD;
        antmanTimeCD = playerController.antmanCD;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthOrbs();
        UpdateMoneyTexts();

        //if (Input.GetKeyDown(KeyCode.Tab)) ToggleInventory();
        ////if (!menuOn && Input.GetKey(KeyCode.M)) ToggleMap();

        //if (inventoryOpen) Inventory();
        //else
        //{
        //    inventory.SetActive(Input.GetKey(KeyCode.M));
        //    map.SetActive(Input.GetKey(KeyCode.M));
        //}

        UpdateShieldCD();
        UpdateAntmanCD();
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

    private void ToggleMenu()
    {
        menuOpen = !menuOpen;

        Time.timeScale = menuOpen ? 0 : 1;
        menu.SetActive(menuOpen);
    }

    public void CloseMenu()
    {
        menuOpen = false;
        Time.timeScale = 1;
        menu.SetActive(false);

        GameManager.Instance.EnablePlayerInputs();
        GameManager.Instance.EnablePixieInputs();
        GameManager.Instance.DisableUIInputs();
    }

    void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;

        Time.timeScale = inventoryOpen ? 0 : 1;
        inventory.SetActive(inventoryOpen);

        if(!inventoryOpen)
            for (int i = 0; i < inventoryGameobjects.Length; i++)
            {
                if (i == 0) inventoryGameobjects[i].gameObject.SetActive(true);
                else inventoryGameobjects[i].SetActive(false);
            }

        inventoryIndex = 0;
    }

    void Inventory()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryGameobjects[inventoryIndex].SetActive(false);

            if (inventoryIndex == 0) inventoryIndex = inventoryGameobjects.Length - 1;
            else inventoryIndex--;
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            inventoryGameobjects[inventoryIndex].SetActive(false);

            if (inventoryIndex == inventoryGameobjects.Length - 1) inventoryIndex = 0;
            else inventoryIndex++;
        }

        if (inventoryGameobjects[inventoryIndex] != map && map.activeSelf) map.SetActive(false);
        inventoryGameobjects[inventoryIndex].SetActive(true);
    }

    void UpdateMoneyTexts()
    {
        moneyText.text = playerStorage.Coins.ToString();
        moneyTextItems.text = playerStorage.Coins.ToString();
    }

    void AddItemToInventory(Items item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            Image _img = itemSlots[i].GetComponent<Image>();
            if (_img.sprite != null) continue;

            itemSlots[i].SetActive(true);
            _img.sprite = GameManager.Instance.GetItemSprite(item);
            return;
        }
    }

    void ControllerConected()
    {
        if (EventSystem.current.alreadySelecting) return;

        if(menuOpen) EventSystem.current.SetSelectedGameObject(menu.transform.GetChild(0).gameObject);
    }

    void AddBoosterToPool(Boosters booster)
    {
        boostersHandler.AddBoosterUnequipped(booster);
    }

    void EquipBooster(Boosters booster)
    {
        boostersHandler.AddBoosterEquipped(booster);
    }

    private void OnEnable()
    {
        controlsInput.Menu.Enable();
        GameManager.Instance.ControllerConected += ControllerConected;
    }
    private void OnDisable()
    {
        controlsInput.Menu.Disable();
        GameManager.Instance.ControllerConected -= ControllerConected;
    }

    void OnEnableUIInputs() => controlsInput.PlayerUI.Enable();
    void OnDisableUIInputs() => controlsInput.PlayerUI.Disable();

    #region InputNotUsed
    public void OnRightClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        
    }
    #endregion
    public void OnClick(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.currentController != Controllers.Controller) return;

        if (context.performed)
        {
            GameObject _selected = EventSystem.current.currentSelectedGameObject;
            if (_selected != null && _selected.activeSelf)
            {
                if(_selected.TryGetComponent(out IClickable clickable))
                {
                    clickable.OnSelected();
                }
            }
        }
    }

    public void OnToggleMenu(InputAction.CallbackContext context)
    {
        if (inventoryOpen) return;

        if (context.started)
        {
            ToggleMenu();
            if (menuOpen)
            {
                if(GameManager.Instance.currentController == Controllers.Controller) EventSystem.current.SetSelectedGameObject(menu.transform.GetChild(0).gameObject);
                GameManager.Instance.DisablePlayerInputs();
                GameManager.Instance.DisablePixieInputs();
                GameManager.Instance.EnableUIInputs();
            }
            else
            {
                GameManager.Instance.EnablePlayerInputs();
                GameManager.Instance.EnablePixieInputs();
                GameManager.Instance.DisableUIInputs();
            }
        }
    }
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (menuOpen) return;

        if (context.started)
        {
            ToggleInventory();
            if (inventoryOpen)
            {
                GameManager.Instance.DisablePlayerInputs();
                GameManager.Instance.EnableUIInputs();
            }
            else
            {
                GameManager.Instance.EnablePlayerInputs();
                GameManager.Instance.DisableUIInputs();
            }
        }
    }
    public void OnNextPage(InputAction.CallbackContext context)
    {
        if (!inventoryOpen) return;

        if (context.started)
        {
            inventoryGameobjects[inventoryIndex].SetActive(false);

            if (inventoryIndex == inventoryGameobjects.Length - 1) inventoryIndex = 0;
            else inventoryIndex++;

            if (inventoryGameobjects[inventoryIndex] != map && map.activeSelf) map.SetActive(false);
            inventoryGameobjects[inventoryIndex].SetActive(true);
        }
    }

    public void OnPreviousPage(InputAction.CallbackContext context)
    {
        if (!inventoryOpen) return;

        if(context.started)
        {
            inventoryGameobjects[inventoryIndex].SetActive(false);

            if (inventoryIndex == 0) inventoryIndex = inventoryGameobjects.Length - 1;
            else inventoryIndex--;

            if (inventoryGameobjects[inventoryIndex] != map && map.activeSelf) map.SetActive(false);
            inventoryGameobjects[inventoryIndex].SetActive(true);
        }
    }

    public void OnMap(InputAction.CallbackContext context)
    {
        if (inventoryOpen) return;

        if (context.started)
        {
            inventory.SetActive(true);
            for (int i = 0; i < inventoryGameobjects.Length; i++)
            {
                inventoryGameobjects[i].SetActive(false);
            }
            map.SetActive(true);
        }
        else if(context.canceled)
        {
            inventory.SetActive(false);
            map.SetActive(false);
        }
    }
}
