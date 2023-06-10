////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class SimpleEnemy : Enemy
{
    [Tooltip("Only values '-1' or '1")]
    [SerializeField] int initialDir;
    int dir;

    bool attacking = true;

    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadius;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        dir = initialDir;
    }

    protected override void Patrol()
    {
        base.Patrol();

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
        base.Chase();

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
        if (!attacking)
        {
            anim.SetTrigger("Attack");
            attacking = true;
        }

        LookToPlayer();

        rb.velocity = Vector2.zero;

        if (Mathf.Abs(playerTransform.position.x - transform.position.x) > attackDistance)
        {
            ChangeState(States.Chasing);
            attacking = false;
        }
    }

    public void CheckHit()
    {
        Collider2D[] _hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);
        if (_hits.Length == 0) return;

        foreach (var hit in _hits)
        {
            if(hit.TryGetComponent(out PlayerController playerController))
            {
                Vector2 _hitDir = new(playerController.transform.position.x < transform.position.x ? -1 : 1, 1);
                playerController.GetComponent<HealthSystem>().GetHurt(damage, _hitDir);
            }
        }
    }

    public void FinishAttack()
    {
        attacking = false;
    }

    public override void Die()
    {
        base.Die();
        AudioManager.Instance.PlaySound("Die_SimpleEnemy");
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D (collision);

        if(state == States.Patrolling) dir *= -1;
    }
}
