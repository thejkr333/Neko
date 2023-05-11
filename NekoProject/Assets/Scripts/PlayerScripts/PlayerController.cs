using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sr;
    Rigidbody2D rb;
    PlayerStorage playerStorage;

    //Movement variables
    [Header("MOVEMENT")]
    bool movementDisabled;
    [SerializeField] float speed = 4f;
    float initialGravityScale, input_hor;
    public int Dir;

    //Jump Variables
    [Header("JUMP")]
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float counterJumpForce = 2f;
    bool isJumping, canDoubleJump, jumpKeyHeld;
    float jumpForce;

    //Attack variables
    [Header("ATTACK")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask jumpAttackHitLayer;
    [SerializeField] Transform attackPos, jumpAttackPos;
    [SerializeField] Vector2[] attackSize;
    [SerializeField] Vector2 jumpAttackSize;
    [SerializeField] float attackFrecuency = 0.5f;
    [SerializeField] float timeToCombo = 1f;
    [SerializeField] int attackDmg;
    [SerializeField] float attackKnockbackForce;
    [SerializeField] float jumpAttackKnockbackForce;
    bool canAttack, attacking;
    float timeSinceLastAttack;
    int lastAttack;

    //Dash variables
    [Header("DASH")]
    [SerializeField] float dashForce = 1500f;
    [SerializeField] float dashFrecuency = 1f;
    bool canDash, dashing;
    float timeSinceLastDash;

    //WallSlide variables
    [Header("WALLSLIDE")]
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform wallSlideContact;
    bool wallSliding;
    [SerializeField] float wallSlideSpeed;
    [SerializeField] float wallSlideCheckRadius;

    [Header("GROUND CHECK")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck_tr;
    [SerializeField] bool grounded;
    [SerializeField] float groundCheckRadius;

    [Header("PIXIE")]
    [SerializeField] Pixie pixie;
    float tpYPos;

    [Header("ANTMAN")]
    [SerializeField] float antmanSpeed = 8f;
    public float antmanCD;
    bool antman, canAntman;
    [HideInInspector] public float antmanTimer;

    [Header("SHIELD")]
    [SerializeField] GameObject shield;
    [SerializeField] float shieldActiveTime;
    public float shieldCD;
    bool shielding, canShield;
    float shieldActiveTimer;
    [HideInInspector] public float shieldCDTimer;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerStorage = GetComponent<PlayerStorage>();

        jumpForce = CalculateJumpForce(Physics2D.gravity.magnitude, jumpHeight);

        Dir = 1;

        movementDisabled = false;

        isJumping = false;
        canDoubleJump = true;
        jumpKeyHeld = false;

        lastAttack = -1;
        canAttack = true;

        antman = false;
        canAntman = true;

        shielding = false;
        canShield = true;

        initialGravityScale = rb.gravityScale;

        tpYPos = 1;
    }

    // Update is called once per frame
    void Update()
    {
        CheckJump();

        CheckAttack();

        if (playerStorage.ItemsUnlockedInfo[Items.Dash]) Dash();

        if (playerStorage.ItemsUnlockedInfo[Items.Shield]) Shield();

        if (playerStorage.ItemsUnlockedInfo[Items.Antman]) AntMan();

        CheckTP();

        UpdateAnim();
    }

    private void FixedUpdate()
    {
        if (isJumping)
        {
            if (dashing) return;

            if (!jumpKeyHeld || rb.velocity.y < 0)
            {
                rb.AddForce(Vector2.down * counterJumpForce, ForceMode2D.Force);
            }
        }

        CheckGround();

        if (playerStorage.ItemsUnlockedInfo[Items.WallSlide]) CheckWallSlide();

        Movement();
    }

    private void LateUpdate()
    {
        PlaySounds();
    }

    #region Movement 

    // Variables para el control del movimiento del jugador
    [HideInInspector] public bool controllingPlayerMovement;
    [HideInInspector] public float controllingDir = 0;
    public void ControlPlayer(float dir)
    {
        controllingPlayerMovement = true;
        controllingDir = dir;
    }

    public void Uncontrol()
    {
        controllingPlayerMovement = false;
        controllingDir = 0;
    }

    public void AddVelocityToRB(Vector2 newVelocity)
    { rb.velocity = newVelocity; }

    void Movement()
    {
        if (movementDisabled) return;

        // Si se le esta controlando
        if (controllingPlayerMovement)
            input_hor = controllingDir;
        else
            input_hor = Input.GetAxisRaw("Horizontal");

        float currentSpeed;
        if (antman) currentSpeed = antmanSpeed;
        else currentSpeed = speed;

        rb.velocity = new Vector2(input_hor * currentSpeed, rb.velocity.y);

        FlipSr();
    }
    void FlipSr()
    {
        if (input_hor != 0)
        {
            if (input_hor < 0) { Dir = -1; sr.transform.localScale = new Vector3(-1, 1, 0); }
            else { Dir = 1; sr.transform.localScale = new Vector3(1, 1, 0); }
        }
    }

    public void DisableMovement()
    {
        movementDisabled = true;
    }

    public void EnableMovement()
    {
        movementDisabled = false;
    }

    public void EnableMovement(float seconds)
    {
        Invoke(nameof(EnableMovement), seconds);
    }

    #endregion

    #region Jump

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dashing || attacking) return;

            jumpKeyHeld = true;
            if (grounded || wallSliding) Jump();
            else if (playerStorage.ItemsUnlockedInfo[Items.DoubleJump] && canDoubleJump) DoubleJump();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) jumpKeyHeld = false;
    }

    void Jump()
    {
        isJumping = true;
        anim.SetTrigger("Jump");
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    private void DoubleJump()
    {
        canDoubleJump = false;
        isJumping = true;

        anim.SetTrigger("Jump");
        //Stop velocity in y axis so it doesn't affect the new jump
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    public static float CalculateJumpForce(float gravityStrength, float jumpHeight)
    {
        return Mathf.Sqrt(2 * gravityStrength * jumpHeight * 1000f);
    }

    #endregion

    #region Attack

    void CheckAttack()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack >= attackFrecuency) canAttack = true;
        if (timeSinceLastAttack >= timeToCombo) lastAttack = -1;

        if (Input.GetMouseButtonDown(0))
        {
            if (!canAttack || dashing || wallSliding || antman || shielding) return;

            attacking = true;
            timeSinceLastAttack = 0;
            canAttack = false;

            //Check if jump attack
            if (Input.GetKey(KeyCode.S) && !grounded)
            {
                anim.SetTrigger("JumpAttack");
            }
            //Rest of attacks
            else
            {
                if (lastAttack == 0)
                {
                    //Attack 2
                    lastAttack++;
                    anim.SetTrigger("Attack2");
                }
                else if (lastAttack == 1)
                {
                    //attack 3
                    lastAttack++;
                    anim.SetTrigger("Attack3");
                }
                else
                {
                    //attack 1
                    lastAttack = 0;
                    anim.SetTrigger("Attack1");
                }
            }
        }
    }

    public void Attack()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(attackPos.position, attackSize[lastAttack], 0, enemyLayer);
        if (hit.Length == 0) return;

        for (int i = 0; i < hit.Length; i++)
        {
            AttackKnockback();
            HealthSystem hitHealth = hit[i].GetComponent<HealthSystem>();
            if (hitHealth == null) continue;

            hitHealth.GetHurt(attackDmg, transform.position - hit[i].transform.position);
        }
    }
    public void EndAttack()
    {
        attacking = false;
    }
    void AttackKnockback()
    {
        DisableMovement();
        rb.gravityScale = 0;
        rb.AddForce(transform.right * Dir * -1f * attackKnockbackForce, ForceMode2D.Impulse);
        rb.gravityScale = initialGravityScale;
        EnableMovement(0.2f);
    }

    public void JumpAttack()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(jumpAttackPos.position, jumpAttackSize, 0, jumpAttackHitLayer);
        if (hit.Length == 0) return;

        JumpAttackKnockback();
        for (int i = 0; i < hit.Length; i++)
        {
            HealthSystem hitHealth = hit[i].GetComponent<HealthSystem>();
            if (hitHealth == null) continue;

            hitHealth.GetHurt(attackDmg, transform.position - hit[i].transform.position);
        }
    }

    void JumpAttackKnockback()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(transform.up * jumpAttackKnockbackForce, ForceMode2D.Impulse);
    }

    #endregion

    #region Dash
    void Dash()
    {
        timeSinceLastDash += Time.deltaTime;
        if (timeSinceLastDash >= dashFrecuency) canDash = true;

        if (!canDash || attacking || antman || dashing) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashing = true;

            canDash = false;

            anim.SetTrigger("Dash");

            //Stop and disable movement and gravity
            DisableMovement();
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;

            //actual dash
            rb.AddForce(transform.right * dashForce * Dir, ForceMode2D.Impulse);
        }
    }
    public void EndDash()
    {
        dashing = false;

        timeSinceLastDash = 0;
        rb.gravityScale = initialGravityScale;
        rb.velocity = Vector2.zero;
        EnableMovement();
    }
    #endregion

    #region WallSlide

    void CheckWallSlide()
    {
        if (grounded || antman) { wallSliding = false; return; }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallSlideContact.position, wallSlideCheckRadius, wallLayer);

        if (colliders.Length == 0) wallSliding = false;
        else
        {
            if ((Dir == -1 && input_hor < 0) || (Dir == 1 && input_hor > 0))
            {
                WallSlide();
            }
            else wallSliding = false;
        }
    }

    void WallSlide()
    {
        wallSliding = true;
        canDoubleJump = true;
        if (rb.velocity.y <= -wallSlideSpeed) rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
    }

    #endregion

    void CheckTP()
    {
        if(Input.GetKeyDown(KeyCode.T) && pixie.states == Pixie.States.Checkpoint)
        {
            TpToPixie();
        }
    }
    void TpToPixie()
    {
        //anim.SetTrigger("Disappear"); //animacion de tp
        transform.position = new Vector3(pixie.transform.position.x, pixie.transform.position.y + tpYPos, pixie.transform.position.z);
        AudioManager.Instance.PlaySound("TP");
    }

    void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck_tr.position, groundCheckRadius, groundLayer);

        if (colliders.Length == 0) grounded = false;
        else
        {
            grounded = true;
            canDoubleJump = true;
        }
    }

    void UpdateAnim()
    {
        anim.SetFloat("XVel", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("YVel", rb.velocity.y);
        anim.SetBool("Grounded", grounded);
        anim.SetBool("WallSliding", wallSliding);
        anim.SetBool("Antman", antman);
    }
    
    void AntMan()
    {
        if (wallSliding || dashing || attacking || shielding) return;

        if (!canAntman)
        {
            antmanTimer += Time.deltaTime;
            if (antmanTimer >= antmanCD)
            {
                antmanTimer = 0;
                canAntman = true;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            antman = !antman;
            antmanTimer = 0;

            if (antman)
            {
                anim.SetTrigger("ConvertToSmall");
                canAntman = false;
            }
            else
            {
                anim.SetTrigger("ConvertToBig");
                canAntman = false; 
            }
        }
    }

    void Shield()
    {
        if (!shielding)
        {
            shield.SetActive(false);

            if (!canShield)
            {
                shieldCDTimer += Time.deltaTime;
                if (shieldCDTimer >= shieldCD)
                {
                    canShield = true;
                    shieldCDTimer = 0;
                }
            }
        }

        if (!canShield || wallSliding || dashing || attacking) return;

        if (Input.GetMouseButton(1))
        {
            //Mantain to be active
            shielding = true;
            shield.SetActive(true);

            shieldActiveTimer += Time.deltaTime;

            if (shieldActiveTimer >= shieldActiveTime) EndShield();
        }
        else if (shielding) EndShield();
    }

    void EndShield()
    {
        shielding = false;
        canShield = false;
        shieldActiveTimer = 0;
        shieldCDTimer = 0;
    }

    void PlaySounds()
    {
        if(grounded && rb.velocity.magnitude > .1f)
        {
            AudioManager.Instance.PlaySound("GrassRun");
        }
        else
        {
            AudioManager.Instance.StopSound("GrassRun");
        }
    }

    private void OnDrawGizmos()
    {
        //Draw attack 1
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackSize[0].x, attackSize[0].y, 0));
        //Draw attack 2
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackSize[1].x, attackSize[1].y, 0));
        //Draw attack 3
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackSize[2].x, attackSize[2].y, 0));
        //Draw jump attack
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(jumpAttackPos.position, new Vector3(jumpAttackSize.x, jumpAttackSize.y, 0));
    }
}
