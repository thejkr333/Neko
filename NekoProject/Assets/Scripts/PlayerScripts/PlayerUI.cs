using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    HealthSystem healthSystem;

    [SerializeField] Image[] healthOrbs;

    [Header("MENU")]
    [SerializeField] GameObject menu;
    bool menuOn;
    int menuIndex, menuLength;

    [Header("MAP")]
    [SerializeField] GameObject map;
    bool mapOn;

    GameObject[] menuGameobjects;
 
    PlayerController playerController;

    private void Awake()
    {
        map.SetActive(false);
        menu.SetActive(false);

        menuLength = menu.transform.childCount;
        menuGameobjects = new GameObject[menuLength];
        for (int i = 0; i < menuLength; i++)
        {
            menuGameobjects[i] = menu.transform.GetChild(i).gameObject;
            menuGameobjects[i].SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthOrbs();

        if (Input.GetKeyDown(KeyCode.Escape)) ToggleMenu();
        if (!menuOn && Input.GetKeyDown(KeyCode.M)) ToggleMap();

        if (menuOn) Menu();

        Time.timeScale = menuOn ? 0 : 1;
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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            menuGameobjects[menuIndex].SetActive(false);

            if (menuIndex == 0) menuIndex = menuLength - 1;
            else menuIndex--;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            menuGameobjects[menuIndex].SetActive(false);

            if (menuIndex == menuLength - 1) menuIndex = 0;
            else menuIndex++;
        }

        menuGameobjects[menuIndex].SetActive(true);
    }
}
