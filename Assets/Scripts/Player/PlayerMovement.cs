using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float AccelerationSpeed { get; set; } = 1f;
    public float MovementSpeed { get; set; } = 5f;
    public float JumpForce { get; set; } = 1800f;
    public float RollForce { get; set; } = 1800f;
    public float RollDistance { get; set; } = 5f;
    public float RollCooldownMs { get; set; } = 2000f;

    float LastRollTime;
    float dirInput;
    bool isGrounded = false;
    bool isRolling = false;
    bool isJumping = false;
    bool isTurning = false;
    bool isRunning = false;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        dirInput = Input.GetAxisRaw("Horizontal");
        var rollCooledDown = LastRollTime + (RollCooldownMs / 1000) <= Time.time;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isRolling)
            Jump();

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && rollCooledDown)
                Roll(false);

            if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && rollCooledDown)
                Roll(true);
        }
        Animation();
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
            isGrounded = false;
    }

    private void Movement()
    {
        if (math.abs(rb.velocity.x) < MovementSpeed)
            rb.velocity += new Vector2(dirInput * AccelerationSpeed, 0);
    }

    private void Jump()
    {
        rb.AddForce(new Vector2(0f, JumpForce));
    }

    private void Roll(bool isLeft)
    {
        LastRollTime = Time.time;
        isRolling = true;
        animator.SetTrigger("Roll");
        float rollDirection = isLeft ? -1 : 1;
        rb.AddForce(new Vector2(RollForce * rollDirection, 10f));
    }

    public void OnRollEnd()
    {
        isRolling = false;
        isRunning = true;
    }
    public void OnTurnEnd()
    {
        isTurning = false;
        isRunning = true;
    }

    private void Animation()
    {
        bool isPressingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool isPressingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool isPressingJump = Input.GetKey(KeyCode.Space);

        var dir = rb.velocity.x;
        if (dir <= 0)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;

        if ((isPressingRight || isPressingLeft) && !isTurning && !isRunning && !isRolling)
        {
            isTurning = true;
            animator.SetTrigger("Turn");
        }

        if (isRunning && !isTurning)
            animator.SetBool("isRunning", true);

        if (math.abs(dir) <= 0.1)
        {
            isRunning = false;
            animator.SetBool("isRunning", false);
        }

        animator.SetBool("isIdle", !isRolling && !isJumping && !isTurning && !isRunning);
    }
}
