using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    PlayerController playerController;

    HealthSystem healthSystem;

    [SerializeField] Image[] healthOrbs;

    [Header("MENU")]
    [SerializeField] GameObject menu;
    bool menuOn;
    int menuIndex, menuLength;
    GameObject[] menuGameobjects;

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
    [SerializeField] GameObject[] inventorySlots;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        playerController = GetComponent<PlayerController>();

        //Set starting UI
        map.SetActive(false);
        menu.SetActive(false);

        //Set menu child objects
        menuLength = menu.transform.childCount;
        menuGameobjects = new GameObject[menuLength];
        for (int i = 0; i < menuLength; i++)
        {
            menuGameobjects[i] = menu.transform.GetChild(i).gameObject;
            menuGameobjects[i].SetActive(false);
        }
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

            if (menuIndex == 0) menuIndex = menuLength - 1;
            else menuIndex--;
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            menuGameobjects[menuIndex].SetActive(false);

            if (menuIndex == menuLength - 1) menuIndex = 0;
            else menuIndex++;
        }

        if (menuGameobjects[menuIndex] != map && map.activeSelf) map.SetActive(false);
        menuGameobjects[menuIndex].SetActive(true);
    }
}
