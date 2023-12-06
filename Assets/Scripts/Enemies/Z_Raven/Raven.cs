using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raven : BaseEnemy, IKnockBackable
{
    public float AttackChance { get; set; } = 0.50f;
    public float AttackAttemptDelayMs { get; set; } = 3000f;
    private float Altitude = 8.57f;
    private float minAltitude = 6f;
    private float maxAltitude = 9f;
    private IEnemyTarget Target;
    private bool isMovingLeft = true;
    private bool isDiving;
    private bool isRising;
    private bool isHovering = true;
    private float DiveSpeedMultiplier = 8f;
    private float maxDistanceHover = 5f;
    [SerializeField]
    protected List<CustomAudio> DiveSounds;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Raven;
        MovementSpeed = 2f;
        AccelerationSpeed = 1f;
        Health = 1f;
        Damage = 5f;
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

        float targetHeight = Target.Bounds.size.y;
        if (isDiving && transform.position.y >= Target.transform.position.y + (targetHeight / 2))
        {
            Vector3 direction = Target.transform.position - transform.position;
            RigidBody.AddForce(direction * MovementSpeed * DiveSpeedMultiplier, ForceMode2D.Force);
        }

        if (!IsInAttackRange && transform.position.y < Target.transform.position.y + (targetHeight / 2.5f))
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
        if (Target != null)
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

            yield return new WaitForSeconds(AttackAttemptDelayMs / 1000f);
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
            DiveSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
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

    public override void Die(string lastDamagedBodyPartName, IEnemyTarget attacker)
    {
        if (isDying || !IsAlive)
            return;

        if (RigidBody != null)
        {
            RigidBody.gravityScale = 4;
            RigidBody.freezeRotation = false;
            RigidBody.AddTorque(Random.Range(-2f, 2f) * 15);
        }
        base.Die(lastDamagedBodyPartName, attacker);
    }
}
