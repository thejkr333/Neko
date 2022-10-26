using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    Rigidbody2D rb;
    public int maxHealth;
    [SerializeField] float knockbackForce;

    [HideInInspector]
    public int currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public void GetHurt(int damage, Vector2 direction)
    {
        currentHealth -= damage;
        Knockback(direction);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Knockback(Vector2 direction)
    {
        if (rb == null) return;

        float initialGravityScale = rb.gravityScale;
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null) { playerController.DisableMovement(); playerController.EnableMovement(0.2f); }
        rb.gravityScale = 0;
        rb.AddForce(direction * -1f * knockbackForce, ForceMode2D.Impulse);
        rb.gravityScale = initialGravityScale;      
    }

    void Die()
    {
        //Destroy(gameObject);
    }
}
