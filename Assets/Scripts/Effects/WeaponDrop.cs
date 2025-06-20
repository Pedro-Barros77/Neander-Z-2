using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : MonoBehaviour
{
    public List<CustomAudio> GroundHitSounds;
    public Vector2 MinMaxRandomRotationForce = new(-7, 7);
    public Vector2 MinMaxRandomXForce = new(100, 200);
    public Vector2 MinMaxRandomYForce = new(100, 200);
    public float GroundHitSoundsGap = 0.15f;
    public bool RotateTowardsDirection;

    [HideInInspector]
    public Vector3 TargetScale = new(1, 1, 1);
    [HideInInspector]
    public float SpawnDirection;

    AudioSource audioSource;
    Rigidbody2D Rigidbody;
    float lastGroundHitSoundTime, settleTime;
    bool hasTouchedGround;

    const float SETTLE_DELAY = 3;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Rigidbody = GetComponent<Rigidbody2D>();

        ApplyForces();
        Transform bulletsContainer = GameObject.Find("ProjectilesContainer").transform;
        transform.SetParent(bulletsContainer);
        transform.localScale = TargetScale;
    }

    private void Update()
    {
        if (settleTime == 0)
        {
            if (Rigidbody != null && Rigidbody.velocity.magnitude < 0.01f)
                settleTime = Time.time;
        }
        else
        {
            if (Time.time > settleTime + SETTLE_DELAY)
                Settle();
        }

        if (RotateTowardsDirection && !hasTouchedGround)
        {
            Vector2 direction = Rigidbody.velocity.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x).RadToDeg();
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Environment"))
            return;

        hasTouchedGround = true;
        HandleImpactSound();
    }

    private void HandleImpactSound()
    {
        if (Time.time < lastGroundHitSoundTime + GroundHitSoundsGap)
            return;

        GroundHitSounds.PlayRandomIfAny(audioSource, AudioTypes.Player);

        lastGroundHitSoundTime = Time.time;
    }

    private void ApplyForces()
    {
        if (Rigidbody == null)
            return;

        if (MinMaxRandomXForce.magnitude > 0 || MinMaxRandomYForce.magnitude > 0)
        {
            float randomXForce = Random.Range(MinMaxRandomXForce.x, MinMaxRandomXForce.y);
            float randomYForce = Random.Range(SpawnDirection * MinMaxRandomYForce.x, SpawnDirection * MinMaxRandomYForce.y);
            Rigidbody.AddRelativeForce(new Vector2(randomXForce, randomYForce));
        }

        if (!RotateTowardsDirection && MinMaxRandomRotationForce.magnitude > 0)
        {
            float randomRotationForce = Random.Range(MinMaxRandomRotationForce.x, MinMaxRandomRotationForce.y);
            Rigidbody.AddTorque(randomRotationForce);
        }
    }

    private void Settle()
    {
        if (Rigidbody != null)
            Destroy(Rigidbody);

        if (audioSource != null)
            Destroy(audioSource);

        var collider = GetComponent<Collider2D>();
        if (collider != null)
            Destroy(collider);

        Destroy(this); //Removendo esse script, não o gameobject
    }
}
