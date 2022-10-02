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
    bool movementDisabled = false, isJumping = false, canDoubleJump = true, jumpKeyHeld;

    [Header("Ground Check Variables")]
    [SerializeField] bool grounded;
    [SerializeField] float groundCheckRadius;
    [SerializeField] Transform groundCheck_tr;
    [SerializeField] LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        jumpForce = CalculateJumpForce(Physics2D.gravity.magnitude, jumpHeight);
    }

    // Update is called once per frame
    void Update()
    {
        FlipSr();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyHeld = true;
            if (grounded) Jump();
            else if (canDoubleJump) DoubleJump();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) jumpKeyHeld = false;
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

    #region Movement
    void Movement()
    {
        if (movementDisabled) return;

        float x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(x * speed, rb.velocity.y);
    }

    void Jump()
    {
        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    private void DoubleJump()
    {
        canDoubleJump = false;
        isJumping = true;

        //Stop velocity in y axis so it doesn't affect the new jump
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    public static float CalculateJumpForce(float gravityStrength, float jumpHeight)
    {
        return Mathf.Sqrt(2 * gravityStrength * jumpHeight * 1000f);
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
}
