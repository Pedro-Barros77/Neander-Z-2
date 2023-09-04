using System.Collections;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public BulletTypes Type { get; set; }
    public float AngleDegrees { get; set; }
    public float Speed { get; set; }
    public float Damage { get; set; }
    public float TotalDamage { get; set; }
    public Vector3 StartPos { get; set; }
    public int MaxPierceCount { get; set; }
    public int PierceCount { get; set; }
    public float PierceDamageMultiplier { get; set; }
    public float MaxDistance { get; set; }
    public float MinDamageRange { get; set; }
    public float MaxDamageRange { get; set; }
    public bool RotateTowardsDirection { get; set; }

    protected Rigidbody2D Rigidbody { get; set; }
    protected Collider2D Collider { get; set; }
    protected SpriteRenderer Sprite { get; set; }
    protected Vector2 LastPosition;
    protected LayerMask TargetLayerMask;

    protected virtual void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        TotalDamage = Damage;
        TargetLayerMask = LayerMask.GetMask("Enemy", "Environment", "PlayerEnvironment");
    }

    protected virtual void Update()
    {

        var distanceFromStart = Vector3.Distance(transform.position, StartPos);
        if (distanceFromStart >= MaxDistance || distanceFromStart >= 100f)
        {
            OnMaxDistanceReach();
        }

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
        if (target != null && target.IsAlive)
        {
            if (PierceCount < MaxPierceCount)
            {
                PierceCount++;
                Damage *= PierceDamageMultiplier;
            }
            else if (MaxPierceCount == 0 || PierceCount >= MaxPierceCount)
            {
                KillSelf();
            }
        }
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
    /// Destroi o proj�til ap�s um delay.
    /// </summary>
    /// <param name="delaySeconds">Delay em segundos para esperar antes de destruir o proj�til.</param>
    /// <returns></returns>
    protected IEnumerator KillSelfDelayed(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        KillSelf();
    }

    /// <summary>
    /// Verifica se o proj�til est� atravessando um objeto.
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
            if (target != null && target.IsAlive)
                Rigidbody.position = hit.point;

            if (hit.collider.isTrigger)
                HandleTrigger(hit.collider);
            else
                HandleCollision(hit.collider);
        }

        LastPosition = Rigidbody.position;
    }
}
