using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sr;
    Rigidbody2D rb;

    [Header("Player attributes")]
    [SerializeField]
    float speed = 4f, jumpHeight = 5f, counterJumpForce = 2f, dashForce = 10f;

    bool movementDisabled;
    float initialGravityScale;
    int dir;

    [Header("Ground Check Variables")]
    [SerializeField] bool grounded;
    [SerializeField] float groundCheckRadius;
    [SerializeField] Transform groundCheck_tr;
    [SerializeField] LayerMask groundLayer;

    //Jump Variables
    bool isJumping, canDoubleJump, jumpKeyHeld, canJump;
    float jumpForce;

    //Attack variables
    bool canAttack, attacking;
    float timeSinceLastAttack, attackFrecuency, timeToCombo;
    int lastAttack;

    //Dash variables
    bool canDash, dashing;
    float timeSinceLastDash, dashFrecuency;

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
        canJump = true;

        lastAttack = -1;
        attackFrecuency = 0.5f;
        timeToCombo = 1f;
        canAttack = true;

        dashFrecuency = 1;

        initialGravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        CheckJump();

        Attack();

        Dash();

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

        Movement();
    }

    #region Movement / Jump / Attack / Dash
    void Movement()
    {
        if (movementDisabled) return;

        float x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(x * speed, rb.velocity.y);

        FlipSr(x);
    }
    void FlipSr(float input_hor)
    {
        if (input_hor != 0)
        {
            if (input_hor < 0) { dir = -1; sr.transform.localScale = new Vector3(-1, 1, 0); }
            else { dir = 1; sr.transform.localScale = new Vector3(1, 1, 0); }
        }
    }

    void DisableMovement()
    {
        movementDisabled = true;
    }

    void EnableMovement()
    {
        movementDisabled = false;
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyHeld = true;
            if (grounded) Jump();
            else if (canDoubleJump) DoubleJump();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) jumpKeyHeld = false;
    }

    void Jump()
    {
        if (dashing || attacking) return;
        
        isJumping = true;
        anim.SetTrigger("Jump");
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    private void DoubleJump()
    {
        if (dashing || attacking) return;

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

    void Attack()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack >= attackFrecuency) canAttack = true;
        if (timeSinceLastAttack >= timeToCombo) lastAttack = -1;

        if (Input.GetMouseButtonDown(0))
        {
            if (!canAttack || dashing) return;

            attacking = true;

            timeSinceLastAttack = 0;
            canAttack = false;
            if (lastAttack == 1)
            {
                //Attack 2
                anim.SetTrigger("Attack2");
            }
            else if (lastAttack == 2)
            {
                //attack 3
                anim.SetTrigger("Attack3");
            }
            else
            {
                //attack 1
                lastAttack = 0;
                anim.SetTrigger("Attack1");
            }
            lastAttack++;
        }
    }

    public void EndAttack()
    {
        attacking = false;
    }

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

   
    #endregion

    void UpdateAnim()
    {
        anim.SetFloat("XVel", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("YVel", rb.velocity.y);
        anim.SetBool("Grounded", grounded);
    }
}
