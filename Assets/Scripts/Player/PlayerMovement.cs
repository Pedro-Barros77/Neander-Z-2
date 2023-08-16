using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// A velocidade de aceleração do jogador.
    /// </summary>
    public float AccelerationSpeed => Player.AccelerationSpeed;
    /// <summary>
    /// A velocidade de movimento máxima do jogador.
    /// </summary>
    public float MovementSpeed => Player.MovementSpeed;
    /// <summary>
    /// A força do pulo do jogador.
    /// </summary>
    public float JumpForce => Player.JumpForce;
    /// <summary>
    /// A força de rolagem da habilidade Rolada Tática.
    /// </summary>
    public float RollForce => Player.RollForce;
    /// <summary>
    /// O tempo de recarga da habilidade Rolada Tática.
    /// </summary>
    public float RollCooldownMs => Player.RollCooldownMs;

    float LastRollTime;
    float dirInput;
    bool isGrounded = false;
    bool isRolling = false;
    bool isJumping = false;
    bool isTurning = false;
    bool isRunning = false;
    bool isFalling = false;

    Player Player;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        Player = GetComponent<Player>();
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

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isJumping)
        {
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && rollCooledDown)
                Roll(false);

            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && rollCooledDown)
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
        {
            isGrounded = true;
            isJumping = false;

            if (!isTurning && !isRunning && !isRolling && !isJumping)
            {
                isFalling = true;
                animator.SetTrigger("fall");
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            isGrounded = false;
            isJumping = false;
            isFalling = false;
        }

    }

    /// <summary>
    /// Processa o input e move o jogador.
    /// </summary>
    private void Movement()
    {
        if (math.abs(rb.velocity.x) < MovementSpeed)
            rb.velocity += new Vector2(dirInput * AccelerationSpeed, 0);
    }

    /// <summary>
    /// Faz o jogador pular.
    /// </summary>
    private void Jump()
    {
        isJumping = true;
        rb.AddForce(new Vector2(0f, JumpForce));
    }

    /// <summary>
    /// Realiza a Rolada Tática na direção especificada.
    /// </summary>
    /// <param name="isLeft">Se a direção deve ser para a esquerda, caso contrário, será para a direita.</param>
    private void Roll(bool isLeft)
    {
        LastRollTime = Time.time;
        isRolling = true;
        animator.SetTrigger("Roll");
        float rollDirection = isLeft ? -1 : 1;
        rb.AddForce(new Vector2(RollForce * rollDirection, 10f));
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame da Rolada Tática.
    /// </summary>
    public void OnRollEnd()
    {
        isRolling = false;
        isRunning = true;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame do giro do personagem.
    /// </summary>
    public void OnTurnEnd()
    {
        isTurning = false;
        isRunning = true;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame ao cair no chão do personagem.
    /// </summary>
    public void OnFallGroundEnd()
    {
        isFalling = false;
    }

    /// <summary>
    /// Processa a l�gica de anima��o do jogador.
    /// </summary>
    private void Animation()
    {
        bool isPressingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool isPressingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);

        var dir = rb.velocity.x;
        if (math.abs(dir) > 0.1)
        {
            if (dir <= 0)
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;
        }

        if ((isPressingRight || isPressingLeft) && !isTurning && !isRunning && !isRolling)
        {
            isTurning = true;
            animator.SetTrigger("Turn");
        }

        if (math.abs(dir) <= 0.1 && !isPressingRight && !isPressingLeft)
        {
            isRunning = false;
            animator.SetBool("isRunning", false);
        }

        if (!isGrounded)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", true);
        }

        if (isGrounded)
        {
            animator.SetBool("isJumping", false);

            if ((dir != 0 || isPressingLeft || isPressingRight) && isRunning && !isJumping && !isTurning && !isRolling)
            {
                animator.SetBool("isRunning", true);
            }
            else
                animator.SetBool("isRunning", false);
        }
        animator.SetBool("isIdle", !isRolling && !isJumping && !isTurning && !isRunning && !isFalling);
    }
}
