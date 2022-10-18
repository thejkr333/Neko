using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    HealthSystem healthSystem;

    [SerializeField] Image[] healthOrbs;
    // Start is called before the first frame update
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthOrbs();
    }

    void UpdateHealthOrbs()
    {
        for (int i = 0; i < healthSystem.maxHealth; i++)
        {
            if (i < healthSystem.currentHealth) healthOrbs[i].gameObject.SetActive(true);
            else healthOrbs[i].gameObject.SetActive(false);
        }
    }
}
