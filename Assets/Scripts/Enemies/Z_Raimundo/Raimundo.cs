using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Raimundo : BaseEnemy
{
    public GameObject SparksPrefab;
    private int HelmetStage = 3;
    private float HelmetHealth = 50f;
    private float HelmetMaxHealth;
    private int HelmetStageCount = 3;
    private Transform Helmet, Head;
    public List<CustomAudio> HitHelmetSounds;
    float lastSparkTime;
    float sparksDelay = 0.03f;
    private Rigidbody2D HelmetRigidBody;
    private SpriteRenderer HelmetSprite;
    private float CurrentHelmetSpriteAlpha = 1f;
    private float BodyDamageMultiplier = 0.2f;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Raimundo;
        MovementSpeed = 0.5f;
        AccelerationSpeed = 1f;
        Health = 70f;
        Damage = 19f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;

        Helmet = transform.Find("Helmet");
        Head = transform.Find("Head");
        HelmetMaxHealth = HelmetHealth;
        HelmetRigidBody = Helmet.GetComponent<Rigidbody2D>();
        HelmetSprite = Helmet.GetComponent<SpriteRenderer>();
        HelmetRigidBody.isKinematic = true;

        base.Start();

        HealthBar.AnimationSpeed = 5f;
    }

    protected override void Update()
    {
        base.Update();
        HelmetStage = Mathf.FloorToInt((HelmetHealth / HelmetMaxHealth) * HelmetStageCount);
        SetHelmetLayer();
    }
    /// <summary>
    /// Função que define a layer da animação do capacete
    /// </summary>
    void SetHelmetLayer()
    {
        switch (HelmetStage)
        {
            case 0:
                if (Helmet != null)
                    BreakHelmet();
                Head.gameObject.SetActive(true);
                break;
            case 1:
                Animator.SetLayerWeight(1, HelmetStage);
                Animator.SetLayerWeight(2, 0);
                Animator.SetLayerWeight(3, 0);
                Head.gameObject.SetActive(false);
                break;
            case 2:
                Animator.SetLayerWeight(1, 0);
                Animator.SetLayerWeight(2, HelmetStage);
                Animator.SetLayerWeight(3, 0);
                Head.gameObject.SetActive(false);
                break;
            case 3:
                Animator.SetLayerWeight(1, 0);
                Animator.SetLayerWeight(2, 0);
                Animator.SetLayerWeight(3, HelmetStage);
                Head.gameObject.SetActive(false);
                break;
        }
    }
    public override void TakeDamage(float value, float headshotMultiplier, string bodyPartName, IEnemyTarget attacker, Vector3? hitPosition = null)
    {
        if (value < 0 || Health <= 0) return;

        Color32 color;

        switch (bodyPartName)
        {
            case "Helmet":
                HelmetHealth = Mathf.Clamp(HelmetHealth - value, 0, HelmetMaxHealth);
                color = Color.white;
                break;
            case "Head":
                value *= headshotMultiplier;
                color = Color.red;
                break;
            default:
                value *= BodyDamageMultiplier;
                color = Color.yellow;
                break;
        }

        ShowPopup(value.ToString("N1"), color, hitPosition ?? transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 2));

        if (bodyPartName != "Helmet")
        {
            if (!AudioSource.isPlaying)
                DamageSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);

            Health = Mathf.Clamp(Health - value, 0, MaxHealth);
            HealthBar.RemoveValue(value);
        }

        if (Health <= 0)
            Die(bodyPartName, attacker);

        if (bodyPartName == "Helmet")
            HitHelmetSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
    }

    public override void OnPointHit(Vector3 hitPoint, Vector3 pointToDirection, string bodyPartName)
    {
        if (SparksPrefab == null)
            return;

        if (lastSparkTime + sparksDelay > Time.time)
            return;

        if (HelmetStage != 0 && bodyPartName == "Helmet")
        {
            var sparks = Instantiate(SparksPrefab, hitPoint, Quaternion.identity, EffectsContainer);
            sparks.transform.up = pointToDirection;
            lastSparkTime = Time.time;
        }

        base.OnPointHit(hitPoint, pointToDirection, bodyPartName);
    }
    /// <summary>
    /// Função que faz o capacete cair
    /// </summary>
    void BreakHelmet()
    {
        HelmetRigidBody.isKinematic = false;
        Helmet.parent = null;
        Helmet.GetComponent<Animator>().enabled = true;
        StartCoroutine(StartHelmetFadeOutCountDown());
    }

    protected override IEnumerator StartDeathFadeOutCountDown()
    {
        if (HealthBar != null)
            Destroy(HealthBar.gameObject);

        if (DeathFadeOutDelayMs > 0)
            yield return new WaitForSeconds(DeathFadeOutDelayMs / 1000f);

        while (CurrentSpriteAlpha > 0)
        {
            CurrentSpriteAlpha -= 10f * Time.deltaTime;

            Color spriteColor = SpriteRenderer.color;
            spriteColor.a = CurrentSpriteAlpha;
            SpriteRenderer.color = spriteColor;
            if (Helmet != null && HelmetStage != 0)
            {
                Color spriteHelmetColor = HelmetSprite.color;
                spriteHelmetColor.a = CurrentSpriteAlpha;
                HelmetSprite.color = spriteHelmetColor;
            }

            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
        if (Helmet != null && HelmetStage != 0)
            Destroy(Helmet.gameObject);
    }
    /// <summary>
    /// Inicia a contagem regressiva para o capacete desaparecer, depois faz com que fique transparente gradativamente, depois destroi o GameObject.
    /// </summary>
    private IEnumerator StartHelmetFadeOutCountDown()
    {
        if (DeathFadeOutDelayMs > 0)
            yield return new WaitForSeconds(DeathFadeOutDelayMs / 1000f);

        while (CurrentHelmetSpriteAlpha > 0)
        {
            CurrentHelmetSpriteAlpha -= 10f * Time.deltaTime;

            Color spriteHelmetColor = HelmetSprite.color;
            spriteHelmetColor.a = CurrentHelmetSpriteAlpha;
            HelmetSprite.color = spriteHelmetColor;

            yield return new WaitForSeconds(0.2f);
        }
        if (Helmet != null)
            Destroy(Helmet.gameObject);
    }

    public override void SetRandomValues(float health, float speed, float damage, int killscore, BaseEnemy enemy, bool isBoss = false)
    {
        base.SetRandomValues(health, speed, damage, killscore, enemy, isBoss);

        HelmetSprite.sortingOrder = SpriteRenderer.sortingOrder + 1;
    }
}
