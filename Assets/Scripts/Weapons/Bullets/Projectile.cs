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


    protected virtual void OnObjectHit(Collider2D collision)
    {
        KillSelf();
    }

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

    protected virtual void OnMaxDistanceReach()
    {
        KillSelf();
    }

    protected virtual void OnEnemyHitByExplosion()
    {

    }

    public virtual void Init()
    {
        float angleInDegrees = Mathf.Repeat(Angle * Mathf.Rad2Deg, 360f);
        StartDirection = (angleInDegrees * Vector3.right).normalized;
    }

    protected virtual void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        TotalDamage = Damage;
    }

    protected virtual void Update()
    {
        var distance = Vector3.Distance(transform.position, StartPos);
        if (distance >= MaxDistance || distance >= 100f)
        {
            OnMaxDistanceReach();
        }
    }

    protected virtual void FixedUpdate()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
            OnEnemyHit(collision.collider);
        else if (collision.gameObject.CompareTag("Environment"))
            OnObjectHit(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
            OnEnemyHit(collision);
        else if (collision.gameObject.CompareTag("Environment"))
            OnObjectHit(collision);
    }

    protected void MoveForward()
    {
        Rigidbody.velocity = transform.right * Speed;
        //transform.Translate(Speed * Time.fixedDeltaTime * StartDirection);
    }
     protected virtual void KillSelf()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
