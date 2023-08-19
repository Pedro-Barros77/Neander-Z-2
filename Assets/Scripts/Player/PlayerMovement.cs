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
    bool isGrounded;
    bool isRolling;
    bool isJumpingSideways;
    bool isTurning;
    bool isTurningBack;
    bool isRunning;
    bool isFalling;
    bool isCrouching;

    bool isIdle => !isRolling && !isJumpingSideways && !isTurning && !isTurningBack && !isRunning && !isFalling && !isCrouching;

    bool isPressingRight;
    bool isPressingLeft;
    bool wasPressingRight;
    bool wasPressingLeft;
    bool isPressingCrouch;
    float movementDir;
    bool isMoving;

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

        if (Input.GetKey(KeyCode.LeftControl) && isGrounded && !isJumpingSideways)
        {
            if (isPressingRight && !isPressingLeft && rollCooledDown)
                Roll(false);

            if (isPressingLeft && !isPressingRight && rollCooledDown)
                Roll(true);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isRolling && !isCrouching)
            Jump();

        isPressingCrouch = Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.S);
        if (isPressingCrouch && isGrounded && !isJumpingSideways && !isRolling)
            Crouch();
        else
            isCrouching = false;

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

            if (!isTurning && !isRunning && !isRolling && !isTurningBack && !isJumpingSideways)
                isFalling = true;

            if (isMoving || isPressingRight || isPressingLeft)
                isRunning = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            isGrounded = false;
            isFalling = false;
        }
    }

    /// <summary>
    /// Processa o input e move o jogador.
    /// </summary>
    private void Movement()
    {
        if ((isPressingLeft && isPressingRight) || isCrouching)
            return;

        if (math.abs(rb.velocity.x) < MovementSpeed)
            rb.velocity += new Vector2(dirInput * AccelerationSpeed, 0);
    }

    /// <summary>
    /// Faz o jogador pular.
    /// </summary>
    private void Jump()
    {
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
        isTurning = false;
        isTurningBack = false;
        float rollDirection = isLeft ? -1 : 1;
        rb.AddForce(new Vector2(RollForce * rollDirection, 10f));
    }

    private void Crouch()
    {
        isRunning = false;
        isTurning = false;
        isTurningBack = false;
        isFalling = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        isCrouching = true;
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
        isTurningBack = false;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame do giro do personagem.
    /// </summary>
    public void OnTurnBackEnd()
    {
        isTurningBack = false;
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
        //Debug.Log($"turn:{isTurning}, turnBack:{isTurningBack}, run:{isRunning}, fall:{isFalling}, jump:{isJumpingSideways}, roll:{isRolling}, crouch:{isCrouching}");
        isPressingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        isPressingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        wasPressingRight = Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D);
        wasPressingLeft = Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A);

        movementDir = rb.velocity.x;
        isMoving = math.abs(movementDir) > 0.1;

        if (isMoving)
        {
            if (movementDir <= 0)
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;
        }

        if ((isPressingRight ^ isPressingLeft) && !isTurning && !isRunning && !isRolling && !isJumpingSideways && !isCrouching)
        {
            isTurning = true;
            isTurningBack = false;
            isFalling = false;
        }

        if (((wasPressingRight || wasPressingLeft) || (isPressingLeft && isPressingRight)) && !isTurningBack && (isTurning || isRunning) && isGrounded && !isJumpingSideways && !isCrouching)
        {
            isTurningBack = true;
            isTurning = false;
            isFalling = false;
        }

        if (isGrounded)
        {
            isJumpingSideways = false;
            if (!isMoving && !isPressingRight && !isPressingLeft)
                isRunning = false;

            if (isRunning && isPressingLeft && isPressingRight)
            {
                isRunning = false;
                isTurningBack = true;
            }
        }
        else
        {
            isRunning = false;
            if (isMoving)
            {
                isJumpingSideways = true;
                isTurning = false;
            }
        }

        SyncAnimationStates();
    }

    private void SyncAnimationStates()
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumpingSideways", isJumpingSideways);
        animator.SetBool("isCrouching", isCrouching);

        if (isFalling) animator.SetTrigger("Fall");
        else animator.ResetTrigger("Fall");

        if (isTurning) animator.SetTrigger("Turn");
        else animator.ResetTrigger("Turn");

        if (isTurningBack) animator.SetTrigger("TurnBack");
        else animator.ResetTrigger("TurnBack");

        if (isRolling) animator.SetTrigger("Roll");
        else animator.ResetTrigger("Roll");
    }
}
