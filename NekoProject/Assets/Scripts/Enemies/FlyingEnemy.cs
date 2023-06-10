////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] private float attackSpeed;
    protected Vector2 attackDirection;

    [Header("PATROLLING")]
    float patrolTimer;
    private Vector2 patrolDirection = Vector2.right;
    [SerializeField] Vector2 patrolInterval;
    [SerializeField] LayerMask obstacles;
    float obstacleDistance = 4;
    float chasingTimer;
    float waitForChase = 3;

    protected bool shootingEnemy;
    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<CircleCollider2D>().radius = chaseDistance;
        shootingEnemy = false;
    }

    protected override void Patrol()
    {
        base.Patrol();

        if (playerTransform != null)
        {
            if (Vector2.Distance(playerTransform.position, transform.position) <= chaseDistance && chasingTimer <= 0)
            {
                ChangeState(States.Chasing);
                return;
            }
        }

        // Move in patrol direction
        rb.velocity = patrolDirection * patrolSpeed;

        // Decrement patrol timer
        patrolTimer -= Time.deltaTime;

        // Change direction if timer is up
        if (patrolTimer <= 0f)
        {
            // Generate new patrol timer and direction
            patrolTimer = Random.Range(patrolInterval.x, patrolInterval.y);
            patrolDirection = Random.insideUnitCircle.normalized;

            if (Physics2D.Raycast(transform.position, transform.up, obstacleDistance, obstacles) && patrolDirection.y > 0) patrolDirection.y *= -1;
            if (Physics2D.Raycast(transform.position, -transform.up, obstacleDistance, obstacles) && patrolDirection.y < 0) patrolDirection.y *= -1;
            if (Physics2D.Raycast(transform.position, transform.right, obstacleDistance, obstacles) && patrolDirection.x > 0) patrolDirection.x *= -1;
            if (Physics2D.Raycast(transform.position, -transform.right, obstacleDistance, obstacles) && patrolDirection.x < 0) patrolDirection.x *= -1;
        }

        chasingTimer -= Time.deltaTime;
    }

    protected override void Chase()
    {
        base.Chase();

        LookToPlayer();

        Vector2 playerDirection = playerTransform.position - transform.position;

        if(Vector2.Distance(transform.position, playerTransform.position) > chaseDistance)
        {
            ChangeState(States.Patrolling);
            return;
        }

        // Check if player is close enough to attack
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackDistance)
        {
            attackDirection = playerDirection.normalized;
            ChangeState(States.Attacking);
            if(!shootingEnemy)
            {
                AudioManager.Instance.PlaySound("Attack_FlyingEnemy");
                anim.SetTrigger("Attack");
            }
            return;
        }

        // Move towards player
        rb.velocity = playerDirection.normalized * chaseSpeed;
    }

    protected override void Attack()
    {
        base.Attack();

        LookToPlayer();

        if (Vector2.Distance(transform.position, playerTransform.position) > attackDistance)
        {
            ChangeState(States.Chasing);
            return;
        }

        // Move towards player with attack speed
        rb.velocity = attackDirection * attackSpeed;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (state == States.Patrolling) patrolTimer = 0;
        
        if(collision.gameObject.TryGetComponent(out PlayerController playerController))
        {
            anim.SetTrigger("Idle");
            ChangeState(States.Patrolling);
            chasingTimer = waitForChase;
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void Die()
    {
        base.Die();
        AudioManager.Instance.PlaySound("Die_FlyingEnemy");
    }
}
