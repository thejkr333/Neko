using System.Collections;
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
    Attack nextAttack;
    bool attackSelected, attackDone;

    Transform player;
    Animator anim;
    Rigidbody2D rb;

    [SerializeField] float movSpeed;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<PlayerController>().transform;
        currentPhase = Phases.Initial;

        currentLife = initialLife;
        SeparateAttacksIntoPhases();
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
    void InitialPhase()
    {
        if (currentLife <= 0)
        {
            currentPhase = Phases.Mid;
            return;
        }

        if (!attackSelected) nextAttack = DecideAttack(initialAttacks);
        else
        {
            if (attackDone)
            {
                attackSelected = false;
                return;
            }

            if (Vector2.Distance(transform.position, player.position) > nextAttack.range)
            {
                MoveTowardsPlayer();
            }
            else
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
        anim.SetTrigger("Idle");
        rb.velocity = Vector2.zero;
    }

    Attack DecideAttack(List<Attack> possibleAttacks)
    {
        int randomAttack = Random.Range(0, possibleAttacks.Count);
        nextAttack = possibleAttacks[randomAttack];
        attackSelected = true;
        attackDone = false;
        return nextAttack;
    }

    void ExecuteAttack()
    {
        rb.velocity = Vector3.zero;
        anim.SetTrigger(nextAttack.attackName);
    }

    public void EndAttack()
    {
        Idle();
        pastAttacks.Add(nextAttack);
        nextAttack = null;
        attackSelected = false;
        attackDone = true;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < attacks.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attacks[i].range);
        }
    }
}

[System.Serializable]
public class Attack
{
    public string attackName;
    public int damage;
    public float range;
    public Phases phase;

    public Attack(int _damage, float _range)
    {
        attackName = this.GetType().Name;
        damage = _damage;
        range = _range;
    }
}
