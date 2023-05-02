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

        dir = initialDir;
    }

    protected override void Patrol()
    {
        if (playerTransform != null)
        {
            if (Mathf.Abs(playerTransform.position.x - transform.position.x) <= chaseDistance)
            {
                ChangeState(States.Chasing);
                return;
            }
        }

        if (dir == 1) transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        else transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);

        rb.velocity = new Vector2(patrolSpeed * dir, rb.velocity.y);
    }

    protected override void Chase()
    {
        LookToPlayer();

        //if player has left chase distance
        if (Mathf.Abs(playerTransform.position.x - transform.position.x) > chaseDistance) 
        { 
            ChangeState(States.Patrolling); 
            return; 
        }

        //if player is in attack range attack, else keep chasing;
        if (Mathf.Abs(playerTransform.position.x - transform.position.x) <= attackDistance)
        {
            ChangeState(States.Attacking);
            return;
        }

        //follow player
        if (facingRight) dir = 1;
        else dir = -1;

        rb.velocity = new Vector2(chaseSpeed * dir, rb.velocity.y);
    }

    protected override void Attack()
    {
        LookToPlayer();

        rb.velocity = Vector2.zero;

        if (Mathf.Abs(playerTransform.position.x - transform.position.x) > attackDistance) ChangeState(States.Chasing);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(state == States.Patrolling) dir *= -1;
    }
}
