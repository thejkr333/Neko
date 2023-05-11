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
        if (TryGetComponent(out PlayerController playerController))
        {
            playerController.DisableMovement();
            playerController.EnableMovement(0.2f); 
        }
        else if(TryGetComponent(out Enemy enemy))
        {
            enemy.DisableMovement();
            enemy.EnableMovement(.2f);
        }
        rb.gravityScale = 0;
        rb.AddForce(direction * -1f * knockbackForce, ForceMode2D.Impulse);
        rb.gravityScale = initialGravityScale;      
    }

    void Die()
    {
        if (TryGetComponent(out PlayerController playerController))
        {

        }

        if (TryGetComponent(out Enemy enemy))
        {
            SpawnCoins(enemy.coinsToSpawn);
        }
        Destroy(gameObject);
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
