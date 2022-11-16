using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sr;
    Rigidbody2D rb;

    //Movement variables
    [Header("Movement variables")]
    bool movementDisabled;
    [SerializeField] float speed = 4f;
    float initialGravityScale, input_hor;
    int dir;

    //Jump Variables
    [Header("Jump variables")]
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float counterJumpForce = 2f;
    bool isJumping, canDoubleJump, jumpKeyHeld;
    float jumpForce;

    //Attack variables
    [Header("Attack variables")]
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
    [Header("Dash variables")]
    [SerializeField] float dashForce = 1500f;
    [SerializeField] float dashFrecuency = 1f;
    bool canDash, dashing;
    float timeSinceLastDash;

    //WallSlide variables
    [Header("WallSlide variables")]
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform wallSlideContact;
    bool wallSliding;
    [SerializeField] float wallSlideSpeed;
    [SerializeField] float wallSlideCheckRadius;

    [Header("Ground Check Variables")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck_tr;
    [SerializeField] bool grounded;
    [SerializeField] float groundCheckRadius;

    [Header("Pixie")]
    [SerializeField] Pixie pixie;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        jumpForce = CalculateJumpForce(Physics2D.gravity.magnitude, jumpHeight);

        dir = 1;

        movementDisabled = false;

        isJumping = false;
        canDoubleJump = true;
        jumpKeyHeld = false;

        lastAttack = -1;
        canAttack = true;

        initialGravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        CheckJump();

        CheckAttack();

        Dash();

        CheckTP();

        UpdateAnim();
    }

    private void FixedUpdate()
    {
        if (isJumping)
        {
            if (!jumpKeyHeld || rb.velocity.y < 0)
            {
                rb.AddForce(Vector2.down * counterJumpForce, ForceMode2D.Force);
            }
        }

        CheckGround();

        CheckWallSlide();

        Movement();
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

    public void UnControl()
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

        rb.velocity = new Vector2(input_hor * speed, rb.velocity.y);

        FlipSr();
    }
    void FlipSr()
    {
        if (input_hor != 0)
        {
            if (input_hor < 0) { dir = -1; sr.transform.localScale = new Vector3(-1, 1, 0); }
            else { dir = 1; sr.transform.localScale = new Vector3(1, 1, 0); }
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
        Invoke("EnableMovement", seconds);
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
            else if (canDoubleJump) DoubleJump();
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
            if (!canAttack || dashing || wallSliding) return;

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
        rb.AddForce(transform.right * dir * -1f * attackKnockbackForce, ForceMode2D.Impulse);
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

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!canDash || attacking) return;

            dashing = true;

            canDash = false;
            timeSinceLastDash = 0;

            anim.SetTrigger("Dash");

            //Stop and disable movement and gravity
            DisableMovement();
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;

            //actual dash
            rb.AddForce(transform.right * dashForce * dir, ForceMode2D.Impulse);
        }
    }
    public void EndDash()
    {
        dashing = false;

        rb.gravityScale = initialGravityScale;
        rb.velocity = Vector2.zero;
        EnableMovement();
    }
    #endregion

    #region WallSlide

    void CheckWallSlide()
    {
        if (grounded) { wallSliding = false; return; }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallSlideContact.position, wallSlideCheckRadius, wallLayer);

        if (colliders.Length == 0) wallSliding = false;
        else
        {
            if ((dir == -1 && input_hor < 0) || (dir == 1 && input_hor > 0))
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
        transform.position = pixie.transform.position;
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
