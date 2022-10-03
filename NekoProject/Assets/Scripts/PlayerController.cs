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
    float speed = 4f, jumpHeight = 5f, counterJumpForce = 2f;

    float jumpForce;
    bool movementDisabled, isJumping, canDoubleJump, jumpKeyHeld;

    [Header("Ground Check Variables")]
    [SerializeField] bool grounded;
    [SerializeField] float groundCheckRadius;
    [SerializeField] Transform groundCheck_tr;
    [SerializeField] LayerMask groundLayer;

    //Attack variables
    bool canAttack;
    float timeSinceLastAttack, attackFrecuency, timeToCombo;
    int lastAttack;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        jumpForce = CalculateJumpForce(Physics2D.gravity.magnitude, jumpHeight);

        movementDisabled = false;
        isJumping = false;
        canDoubleJump = true;
        jumpKeyHeld = false;

        lastAttack = -1;
        attackFrecuency = 0.5f;
        timeToCombo = 1f;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckJump();

        Attack();

        Dash();

        UpdateAnim();

        FlipSr();
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

    void Attack()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack >= attackFrecuency) canAttack = true;
        if (timeSinceLastAttack >= timeToCombo) lastAttack = -1;

        if (Input.GetMouseButtonDown(0))
        {
            if (!canAttack) return;

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

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) anim.SetTrigger("Dash");
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

    void FlipSr()
    {
        if (rb.velocity.x < 0 && !sr.flipX) sr.flipX = true;
        else if (rb.velocity.x > 0 && sr.flipX) sr.flipX = false;
    }
    #endregion

    void UpdateAnim()
    {
        anim.SetFloat("XVel", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("YVel", rb.velocity.y);
        anim.SetBool("Grounded", grounded);
    }
}
