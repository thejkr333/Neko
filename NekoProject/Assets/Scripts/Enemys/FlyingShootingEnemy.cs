using UnityEngine;

public class FlyingShootingEnemy : FlyingEnemy
{
    [Header("SHOOTING")]
    [SerializeField] Transform shootingPoint;
    [SerializeField] GameObject projectilePrefab;
    float attackTimer;
    [SerializeField] float projectileSpeed;


    protected override void Attack()
    {
        LookToPlayer();

        // Check if player has left attacking range
        if (Vector2.Distance(transform.position, playerTransform.position) > attackDistance)
        {
            ChangeState(States.Chasing);
            attackTimer = 0;
            return;
        }

        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0f)
        {
            rb.velocity = Vector2.zero;
            attackDirection = (playerTransform.position - transform.position).normalized;
            Shoot();
            attackTimer = attackCD;
        }
        else if(attackTimer <= attackCD - 1)
        {
            rb.velocity = rb.velocity = attackDirection.normalized * chaseSpeed;
        }
    }

    void Shoot()
    {
        GameObject clon = Instantiate(projectilePrefab);
        clon.transform.position = shootingPoint.position;
        if (clon.TryGetComponent(out Projectile projectile))
        {
            projectile.direction = attackDirection;
            projectile.speed = projectileSpeed;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
