////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    Rigidbody2D rb;
    public int maxHealth;
    [SerializeField] float knockbackForce;

    [HideInInspector]
    public int currentHealth;

    [SerializeField] GameObject coinPrefab;
    [SerializeField] float coinForce;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public void GetHurt(int damage, Vector2 direction)
    {
        if(TryGetComponent(out PlayerUI playerUI)) 
        {
            if (playerUI.ExtraHealthOn) playerUI.LooseExtraHealth();
            else currentHealth -= damage;
        }
        else currentHealth -= damage;

        Knockback(direction.normalized);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Knockback(Vector2 direction)
    {
        if (rb == null) return;

        if (TryGetComponent(out PlayerController playerController))
        {
            AudioManager.Instance.PlaySound("Hiss");
            playerController.GetHit();
        }
        else if(TryGetComponent(out Enemy enemy))
        {
            AudioManager.Instance.PlaySound("EnemyHit");
            enemy.DisableMovement();
            enemy.EnableMovement(.3f);
        }
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    void Die()
    {
        if (TryGetComponent(out PlayerController playerController))
        {
            playerController.Die();
            return;
        }

        if (TryGetComponent(out Enemy enemy))
        {
            enemy.Die();
            SpawnCoins(enemy.coinsToSpawn);
            return;
        }
    }

    void SpawnCoins(int coinAmount)
    {
        for (int i = 0; i < coinAmount; i++)
        {
            GameObject clon = Instantiate(coinPrefab);
            clon.transform.position = transform.position;

            float x = Random.Range(-1, 1);
            float y = Random.Range(0, 1);
            Vector2 dir = new Vector2(x, y).normalized;
            clon.GetComponent<Rigidbody2D>().AddForce(dir * coinForce, ForceMode2D.Impulse);
        }
    }
}
