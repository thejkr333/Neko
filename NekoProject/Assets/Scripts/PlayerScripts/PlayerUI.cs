using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    HealthSystem healthSystem;

    [SerializeField] Image[] healthOrbs;

    [Header("MAP")]
    [SerializeField] GameObject map;
    bool mapOn;
 
    PlayerController playerController;

    private void Awake()
    {
        map.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.M)) ToggleMap();

        Time.timeScale = mapOn ? 0 : 1;
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
        if (!mapOn) return;     
    }
}
