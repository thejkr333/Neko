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
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController == null) return;

        ChangeState(States.Chasing);
        playerTransform = playerController.transform;
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController == null) return;

        ChangeState(States.Patrolling);
    }

    protected virtual void ChangeState(States nextState)
    {
        state = nextState;
    }
}
