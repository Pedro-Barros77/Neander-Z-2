using UnityEngine;

public class MagnetEffect : BaseAppliedEffect
{
    public Transform OriginPoint { get; set; }
    public Vector2 MagnetStrength { get; set; } = new(100f, 100f);
    public float MaxDistance { get; set; } = 3f;

    private Rigidbody2D TargetRB;

    protected override void Start()
    {
        base.Start();

        TargetRB = EnemyTarget?.RigidBody ?? PlayerTarget?.gameObject.GetComponent<Rigidbody2D>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (transform.parent == null)
            return;

        if (StartTime == 0)
            return;

        if(TargetRB == null)
            return;

        Debug.Log("Attracting");

        Vector2 direction = (OriginPoint.position - TargetRB.transform.position).normalized;
        float distance = Vector2.Distance(TargetRB.transform.position, OriginPoint.position);

        if (distance < MaxDistance)
        {
            direction.Normalize();

            float forceFactor = 1 - (distance / MaxDistance);

            Vector2 force = new Vector2(
                direction.x * MagnetStrength.x * forceFactor, // Force on the X axis
                direction.y * MagnetStrength.y * forceFactor  // Force on the Y axis
            );

            TargetRB.AddForce(force);
        }
    }
}
