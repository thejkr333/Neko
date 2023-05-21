using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum Phases { None = 0, Initial = 1, Mid = 1 << 1, Final = 1 << 2}
public class Boss : MonoBehaviour
{
    Phases currentPhase;

    [SerializeField] int initialLife, midLife, finalLife;
    int currentLife;

    [Header("ATTACKS")]
    [SerializeField] List<Attack> attacks;
    List<Attack> pastAttacks, initialAttacks, midAttacks, finalAttacks;
    Attack currentAttack;
    bool attackSelected, attackDone, attacking;

    Transform player;
    Animator anim;
    Rigidbody2D rb;

    [SerializeField] float movSpeed;
    bool inIdle;
    float idleTimer = 0, idleTime;

    int direction;

    [Header("JUMP ATTACK")]
    [SerializeField] float jumpSpeed;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<PlayerController>().transform;
        currentPhase = Phases.Initial;

        currentLife = initialLife;
        SeparateAttacksIntoPhases();

        Idle();
    }
    void SeparateAttacksIntoPhases()
    {
        pastAttacks = new();
        initialAttacks = new();
        midAttacks = new();
        finalAttacks = new();  

        foreach (Attack attack in attacks)
        {
            if ((attack.phase & Phases.Initial) != Phases.None)
            {
                initialAttacks.Add(attack);
            }
            if ((attack.phase & Phases.Mid) != Phases.None)
            {
                midAttacks.Add(attack);
            }
            if ((attack.phase & Phases.Final) != Phases.None)
            {
                finalAttacks.Add(attack);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        direction = transform.position.x > player.position.x ? -1 : 1;

        switch (currentPhase)
        {
            case Phases.Initial:
                InitialPhase();
                break;

            case Phases.Mid:
                MidPhase();
                break;

            case Phases.Final:
                FinalPhase();
                break;
        }
    }

    private void FixedUpdate()
    {
        if(currentAttack == null) return;

        if (currentAttack.isHitting) AttackHit();       
    }
    void InitialPhase()
    {
        if (currentLife <= 0)
        {
            currentPhase = Phases.Mid;
            return;
        }

        if (inIdle)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer > idleTime)
            {
                inIdle = false;
            }
            return;
        }

        if (!attackSelected) currentAttack = DecideAttack(initialAttacks);
        else
        {
            if (attackDone)
            {
                attackSelected = false;
                return;
            }

            if (Vector2.Distance(transform.position, player.position) > currentAttack.range)
            {
                MoveTowardsPlayer();
            }
            else if(!attacking)
            {
                ExecuteAttack();
            }
        }
    }

    private void MidPhase()
    {
        throw new System.NotImplementedException();
    }
    private void FinalPhase()
    {
        throw new System.NotImplementedException();
    }

    void MoveTowardsPlayer()
    {
        int dir = transform.position.x > player.position.x ? -1 : 1;

        if (dir == 1) transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else transform.eulerAngles = new Vector3(0f, 180f, 0);

        rb.velocity = movSpeed * transform.right;
    }

    void Idle()
    {
        inIdle = true;
        idleTimer = 0;
        idleTime = Random.Range(1f, 2f);
        anim.SetTrigger("Idle");
        rb.velocity = Vector2.zero;
    }

    Attack DecideAttack(List<Attack> possibleAttacks)
    {
        int randomAttack = Random.Range(0, possibleAttacks.Count);
        currentAttack = possibleAttacks[randomAttack];
        attackSelected = true;
        attackDone = false;
        return currentAttack;
    }

    void ExecuteAttack()
    {
        attacking = true;
        rb.velocity = Vector3.zero;
        anim.SetTrigger(currentAttack.attackName);
    }

    public void EndAttack()
    {
        Idle();
        pastAttacks.Add(currentAttack);
        currentAttack = null;
        attackSelected = false;
        attackDone = true;
        attacking = false;
    }

    public void JumpAttackMove()
    {
        rb.velocity = new Vector2(jumpSpeed * direction, rb.velocity.y);
    }

    public void StopBoss()
    {
        rb.velocity = Vector2.zero;
    }

    public void StartHit()
    {
        currentAttack.isHitting = true;
    }

    public void StopHit()
    {
        currentAttack.isHitting = false;
    }

    void AttackHit()
    {
        if (currentAttack.hasHit) return;

        Collider2D[] _col = Physics2D.OverlapCircleAll(currentAttack.hitPoint.position, currentAttack.radius);

        if (_col.Length < 0) return;
        for (int i = 0; i < _col.Length; i++)
        {
            if (_col[i].TryGetComponent(out PlayerController playerController))
            {
                if (playerController.Invincible) return;
                playerController.GetComponent<HealthSystem>().GetHurt(currentAttack.damage, Vector2.right * direction);
                currentAttack.hasHit = true;
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < attacks.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attacks[i].range);
        }

        for (int i = 0; i < attacks.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attacks[i].hitPoint.position, attacks[i].radius);
        }
    }
}

[System.Serializable]
public class Attack
{
    public string attackName;
    public int damage;
    public float range;
    public Transform hitPoint;
    public float radius;
    public Phases phase;
    public bool isHitting = false;
    public bool hasHit = false;

    public Attack(int _damage, float _range)
    {
        attackName = this.GetType().Name;
        damage = _damage;
        range = _range;
    }
}
