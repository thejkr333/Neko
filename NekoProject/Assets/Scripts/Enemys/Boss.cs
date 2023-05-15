using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum Phases { None = 0, Initial = 1, Mid = 1 << 1, Final = 1 << 2}
public class Boss : MonoBehaviour
{
    Phases currentPhase;

    [Header("ATTACKS")]
    int initialLife, midLife, finalLife;
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
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<PlayerController>().transform;
        currentPhase = Phases.Initial;

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
                break;

            case Phases.Final:
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        int dir = transform.position.x > player.position.x ? -1 : 1;
        rb.velocity = dir * movSpeed * transform.right;
    }

    void Idle()
    {
        rb.velocity = Vector2.zero;
    }

    void InitialPhase()
    {
        if(!attackSelected) nextAttack = DecideAttack(initialAttacks);
        else
        {
            if(attackDone)
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

    Attack DecideAttack(List<Attack> possibleAttacks)
    {
        int randomAttack = Random.Range(0, possibleAttacks.Count);
        nextAttack = possibleAttacks[randomAttack];
        attackSelected = true;
        return nextAttack;
    }

    void ExecuteAttack()
    {
        anim.SetTrigger(nextAttack.attackName);
    }

    public void EndAttack()
    {
        pastAttacks.Add(nextAttack);
        nextAttack = null;
        attackSelected = false;
        attackDone = true;
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
