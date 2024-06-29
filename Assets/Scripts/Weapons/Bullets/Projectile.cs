using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public BulletTypes Type { get; set; }
    public float AngleDegrees { get; set; }
    public float Speed { get; set; }
    public float GravityScale { get; set; }
    public bool FlyInfinitely { get; set; } = true;
    public float Damage { get; set; }
    public float MinDamage { get; set; }
    public float TotalDamage { get; set; }
    public float HeadshotMultiplier { get; set; }
    public Vector3 StartPos { get; set; }
    public int MaxPierceCount { get; set; }
    public int PierceCount { get; set; }
    public float PierceDamageMultiplier { get; set; }
    public float MaxDistance { get; set; }
    public float MinDamageRange { get; set; }
    public float MaxDamageRange { get; set; }
    public bool RotateTowardsDirection { get; set; }
    public bool IsTargetHit { get; set; }
    public float ShotTime { get; set; }
    public delegate void OnBulletKillDelegate(Projectile projectile, IPlayerTarget playerTarget, IEnemyTarget enemyTarget);
    public event OnBulletKillDelegate OnBulletKill;

    /// <summary>
    /// O dono do projétil, se for um inimigo.
    /// </summary>
    public IPlayerTarget EnemyOwner { get; set; }
    /// <summary>
    /// O dono do projétil, se for um player.
    /// </summary>
    public IEnemyTarget PlayerOwner { get; set; }

    /// <summary>
    /// O último inimigo atingido, se for um projétil de um player.
    /// </summary>
    public IPlayerTarget LastEnemyHit { get; set; }
    /// <summary>
    /// Último player atingido, se for um projétil de um inimigo.
    /// </summary>
    public IEnemyTarget LastPlayerHit { get; set; }

    protected Rigidbody2D Rigidbody { get; set; }
    protected Collider2D Collider { get; set; }
    protected SpriteRenderer Sprite { get; set; }
    protected Vector2 LastPosition;
    protected LayerMask TargetLayerMask;
    protected List<int> PiercedTargetsIds = new();

    protected virtual void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        TotalDamage = Damage;
        TargetLayerMask = LayerMask.GetMask("Enemy", "Environment", "PlayerEnvironment");
        Rigidbody.gravityScale = GravityScale;
    }

    protected virtual void Update()
    {
        CheckRange();

        if (PierceCount > 0)
            Damage *= Mathf.Pow(PierceDamageMultiplier, PierceCount);

        if (RotateTowardsDirection)
        {
            Vector2 direction = Rigidbody.velocity.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x).RadToDeg();
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        CheckTunneling();
    }

    protected virtual void FixedUpdate()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Collider.isTrigger)
            HandleCollision(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Collider.isTrigger)
            HandleCollision(collision);
    }

    /// <summary>
    /// Lida com colisões do projétil.
    /// </summary>
    /// <param name="collision">O collider do objeto com que o projétil colidiu.</param>
    private void HandleCollision(Collider2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            var target = collision.GetComponentInParent<IPlayerTarget>();
            if (target == null)
                return;

            int targetId = target.transform.GetInstanceID();

            if (!PiercedTargetsIds.Contains(targetId))
            {
                PiercedTargetsIds.Add(targetId);
                OnEnemyHit(collision);
            }
        }
        else if (collision.gameObject.CompareTag("Environment"))
            OnObjectHit(collision);
    }

    /// <summary>
    /// Inicializa o projétil após a instanciação.
    /// </summary>
    public virtual void Init()
    {
    }

    /// <summary>
    /// Função chamada quando o projétil colide com um objeto do tipo "Environment".
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnObjectHit(Collider2D collision)
    {
        KillSelf();
    }

    /// <summary>
    /// Função chamada quando o projétil colide com um objeto do tipo "Enemy".
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnEnemyHit(Collider2D collision)
    {
        var target = collision.GetComponentInParent<IPlayerTarget>();
        if (target != null)
        {
            if (target.IsAlive)
            {
                IsTargetHit = true;
                LastEnemyHit = target;
            }
            if (PierceCount < MaxPierceCount)
                PierceCount++;
            else if ((MaxPierceCount == 0 || PierceCount >= MaxPierceCount) && target.IsAlive)
            {
                KillSelf();
            }
        }
    }

    /// <summary>
    /// Função chamada quando o projétil atinge a distância máxima.
    /// </summary>
    protected virtual void OnMaxDistanceReach()
    {
        KillSelf();
    }

    /// <summary>
    /// Inicia o movimento do projétil.
    /// </summary>
    protected void StartForwardMovement()
    {
        Rigidbody.velocity = transform.right * Speed;
    }

    /// <summary>
    /// Destroi o projétil.
    /// </summary>
    protected virtual void KillSelf()
    {
        OnBulletKill(this, LastEnemyHit, LastPlayerHit);

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Destroi o projétil após um delay.
    /// </summary>
    /// <param name="delaySeconds">Delay em segundos para esperar antes de destruir o projétil.</param>
    /// <returns></returns>
    protected IEnumerator KillSelfDelayed(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        KillSelf();
    }

    /// <summary>
    /// Verifica o alcance do projétil, aplica o dano proporcional e destrói no alcance máximo.
    /// </summary>
    protected virtual void CheckRange()
    {
        var distanceFromStart = Vector3.Distance(transform.position, StartPos);
        if (distanceFromStart >= MaxDistance || distanceFromStart >= 100f)
        {
            OnMaxDistanceReach();
            return;
        }

        if (MaxDamageRange <= 0 && MinDamageRange <= 0)
        {
            Damage = TotalDamage;
            return;
        }

        if (distanceFromStart < MaxDamageRange)
        {
            Damage = TotalDamage;
            return;
        }

        var clampedDistance = Mathf.Clamp(distanceFromStart, MaxDamageRange, MinDamageRange);
        var percentage = (clampedDistance - MaxDamageRange) / (MinDamageRange - MaxDamageRange);

        Damage = Mathf.Lerp(TotalDamage, MinDamage, percentage);
    }

    /// <summary>
    /// Verifica se o projétil está atravessando um objeto.
    /// </summary>
    protected virtual void CheckTunneling()
    {
        if (LastPosition == Vector2.zero)
        {
            LastPosition = Rigidbody.position;
            return;
        }
        Vector2 currentPosition = Rigidbody.position;
        Vector2 direction = currentPosition - LastPosition;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(LastPosition, direction, distance, TargetLayerMask);

        if (hit.collider != null)
        {
            var target = hit.transform.GetComponentInParent<IPlayerTarget>();

            if (target != null)
            {
                int targetId = target.transform.GetInstanceID();

                if (!PiercedTargetsIds.Contains(targetId) && target.IsAlive)
                    Rigidbody.position = hit.point;

                HandleCollision(hit.collider);
            }
        }
        LastPosition = Rigidbody.position;
    }
}
