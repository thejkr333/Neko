using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] float maxHealth;

    [SerializeField] float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void GetHurt(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        //Destroy(gameObject);
    }
}
