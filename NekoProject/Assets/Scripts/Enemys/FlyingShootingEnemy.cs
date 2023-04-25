using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FlyingShootingEnemy : Enemy
{
    [SerializeField] Vector2 patrolInterval;

    float patrolTimer;
    private Vector2 patrolDirection = Vector2.right;
    private Vector2 attackDirection;

    [Header("SHOOTING")]
    [SerializeField] Transform shootingPoint;
    [SerializeField] GameObject projectilePrefab;
    float attackTimer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<CircleCollider2D>().radius = chaseDistance;
        attackTimer = 0;
    }

    protected override void Patrol()
    {
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
        }
    }

    protected override void Chase()
    {
        // Move towards player
        Vector2 playerDirection = playerTransform.position - transform.position;
        rb.velocity = playerDirection.normalized * chaseSpeed;

        // Check if player is close enough to attack
        if (Vector2.Distance(transform.position, playerTransform.position) < attackDistance)
        {
            attackDirection = playerDirection.normalized;
            ChangeState(States.Attacking);
        }
    }

    protected override void Attack()
    {
        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0f)
        {
            Shoot();
            attackTimer = attackCD;
        }
    }

    void Shoot()
    {
        GameObject clon = Instantiate(projectilePrefab);
        clon.transform.position = shootingPoint.position;
        clon.transform.forward = (playerTransform.position - transform.position).normalized;
    }

    //need to change this
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == States.Patrolling) patrolTimer = 0;
        else if (state == States.Attacking)
        {
            attackTimer = 0;
            // Stop attacking if colliding with something
            ChangeState(States.Chasing);
            // Stop when colliding with something
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (state == States.Chasing)
        {
            // Return to patrolling if player is out of range
            ChangeState(States.Patrolling);
        }

        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

    }
}
