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
        if (!canMove) return;

        LookToPlayer();

        float _distance = Vector2.Distance(transform.position, playerTransform.position);
        // Check if player has left attacking range
        if (_distance > attackDistance)
        {
            ChangeState(States.Chasing);
            return;
        }

        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0f)
        {
            rb.velocity = Vector2.zero;
            attackDirection = (playerTransform.position - transform.position).normalized;
            anim.SetTrigger("Shoot");
            attackTimer = attackCD;
        }
        else if(attackTimer <= attackCD - 1)
        {
            if(_distance > attackDistance * .5f) rb.velocity = attackDirection.normalized * chaseSpeed;
            else rb.velocity = -attackDirection.normalized * chaseSpeed;
        }
    }

    public void CreateBullet()
    {
        AudioManager.Instance.PlaySound("Attack_ShootingEnemy");
        GameObject clon = Instantiate(projectilePrefab);
        clon.transform.position = shootingPoint.position;
        if (clon.TryGetComponent(out Projectile projectile))
        {
            projectile.direction = attackDirection;
            projectile.speed = projectileSpeed;
        }
    }

    public override void Die()
    {
        base.Die();
        AudioManager.Instance.PlaySound("Die_ShootingEnemy");
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
