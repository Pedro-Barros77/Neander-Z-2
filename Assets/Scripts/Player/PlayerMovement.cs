using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool IsCrouching { get; private set; }
    public float LastRollTime { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool DoubleJumped { get; private set; }
    public Animator PlayerAnimator => animator;
    public Animator DoubleJumpEffectAnimator;
    float dirInput;
    bool isRolling;
    bool isJumpingSideways;
    bool isTurning;
    bool isTurningBack;
    bool isRunning;
    bool isFalling;
    bool isSprinting;

    bool isIdle => !isRolling && !isJumpingSideways && !isTurning && !isTurningBack && !isRunning && !isFalling && !IsCrouching && !Player.IsDying;

    bool isPressingRight;
    bool isPressingLeft;
    bool wasPressingRight;
    bool wasPressingLeft;
    bool isPressingCrouch;
    float movementDir;
    bool isMoving;

    Player Player;
    Animator animator;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        Player = GetComponentInParent<Player>();
        animator = GetComponentInParent<Animator>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        DoubleJumpEffectAnimator = Player.transform.Find("DoubleJumpSwiftEffect").GetComponent<Animator>();
    }

    void Update()
    {
        if (!Player.IsAlive || Player.IsDying)
        {
            isRolling = false;
            isJumpingSideways = false;
            isTurning = false;
            isTurningBack = false;
            isRunning = false;
            isFalling = false;
            IsCrouching = false;
            isSprinting = false;
            SyncAnimationStates();
            return;
        }

        dirInput = 0;
        if (Constants.GetAction(InputActions.MoveLeft)) dirInput = -1;
        if (Constants.GetAction(InputActions.MoveRight)) dirInput = 1;

        var rollCooledDown = LastRollTime + (Player.RollCooldownMs / 1000) <= Time.time;

        if (Constants.GetAction(InputActions.TacticalAbility))
        {
            switch (Player.Backpack.EquippedTacticalAbilityType)
            {
                case TacticalAbilityTypes.TacticalRoll:
                    if (IsGrounded && !isJumpingSideways && rollCooledDown)
                    {
                        if (isPressingRight && !isPressingLeft)
                            Roll(false);

                        if (isPressingLeft && !isPressingRight)
                            Roll(true);
                    }
                    break;

                case TacticalAbilityTypes.DoubleJump:
                    if (!DoubleJumped && !IsGrounded)
                        Jump(isDoubleJump: true);
                    break;
            }
        }

        if (Constants.GetActionDown(InputActions.Jump) && !isRolling && !IsCrouching)
        {
            if (IsGrounded)
                Jump();
            else if (Player.Backpack.EquippedTacticalAbilityType == TacticalAbilityTypes.DoubleJump && !DoubleJumped)
                Jump(isDoubleJump: true);
        }

        isPressingCrouch = Constants.GetAction(InputActions.Crouch);
        if (isPressingCrouch && IsGrounded && !isJumpingSideways && !isRolling)
            Crouch();
        else
            IsCrouching = false;

        if (Constants.GetAction(InputActions.Sprint) && !IsCrouching && !isJumpingSideways && !isRolling && Player.Stamina >= Player.SprintStaminaDrain)
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
            IsGrounded = true;
            DoubleJumped = false;

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
            IsGrounded = false;
            isFalling = false;
        }
    }

    /// <summary>
    /// Processa o input e move o jogador.
    /// </summary>
    private void Movement()
    {
        if ((isPressingLeft && isPressingRight) || IsCrouching)
            return;

        if (Mathf.Abs(Player.RigidBody.velocity.x) < Player.MovementSpeed && !isSprinting)
            Player.RigidBody.velocity += new Vector2(dirInput * Player.AccelerationSpeed, 0);

        else if (Mathf.Abs(Player.RigidBody.velocity.x) < Player.MovementSpeed * Player.SprintSpeedMultiplier && isSprinting)
        {
            Player.RigidBody.velocity += new Vector2(dirInput * (Player.AccelerationSpeed * Player.SprintSpeedMultiplier), 0);
            if (dirInput != 0)
                Player.LoseStamina(Player.SprintStaminaDrain);
        }
    }

    /// <summary>
    /// Faz o jogador pular.
    /// </summary>
    private void Jump(bool isDoubleJump = false)
    {
        if (Player.Stamina < Player.JumpStaminaDrain)
            return;

        if (isDoubleJump)
        {
            DoubleJumped = true;
            Player.RigidBody.velocity = new Vector2(Player.RigidBody.velocity.x, 0f);
            DoubleJumpEffectAnimator.Play("WindSwift");
        }

        IsGrounded = false;
        Player.LoseStamina(Player.JumpStaminaDrain);
        Player.RigidBody.AddForce(new Vector2(0f, Player.JumpForce));
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
        Player.RigidBody.AddForce(new Vector2(Player.RollForce * rollDirection, 10f));
        Player.LoseStamina(Player.RollStaminaDrain);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyAttack"), LayerMask.NameToLayer("Player"), true);
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
        Player.RigidBody.velocity = new Vector2(0, Player.RigidBody.velocity.y);
        IsCrouching = true;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame da Rolada Tática.
    /// </summary>
    public void OnRollEnd()
    {
        isRolling = false;
        isRunning = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyAttack"), LayerMask.NameToLayer("Player"), false);
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
        isPressingRight = Constants.GetAction(InputActions.MoveRight);
        isPressingLeft = Constants.GetAction(InputActions.MoveLeft);
        wasPressingRight = Constants.GetActionUp(InputActions.MoveRight);
        wasPressingLeft = Constants.GetActionUp(InputActions.MoveLeft);

        movementDir = Player.RigidBody.velocity.x;
        isMoving = Mathf.Abs(movementDir) > 0.1;

        Player.CurrentWeapon.PlayerFlipDir = transform.parent.localScale.x;

        if (IsCrouching)
        {
            if (Player.WeaponController.IsAimingLeft)
                FlipPlayer(true);
            else
                FlipPlayer(false);
        }
        else
        {
            if (movementDir < 0 || isPressingLeft)
                FlipPlayer(true);
            else if (movementDir > 0 || isPressingRight)
                FlipPlayer(false);
        }

        if ((isPressingRight ^ isPressingLeft) && !isTurning && !isRunning && !isRolling && !isJumpingSideways && !IsCrouching)
        {
            isTurning = true;
            isTurningBack = false;
            isFalling = false;
        }

        if (((wasPressingRight || wasPressingLeft) || (isPressingLeft && isPressingRight)) && !isTurningBack && (isTurning || isRunning) && IsGrounded && !isJumpingSideways && !IsCrouching)
        {
            isTurningBack = true;
            isTurning = false;
            isFalling = false;
        }

        if (IsGrounded)
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
            transform.parent.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            Player.CurrentWeapon.PlayerFlipDir = -1;
            transform.parent.localScale = new Vector3(-1, 1, 1);
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
        animator.SetBool("isCrouching", IsCrouching);

        if (Player.IsDying)
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
