using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Raven : BaseEnemy
{
    private float Altitude = 8.57f;
    private float minAltitude = 6f;
    private float maxAltitude = 9f;
    private IEnemyTarget Target;
    private float AttackChance = 0.50f;
    private float AttackChanceDelayMs = 3000f;
    private bool isMovingLeft = true;
    private bool isDiving;
    private bool isRising;
    private bool isHovering = true;
    private float DiveSpeedMultiplier = 8f;
    private float maxDistanceHover = 5f;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Raven;
        MovementSpeed = 2f;
        AccelerationSpeed = 1f;
        Health = 1f;
        Damage = 5f;
        HeadshotDamageMultiplier = 2f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;
        Altitude = Random.Range(minAltitude, maxAltitude);
        transform.position = new Vector3(transform.position.x, Altitude, 0);
        base.Start();

        HealthBar.AnimationSpeed = 5f;
        StartCoroutine(AttackTimer());
    }
    protected override void Update()
    {
        if (Target == null)
            UpdateDirection();

        if (isDiving && !isAttacking && !isRising)
            Dive();

        if (isRising)
            Rise();

        base.Update();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Environment"))
            isMovingLeft = !isMovingLeft;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Environment"))
            isMovingLeft = !isMovingLeft;
    }

    protected override void Movement(IEnemyTarget target)
    {
        if (Target == null)
            return;

        if (!IsAlive || isDying)
            return;

        if (transform.position.x < LevelXLimit.x)
            isMovingLeft = false;
        else if (transform.position.x > LevelXLimit.y)
            isMovingLeft = true;

        float distanceX = Vector3.Distance(new Vector3(transform.position.x, 0), new Vector3(Target.transform.position.x, 0));
        if (distanceX > maxDistanceHover)
            UpdateDirection();

        if (Mathf.Abs(RigidBody.velocity.x) < MovementSpeed && !IsInAttackRange && !isAttacking && !isDiving)
            RigidBody.velocity += new Vector2(isMovingLeft ? -1 : 1 * AccelerationSpeed, 0);

        if (Mathf.Abs(RigidBody.velocity.y) < MovementSpeed && isRising)
            RigidBody.velocity += new Vector2(0, Mathf.Clamp(AccelerationSpeed, 0, MovementSpeed));

        float targetHeight = Target.SpriteRenderer.bounds.size.y;
        if (isDiving && transform.position.y >= Target.transform.position.y + (targetHeight / 2))
        {
            Vector3 direction = Target.transform.position - transform.position;
            RigidBody.AddForce(direction * MovementSpeed * DiveSpeedMultiplier, ForceMode2D.Force);
        }

        if (!IsInAttackRange && transform.position.y < Target.transform.position.y + (targetHeight / 3f))
            Rise();

        if (transform.position.y > Altitude)
            transform.position = new Vector3(transform.position.x, Altitude);
    }
    /// <summary>
    /// Função responsável por atualizar a direção do inimigo.
    /// </summary>
    void UpdateDirection()
    {
        Target = GetClosestTarget();
        isMovingLeft = Target.transform.position.x < transform.position.x;
    }
    /// <summary>
    /// Loop responsável por verificar se pode rodar o dado.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackTimer()
    {
        while (IsAlive)
        {
            if ((isAttacking || isDiving || isRising) && !isHovering)
                yield return null;

            yield return new WaitForSeconds(AttackChanceDelayMs / 1000f);
            RollAttackDice();
        }
    }
    /// <summary>
    /// Função responsável por rolar o dado para decidir se o inimigo vai atacar.
    /// </summary>
    void RollAttackDice()
    {
        if ((isAttacking || isDiving || isRising) && !isHovering)
            return;

        var dice = Random.Range(0f, 1f);
        bool attack = dice < AttackChance;
        if (attack)
        {
            Dive();
        }
    }
    /// <summary>
    /// Função responsável por fazer o inimigo mergulhar.
    /// </summary>
    void Dive()
    {
        if (Target == null)
            return;

        isDiving = true;
        isRising = false;
        isHovering = false;
        isMovingLeft = Target.transform.position.x < transform.position.x;
    }

    protected override void OnAttackEnd()
    {
        Rise();
        base.OnAttackEnd();
    }
    protected override void StartAttack(IEnemyTarget target)
    {
        if (isRising)
            return;

        base.StartAttack(target);
    }
    /// <summary>
    /// Função responsável por fazer o inimigo subir.
    /// </summary>
    void Rise()
    {
        if (Target == null)
            return;

        isRising = true;
        isDiving = false;
        isHovering = false;

        if (transform.position.y >= Altitude)
        {
            isRising = false;
            isHovering = true;
        }
    }

    protected override void Die(string lastDamagedBodyPartName)
    {
        RigidBody.gravityScale = 4;
        RigidBody.freezeRotation = false;
        RigidBody.AddTorque(Random.Range(-2f, 2f) * 15);
        base.Die(lastDamagedBodyPartName);
    }
}
