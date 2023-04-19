using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : Enemy
{
    [Tooltip("Only values '-1' or '1")]
    [SerializeField] int initialDir;
    int dir;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<BoxCollider2D>().size = new Vector2(chaseDistance, GetComponentInChildren<BoxCollider2D>().size.y);
        dir = initialDir;
    }

    protected override void Patrol()
    {
        rb.velocity = new Vector2(patrolSpeed * dir, rb.velocity.y);
    }

    protected override void Chase()
    {
        //if player is in attack range attack, else keep chasing;
        if(Mathf.Abs(playerTransform.position.x - transform.position.x) <= attackDistance)
        {
            Attack();
        }
        else
        {
            int _dir;
            //follow player
            if (playerTransform.position.x - transform.position.x <= 0) _dir = -1;
            else _dir = 1;

            dir = _dir;

            rb.velocity = new Vector2(chaseSpeed * dir, rb.velocity.y);
        }
    }

    protected override void Attack()
    {
        rb.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

        if (playerController == null) dir *= -1;
    }
}
