using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;

    [SerializeField] protected float patrolSpeed, chaseSpeed, attackCD, attackDistance;
    protected Transform player;

    protected enum States { Patrolling, Chasing }
    States states;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        states = States.Patrolling;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        switch (states)
        {
            case States.Patrolling:
                Patrolling();
                break;
            case States.Chasing:
                Chasing();
                break;
        }
    }

    protected virtual void Patrolling()
    {

    }

    protected virtual void Chasing()
    {

    }

    protected virtual void Attack()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController == null) return;

        states = States.Chasing;
        player = playerController.transform;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController == null) return;

        states = States.Patrolling;
    }
}
