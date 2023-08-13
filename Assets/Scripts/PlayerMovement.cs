using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed { get; set; } = 5f;
    public float JumpForce { get; set; } = 800f;

    bool isGrounded = false;

    Rigidbody2D rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        Animation();
    }

    // FixedUpdate is called once per physics frame
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
        float x = Input.GetAxis("Horizontal");
        rb.velocity = new(x * MovementSpeed, rb.velocity.y);

    }

    private void Animation()
    {
        bool isPressingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool isPressingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool isPressingJump = Input.GetKey(KeyCode.Space);

        var dir = rb.velocity.x;
        if (dir <= 0)
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        else
            gameObject.transform.localScale = new Vector3(-1, 1, 1);

        if (isPressingRight || isPressingLeft)
        {
            animator.SetBool("isTurning", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isTurning", false);
            animator.SetBool("isIdle", true);
        }

    }

    private void Jump()
    {
        rb.AddForce(new Vector2(0f, JumpForce));
    }
}
