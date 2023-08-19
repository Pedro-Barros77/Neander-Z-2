using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    /// <summary>
    /// A vida máxima do inimigo.
    /// </summary>
    public float MaxHealth { get; protected set; }
    /// <summary>
    /// A vida atual do inimigo.
    /// </summary>
    public float Health { get; protected set; } = 100f;
    /// <summary>
    /// O dano causado pelo inimigo.
    /// </summary>
    public float Damage { get; protected set; }
    /// <summary>
    /// A velocidade de movimento atual do inimigo.
    /// </summary>
    public float MovementSpeed { get; protected set; } = 1f;
    /// <summary>
    /// A velocidade de movimento máxima do inimigo.
    /// </summary>
    public float MaxMovementSpeed { get; protected set; }
    /// <summary>
    /// A velocidade de aceleração do inimigo.
    /// </summary>
    public float AccelerationSpeed { get; protected set; } = 1f;
    /// <summary>
    /// O multiplicador de dano recebido por um tiro na cabeça desse inimigo.
    /// </summary>
    public float HeadshotDamageMultiplier { protected get; set; }
    /// <summary>
    /// A pontuação recebida por matar esse inimigo.
    /// </summary>
    public int KillScore { get; protected set; }
    /// <summary>
    /// O multiplicador de pontos recebidos por um tiro na cabeça desse inimigo.
    /// </summary>
    public float HeadshotScoreMultiplier { get; protected set; }
    /// <summary>
    /// O tipo de inimigo.
    /// </summary>
    public EnemyTypes Type { get; protected set; }

    /// <summary>
    /// Infica se esse inimigo está na distância de ataque de um alvo.
    /// </summary>
    protected bool IsInAttackRange { get; set; }
    /// <summary>
    /// Hora da morte desse inimigo.
    /// </summary>
    protected Time? DeathTime { get; set; }
    /// <summary>
    /// O tempo que leva para esse inimigo desaparecer após ser morto.
    /// </summary>
    protected float DeathFadeOutDurationMs { get; set; } = 1000f;
    /// <summary>
    /// A opacidade atual do inimigo.
    /// </summary>
    protected float CurrentSpriteAlpha { get; set; } = 255;
    /// <summary>
    /// Lista de alvos disponíveis para esse inimigo perseguir e atacar.
    /// </summary>
    protected List<IEnemyTarget> Targets => WavesManager.Instance.EnemiesTargets;
    /// <summary>
    /// Referência ao componente Rigidbody2D desse inimigo.
    /// </summary>
    protected Rigidbody2D RigidBody;
    /// <summary>
    /// Referência ao componente SpriteRenderer desse inimigo.
    /// </summary>
    protected SpriteRenderer SpriteRenderer;


    protected virtual void Start()
    {
        MaxMovementSpeed = MovementSpeed;
        MaxHealth = Health;
        RigidBody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        var closestTarget = GetClosestTarget();
        if (IsInAttackRange)
            Attack(closestTarget);

        Animation();
    }

    protected virtual void FixedUpdate()
    {
        var closestTarget = GetClosestTarget();
        Movement(closestTarget);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsInAttackRange = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsInAttackRange = false;
        }
    }

    protected float DistanceFrom(Transform target) => Mathf.Abs(target.position.x - transform.position.x);
    protected float DistanceFrom(IEnemyTarget target) => DistanceFrom(target.transform);
    protected virtual IEnemyTarget GetClosestTarget() => Targets.FirstOrDefault(a => DistanceFrom(a) == Targets.Min(DistanceFrom));

    protected virtual void Movement(IEnemyTarget target)
    {
        var targetDir = target.transform.position.x < transform.position.x ? -1 : 1;
        var targetDistance = DistanceFrom(target);

        if (Mathf.Abs(RigidBody.velocity.x) < MovementSpeed && !IsInAttackRange)
            RigidBody.velocity += new Vector2(targetDir * AccelerationSpeed, 0);
    }

    protected virtual void Attack(IEnemyTarget target)
    {
        var targetDir = target.transform.position.x < transform.position.x ? -1 : 1;
        var targetDistance = DistanceFrom(target);
    }

    /// <summary>
    /// Processa a lógica de animação do jogador.
    /// </summary>
    protected void Animation()
    {
        float movementDir = RigidBody.velocity.x;
        bool isMoving = Mathf.Abs(movementDir) > 0.1;

        if (isMoving)
        {
            if (movementDir <= 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
