using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

[System.Flags]
public enum Phases { None = 0, Initial = 1, Mid = 1 << 1, Final = 1 << 2}
public class Boss : MonoBehaviour
{
    Phases currentPhase;

    [SerializeField] int initialLife, finalLife;
    int currentLife;

    [Header("ATTACKS")]
    [SerializeField] List<Attack> attacks;
    List<Attack> pastAttacks, initialAttacks, midAttacks, finalAttacks;
    Attack currentAttack;
    bool attackSelected, attackDone, attacking;

    Transform player;
    Animator anim;
    Rigidbody2D rb;
    Transform cachedTr;

    [SerializeField] float movSpeed;
    bool inIdle;
    float idleTimer = 0, idleTime;


    [SerializeField] GameObject bossCanvas;

    int direction;

    [Header("JUMP ATTACK")]
    [SerializeField] float jumpSpeed;

    [Header("THROW ROCKS")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Transform minPoint, maxPoint;
    [SerializeField] private float rockSpeed;
    private bool throwingRocks, inPos;

    [Header("WAVE ATTACK")]
    [SerializeField] GameObject waveAttackPrefab;
    [SerializeField] Transform waveAttackPos;
    [SerializeField] float waveAttackSpeed;

    [SerializeField] private Transform midPhasePos;
    Vector3 initialPosition;

    [SerializeField] Slider healthSlider;
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] private float distanceToWall;
    bool canMove;

    [SerializeField] private Transform rightEnemySpawnPoint, leftEnemySpawnPoint;
    [SerializeField] private GameObject[] enemiesPrefab;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cachedTr = transform;

        player = FindObjectOfType<PlayerController>().transform;

        currentPhase = Phases.None;

        SeparateAttacksIntoPhases();

        initialPosition = cachedTr.position;

        bossCanvas.SetActive(false);
        UpdateUI();

        GameManager.Instance.OnStartBossFight += BeginFight;
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

        UpdateAnim();
        UpdateUI();
    }

    private void FixedUpdate()
    {
        if (Physics2D.Raycast(transform.position + new Vector3(0, 1, 0), new Vector2(direction, 0), distanceToWall, LayerMask.GetMask("Ground"))) canMove = false;
        else canMove = true;

        if(currentAttack == null) return;

        if (currentAttack.isHitting) AttackHit();       
    }

    void BeginFight()
    {
        anim.SetTrigger("BeginFight");
        bossCanvas.SetActive(true);
        ChangePhase(Phases.Initial);
    }
        
    #region Phases
    void ChangePhase(Phases nextPhase)
    {
        switch (nextPhase)
        {
            case Phases.Initial:
                healthSlider.maxValue = initialLife;
                currentLife = initialLife;
                break;

            case Phases.Mid:
                healthSlider.gameObject.SetActive(false);
                anim.SetTrigger("Rage");
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = false;
                }
                StartCoroutine(Co_LeaveStage());
                break;

            case Phases.Final:
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = true;
                }
                healthSlider.gameObject.SetActive(true);
                anim.SetTrigger("Rage");
                healthSlider.maxValue = finalLife;
                currentLife = finalLife;
                anim.speed *= 1.2f;
                movSpeed *= 1.2f;
                StartCoroutine(Co_EnterStage());
                break;
        }

        currentPhase = nextPhase;
    }

    void InitialPhase()
    {
        if (currentLife <= 0)
        {
            ChangePhase(Phases.Mid);
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

            if (canMove && Vector2.Distance(transform.position, player.position) > currentAttack.range)
            {
                MoveTowardsPlayer();
            }
            else if(!attacking)
            {
                ExecuteAttack();
            }
        }
    }

    IEnumerator Co_LeaveStage()
    {
        inPos = false;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > .9f);

        anim.SetTrigger("Idle");

        while(Vector3.Distance(cachedTr.position, midPhasePos.position) > .5f)
        {
            Vector3 _direction = midPhasePos.position - cachedTr.position;
            rb.velocity = _direction.normalized * movSpeed;

            Vector3 z = Vector3.MoveTowards(cachedTr.position, midPhasePos.position, movSpeed * Time.deltaTime);
            cachedTr.position = new Vector3(cachedTr.position.x, cachedTr.position.y, z.z);
           
            int dir = cachedTr.position.x > midPhasePos.position.x ? -1 : 1;

            if (dir == 1) cachedTr.eulerAngles = new Vector3(0f, 0f, 0f);
            else cachedTr.eulerAngles = new Vector3(0f, 180f, 0);
            yield return 0;
        }

        rb.velocity = Vector2.zero;

        inPos = true;
        StartCoroutine(Co_SpawnEnemies());
    }

    IEnumerator Co_EnterStage()
    {
        inPos = false;
        anim.SetTrigger("Idle");

        while(Vector3.Distance(cachedTr.position, initialPosition) > .5f)
        {
            Vector3 _direction = initialPosition - cachedTr.position;
            rb.velocity = _direction.normalized * movSpeed;

            Vector3 z = Vector3.MoveTowards(cachedTr.position, initialPosition, movSpeed * Time.deltaTime);
            cachedTr.position = new Vector3(cachedTr.position.x, cachedTr.position.y, z.z);
           
            int dir = cachedTr.position.x > initialPosition.x ? -1 : 1;

            if (dir == 1) cachedTr.eulerAngles = new Vector3(0f, 0f, 0f);
            else cachedTr.eulerAngles = new Vector3(0f, 180f, 0);
            yield return 0;
        }

        rb.velocity = Vector2.zero;

        inPos = true;
        anim.SetTrigger("Rage");
    }

    IEnumerator Co_SpawnEnemies()
    {
        bool rightSpawnPoint = true;
        for (int i = 0; i < 3; i++)
        {
            GameObject clon = Instantiate(enemiesPrefab[0]);
            if (rightSpawnPoint)
            {
                clon.transform.position = rightEnemySpawnPoint.position;
            }
            else
            {
                clon.transform.position = leftEnemySpawnPoint.position;
            }
            rightSpawnPoint = !rightSpawnPoint;
            yield return new WaitForSeconds(7);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject clon = Instantiate(enemiesPrefab[1]);
            if (rightSpawnPoint)
            {
                clon.transform.position = rightEnemySpawnPoint.position;
            }
            else
            {
                clon.transform.position = leftEnemySpawnPoint.position;
            }
            rightSpawnPoint = !rightSpawnPoint;
            yield return new WaitForSeconds(5);
        }

        for (int i = 0; i < 10; i++)
        {
            int r = Random.Range(0, 3);
            GameObject clon = Instantiate(enemiesPrefab[r]);
            if (rightSpawnPoint)
            {
                clon.transform.position = rightEnemySpawnPoint.position;
            }
            else
            {
                clon.transform.position = leftEnemySpawnPoint.position;
            }
            rightSpawnPoint = !rightSpawnPoint;
            yield return new WaitForSeconds(2);
        }

        yield return new WaitUntil(() => FindObjectsOfType<Enemy>().Length == 0);

        ChangePhase(Phases.Final);
    }
    private void MidPhase()
    {
        if(!inPos) return;

        throwingRocks = true;
    }
    private void FinalPhase()
    {
        if (currentLife <= 0)
        {
            anim.SetTrigger("Die");
            GameManager.Instance.BossDefeated();
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

            if (canMove && Vector2.Distance(transform.position, player.position) > currentAttack.range)
            {
                MoveTowardsPlayer();
            }
            else if(!attacking)
            {
                ExecuteAttack();
            }
        }
    }
    #endregion
    void MoveTowardsPlayer()
    {
        int dir = cachedTr.position.x > player.position.x ? -1 : 1;

        if (dir == 1) cachedTr.eulerAngles = new Vector3(0f, 0f, 0f);
        else cachedTr.eulerAngles = new Vector3(0f, 180f, 0);

        rb.velocity = movSpeed * cachedTr.right;
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
        currentAttack.hasHit = false;
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
        if(canMove) rb.velocity = new Vector2(jumpSpeed * direction, rb.velocity.y);
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

    public void CreateRock()
    {
        float x = Random.Range(minPoint.position.x, maxPoint.position.x);
        GameObject clon = Instantiate(rockPrefab, new Vector2(x, minPoint.position.y), Quaternion.identity);
        clon.GetComponent<Rigidbody2D>().velocity = Vector2.down * rockSpeed;
    }

    public void WaveAttack()
    {
        GameObject _clon = Instantiate(waveAttackPrefab);
        _clon.transform.position = waveAttackPos.position;
        _clon.transform.rotation = transform.rotation;
        _clon.GetComponent<Rigidbody2D>().velocity = transform.right * waveAttackSpeed;
    }

    void UpdateAnim()
    {
        anim.SetFloat("xVel", rb.velocity.magnitude);
        anim.SetBool("Throwing", throwingRocks);
    }

    void UpdateUI()
    {
        if(bossCanvas.activeInHierarchy) healthSlider.value = currentLife;
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

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0), transform.position + new Vector3(0, 1, 0) + new Vector3(direction * distanceToWall, 0, 0));
    }

    public void GetHurt(int damage)
    {
        anim.SetTrigger("Hurt");
        currentLife -= damage;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStartBossFight -= BeginFight;
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
