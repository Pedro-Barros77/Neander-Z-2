using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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
    bool isSprinting;

    bool isIdle => !isRolling && !isJumpingSideways && !isTurning && !isTurningBack && !isRunning && !isFalling && !isCrouching && !Player.isDying;

    bool isPressingRight;
    bool isPressingLeft;
    bool wasPressingRight;
    bool wasPressingLeft;
    bool isPressingCrouch;
    float movementDir;
    bool isMoving;

    Player Player;
    Rigidbody2D rigidBody;
    Animator animator;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        Player = GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!Player.IsAlive || Player.isDying)
        {
            isRolling = false;
            isJumpingSideways = false;
            isTurning = false;
            isTurningBack = false;
            isRunning = false;
            isFalling = false;
            isCrouching = false;
            isSprinting = false;
            SyncAnimationStates();
            return;
        }

        dirInput = Input.GetAxisRaw("Horizontal");
        var rollCooledDown = LastRollTime + (Player.RollCooldownMs / 1000) <= Time.time;

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

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && !isJumpingSideways && !isRolling && Player.Stamina >= Player.SprintStaminaDrain)
        {
            isSprinting = true;
        }
        else
            isSprinting = false;

        Animation();
    }

    void FixedUpdate()
    {
        if (!Player.IsAlive)
            return;

        Movement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Player.IsAlive)
            return;

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
        if (!Player.IsAlive)
            return;

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

        if (Mathf.Abs(rigidBody.velocity.x) < Player.MovementSpeed && !isSprinting)
            rigidBody.velocity += new Vector2(dirInput * Player.AccelerationSpeed, 0);

        else if (Mathf.Abs(rigidBody.velocity.x) < Player.MovementSpeed * Player.SprintSpeedMultiplier && isSprinting)
        {
            rigidBody.velocity += new Vector2(dirInput * (Player.AccelerationSpeed * Player.SprintSpeedMultiplier), 0);
            if (dirInput != 0)
                Player.LoseStamina(Player.SprintStaminaDrain);
        }
    }

    /// <summary>
    /// Faz o jogador pular.
    /// </summary>
    private void Jump()
    {
        if (Player.Stamina < Player.JumpStaminaDrain)
            return;
        Player.LoseStamina(Player.JumpStaminaDrain);
        rigidBody.AddForce(new Vector2(0f, Player.JumpForce));
    }

    /// <summary>
    /// Realiza a Rolada Tática na direção especificada.
    /// </summary>
    /// <param name="isLeft">Se a direção deve ser para a esquerda, caso contrário, será para a direita.</param>
    private void Roll(bool isLeft)
    {
        if (Player.Stamina < Player.RollStaminaDrain)
            return;

        LastRollTime = Time.time;
        isRolling = true;
        isTurning = false;
        isTurningBack = false;
        float rollDirection = isLeft ? -1 : 1;
        rigidBody.AddForce(new Vector2(Player.RollForce * rollDirection, 10f));
        Player.LoseStamina(Player.RollStaminaDrain);
    }

    /// <summary>
    /// Faz o jogador parar e agachar.
    /// </summary>
    private void Crouch()
    {
        isRunning = false;
        isTurning = false;
        isTurningBack = false;
        isFalling = false;
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
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

        movementDir = rigidBody.velocity.x;
        isMoving = Mathf.Abs(movementDir) > 0.1;

        if (isCrouching)
        {
            if (Player.WeaponController.IsAimingLeft)
                FlipPlayer(true);
            else
                FlipPlayer(false);
        }
        else
        {
            if (movementDir <= 0)
                FlipPlayer(true);
            else
                FlipPlayer(false);
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

    private void FlipPlayer(bool isLeft)
    {
        if (isLeft)
        {
            Player.CurrentWeapon.PlayerFlipDir = 1;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            Player.CurrentWeapon.PlayerFlipDir = -1;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }


    /// <summary>
    /// Sinconiza os estados da animação do Animator com as variáveis de controle.
    /// </summary>
    private void SyncAnimationStates()
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumpingSideways", isJumpingSideways);
        animator.SetBool("isCrouching", isCrouching);

        if (Player.isDying)
            animator.SetTrigger("Die");
        else
            animator.ResetTrigger("Die");

        if (isSprinting)
            animator.SetFloat("SprintSpeedMultiplier", Player.SprintSpeedMultiplier);
        else
            animator.SetFloat("SprintSpeedMultiplier", 1f);

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
