using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTest : MonoBehaviour
{
    bool jumping, canDoubleJump, jumpKeyHeld;
    [SerializeField] float jumpSpeed, jumpCounterSpeed, jumpTime;
    float jumpTimer;
    Rigidbody2D rb;

    [Header("GROUND CHECK")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck_tr;
    [SerializeField] bool grounded;
    [SerializeField] float groundCheckRadius;

    [SerializeField] float movSpeed;
    float input_hor;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();

        input_hor = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Jump();
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            StopJump();
        }
    }

    private void FixedUpdate()
    {
        Movement();

        if (jumpKeyHeld)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

            jumpTimer += Time.deltaTime;
            if (jumpTimer > jumpTime)
            {
                JumpFinished();
            }
        }
    }

    void Movement()
    {
        rb.velocity = new Vector2(input_hor * movSpeed, rb.velocity.y);
    }

    void Jump()
    {
        if (!canDoubleJump || jumping) return;

        if (!grounded) canDoubleJump = false;
        jumpKeyHeld = true;
        jumping = true;
    }

    void StopJump()
    {
        if (!jumpKeyHeld) return;

        rb.velocity = new Vector2(rb.velocity.x, -jumpCounterSpeed);
        //rb.velocity = new Vector2(rb.velocity.x, 0);
        jumpTimer = 0;
        jumpKeyHeld = false;
        jumping = false;
    }
    
    void JumpFinished()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .75f);
        jumpTimer = 0;
        jumpKeyHeld = false;
        jumping = false;
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
}
