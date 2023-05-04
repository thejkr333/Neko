using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;

    [SerializeField] protected float patrolSpeed, chaseSpeed, attackCD, chaseDistance, attackDistance;
    protected Transform playerTransform;

    protected enum States { Patrolling, Chasing , Attacking}
    [SerializeField] protected States state;

    protected bool facingRight;

    public int coinsToSpawn;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        state = States.Patrolling;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        switch (state)
        {
            case States.Patrolling:
                Patrol();
                break;
            case States.Chasing:
                Chase();
                break;
            case States.Attacking:
                Attack();
                break;
        }
    }

    protected virtual void Patrol()
    {

    }

    protected virtual void Chase()
    {
        
    }

    protected virtual void Attack()
    {
        
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerTransform != null) return;

        if(collision.TryGetComponent(out PlayerController playerController)) playerTransform = playerController.transform;
    }

    protected virtual void ChangeState(States nextState)
    {
        state = nextState;
    }

    protected void LookToPlayer()
    {
        if (playerTransform.position.x < transform.position.x) facingRight = false;
        else facingRight = true;

        if (facingRight)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

}
