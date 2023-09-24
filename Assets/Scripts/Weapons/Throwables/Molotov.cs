using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : BaseThrowable
{
    [SerializeField]
    public GameObject FireFlamesPrefab;

    private GameObject FlamesInstance;
    private List<IPlayerTarget> PlayerTargets = new();
    private List<IEnemyTarget> EnemyTargets = new();

    protected override void Awake()
    {
        Type = ThrowableTypes.Molotov;

        base.Awake();
    }

    protected override void Update()
    {
        if (MenuController.Instance.IsGamePaused)
            return;

        base.Update();

        if (HasDetonated)
            CheckTargetsOnFire();
    }

    protected override void Detonate()
    {
        HasDetonated = true;

        SpawnFlames();

        DetonateSounds.PlayRandomIfAny(AudioSource);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        var ememyTarget = collision.GetComponentInParent<IEnemyTarget>();
        var playerTarget = collision.GetComponentInParent<IPlayerTarget>();

        if (ememyTarget != null)
        {
            if (!EnemyTargets.Contains(ememyTarget))
                EnemyTargets.Add(ememyTarget);
        }

        if (playerTarget != null)
        {
            if (!PlayerTargets.Contains(playerTarget))
                PlayerTargets.Add(playerTarget);

            if (playerTarget.IsAlive)
                IsTargetHit = true;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        var ememyTarget = collision.GetComponentInParent<IEnemyTarget>();
        var playerTarget = collision.GetComponentInParent<IPlayerTarget>();

        if (ememyTarget != null)
        {
            if (EnemyTargets.Contains(ememyTarget))
                EnemyTargets.Remove(ememyTarget);
        }

        if (playerTarget != null)
        {
            if (PlayerTargets.Contains(playerTarget))
                PlayerTargets.Remove(playerTarget);
        }
    }

    /// <summary>
    /// Cria um tapete de fogo no chão, no local de impacto.
    /// </summary>
    private void SpawnFlames()
    {
        Rigidbody.rotation = 0;
        Rigidbody.freezeRotation = true;
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.gravityScale = 0;
        transform.rotation = Quaternion.identity;
        Sprite.enabled = false;
        Collider.enabled = false;

        var fireSprite = FireFlamesPrefab.GetComponent<SpriteRenderer>();
        var hitPosition = transform.position + new Vector3(0, fireSprite.size.y / 2);
        FlamesInstance = Instantiate(FireFlamesPrefab, hitPosition, Quaternion.identity, transform);
        transform.localScale = Vector3.one;

        StartCoroutine(FireTimeOut());
    }

    /// <summary>
    /// Inicia a contagem regressiva para o fim do tapete de fogo.
    /// </summary>
    /// <returns></returns>
    IEnumerator FireTimeOut()
    {
        yield return new WaitForSeconds(EffectDurationMs / 1000);

        var fireAnimator = FlamesInstance.GetComponent<Animator>();
        fireAnimator.SetTrigger("End");

        while (!fireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Finished"))
        {
            yield return null;
        }

        Destroy(FlamesInstance);
        KillSelf();
    }

    /// <summary>
    /// Cria o efeito de queimadura no alvo.
    /// </summary>
    /// <param name="parent">O pai do efeito (alvo).</param>
    /// <returns>O objeto criado.</returns>
    private GameObject CreateBurningEffect(Transform parent)
    {
        var burnEffectObj = new GameObject("BurningEffect");
        var burningEffect = burnEffectObj.AddComponent<BurningEffect>();
        burningEffect.TickDamage = Damage;
        burnEffectObj.transform.SetParent(parent);

        return burnEffectObj;
    }

    /// <summary>
    /// Verifica se os alvos estão dentro do tapete de fogo e aplica/reseta o efeito de queimadura.
    /// </summary>
    private void CheckTargetsOnFire()
    {
        foreach (IPlayerTarget playerTarget in PlayerTargets)
        {
            var burnFX = playerTarget.transform.GetComponentInChildren<BurningEffect>();
            if (burnFX == null)
            {
                var burnEffectObj = CreateBurningEffect(playerTarget.transform);
                burnFX = burnEffectObj.GetComponent<BurningEffect>();
            }

            if (PlayerOwner != null)
                burnFX.PlayerOwner = PlayerOwner;
            else if (EnemyOwner != null)
                burnFX.EnemyOwner = EnemyOwner;

            burnFX.SetEffect(EffectDecoupledDurationMs, EffectTickIntervalMs);
        }

        foreach (IEnemyTarget enemyTarget in EnemyTargets)
        {
            var burnFX = enemyTarget.transform.GetComponentInChildren<BurningEffect>();
            if (burnFX == null)
            {
                var burnEffectObj = CreateBurningEffect(enemyTarget.transform);
                burnFX = burnEffectObj.GetComponent<BurningEffect>();
            }

            if (PlayerOwner != null)
                burnFX.PlayerOwner = PlayerOwner;
            else if (EnemyOwner != null)
                burnFX.EnemyOwner = EnemyOwner;

            burnFX.SetEffect(EffectDecoupledDurationMs, EffectTickIntervalMs);
        }
    }
}
