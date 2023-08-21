using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public BulletTypes Type { get; set; }
    public float Angle { get; set; }
    public float Speed { get; set; }
    public float Damage { get; set; }
    public float TotalDamage { get; set; }
    public bool HasGravity { get; set; }
    public Vector3 StartPos { get; set; }
    public int MaxPierceCount { get; set; }
    public int PierceCount { get; set; }
    public float PierceDamageMultiplier { get; set; }
    public float MaxDistance { get; set; }
    public float MinDamageRange { get; set; }
    public float MaxDamageRange { get; set; }
    public float BlastRadius { get; set; }
    public float BlastKnockbackForce { get; set; }
    public float BlastMaxDamageRadius { get; set; }
    public float BlastMinDamageRadius { get; set; }

    protected Vector3 StartDirection { get; set; }
    protected Rigidbody2D Rigidbody { get; set; }
    protected Vector2 LastPosition;
    protected LayerMask TargetLayerMask;

    protected virtual void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        TotalDamage = Damage;
        TargetLayerMask = LayerMask.GetMask("Enemy", "Environment");
    }

    protected virtual void Update()
    {
        var distanceFromStart = Vector3.Distance(transform.position, StartPos);
        if (distanceFromStart >= MaxDistance || distanceFromStart >= 100f)
        {
            OnMaxDistanceReach();
        }

        CheckTunneling();
    }

    protected virtual void FixedUpdate()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTrigger(collision);
    }

    /// <summary>
    /// Lida com colis�es do proj�til.
    /// </summary>
    /// <param name="collision">O collider do objeto com que o proj�til colidiu.</param>
    private void HandleCollision(Collider2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
            OnEnemyHit(collision);
        else if (collision.gameObject.CompareTag("Environment"))
            OnObjectHit(collision);
    }

    /// <summary>
    /// Lida com colis�es do tipo Trigger do do proj�til.
    /// </summary>
    /// <param name="collision">O collider do objeto com que o proj�til colidiu.</param>
    private void HandleTrigger(Collider2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
            OnEnemyHit(collision);
        else if (collision.gameObject.CompareTag("Environment"))
            OnObjectHit(collision);
    }

    /// <summary>
    /// Inicializa o proj�til ap�s a instancia��o.
    /// </summary>
    public virtual void Init()
    {
        float angleInDegrees = Mathf.Repeat(Angle * Mathf.Rad2Deg, 360f);
        StartDirection = (angleInDegrees * Vector3.right).normalized;
    }

    /// <summary>
    /// Fun��o chamada quando o proj�til colide com um objeto do tipo "Environment".
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnObjectHit(Collider2D collision)
    {
        KillSelf();
    }

    /// <summary>
    /// Fun��o chamada quando o proj�til colide com um objeto do tipo "Enemy".
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnEnemyHit(Collider2D collision)
    {
        var target = collision.GetComponentInParent<IPlayerTarget>();
        if (target != null)
        {
            if (!target.IsAlive)
            {
                KillSelf();
                return;
            }
        }

        KillSelf();
    }

    /// <summary>
    /// Fun��o chamada quando o proj�til atinge a dist�ncia m�xima.
    /// </summary>
    protected virtual void OnMaxDistanceReach()
    {
        KillSelf();
    }

    /// <summary>
    /// Fun��o chamada quando a explos�o do proj�til atinge um inimigo.
    /// </summary>
    protected virtual void OnEnemyHitByExplosion()
    {

    }

    /// <summary>
    /// Inicia o movimento do proj�til.
    /// </summary>
    protected void StartForwardMovement()
    {
        Rigidbody.velocity = transform.right * Speed;
    }

    /// <summary>
    /// Destroi o proj�til.
    /// </summary>
    protected virtual void KillSelf()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Verifica se o proj�til est� atravessando um objeto.
    /// </summary>
    protected virtual void CheckTunneling()
    {
        if(LastPosition == Vector2.zero)
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
            Rigidbody.position = hit.point;
            if (hit.collider.isTrigger)
                HandleTrigger(hit.collider);
            else
                HandleCollision(hit.collider);
        }

        LastPosition = Rigidbody.position;
    }
}
