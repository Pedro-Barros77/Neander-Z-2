using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float movementSpeed = 5f, jumpForce = 10f;

    bool isGrounded = false;

    Rigidbody2D rb;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isGrounded);
        var dir = rb.velocity.x;
        if (dir > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (dir < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        Movement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            isGrounded = false;
        }
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        rb.velocity = new(x * movementSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpForce));
    }
}
