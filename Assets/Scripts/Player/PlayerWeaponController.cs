using System;
using System.Linq;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    /// <summary>
    /// O ângulo de mira do jogador, em Graus (Direita: 0, Cima: 90, Esquerda: 180, Baixo: 270).
    /// </summary>
    public float AimAngleDegrees { get; set; }
    /// <summary>
    /// Se a arma está sendo trocada atualmente.
    /// </summary>
    public bool IsSwitchingWeapon { get; set; }
    /// <summary>
    /// Se o jogador está arremessando um item.
    /// </summary>
    public bool IsThrowingItem { get; set; }
    /// <summary>
    /// O jogador pai deste controlador.
    /// </summary>
    public Player Player { get; set; }
    /// <summary>
    /// A posição relativa local inicial do container de mira.
    /// </summary>
    public Vector3 StartLocalPosition { get; private set; }
    /// <summary>
    /// Se o jogador está mirando para a esquerda.
    /// </summary>
    public bool IsAimingLeft => AimAngleDegrees > 90 && AimAngleDegrees < 270;

    [SerializeField]
    public BlinkingText blinkingReloadText;
    [SerializeField]
    int ThrowTrajectorySteps;
    [SerializeField]
    float ThrowTrajectoryStepDistance;

    Transform handTransform, throwingContainerTransform, throwableSpawnPointTransform;
    float? startSwitchTime;
    float startThrowingContainerScale;
    SpriteRenderer playerSprite, handPalmSprite, fingersSprite;
    protected LineRenderer LineRenderer;
    Animator playerAnimator;
    Transform floor;
    Vector2 lastThrowTrajectoryForce, lastThrowStartPoint;
    bool itemThrown;
    Gradient ThrowableLineRendererColor;

    readonly FireModes[] HoldTriggerFireModes = { FireModes.FullAuto, FireModes.Melee };

    private void Awake()
    {
        handTransform = transform.Find("Hand");
        throwingContainerTransform = handTransform.Find("ThrowingContainer");
        var palm = throwingContainerTransform.Find("Palm");
        var fingers = throwingContainerTransform.Find("Fingers");
        throwableSpawnPointTransform = palm.Find("ThrowableSpawnPoint");
        handPalmSprite = palm.GetComponent<SpriteRenderer>();
        fingersSprite = fingers.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        floor = GameObject.Find("Floor")?.transform;
        Player = transform.parent.GetComponent<Player>();
        playerSprite = Player.GetComponent<SpriteRenderer>();
        playerAnimator = Player.GetComponent<Animator>();
        LineRenderer = GetComponent<LineRenderer>();
        StartLocalPosition = transform.localPosition;
        startThrowingContainerScale = throwingContainerTransform.localScale.x;

        ThrowableLineRendererColor = LineRenderer.colorGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
            alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(1, 0.2f), new GradientAlphaKey(1, 0.6f), new GradientAlphaKey(0, 1) }
        };
    }

    void Update()
    {
        if (!Player.IsAlive)
            return;

        if (MenuController.Instance.IsGamePaused)
            return;

        bool isFiring;

        if (HoldTriggerFireModes.Contains(Player.CurrentWeapon.FireMode))
            isFiring = Constants.GetAction(InputActions.Shoot);
        else
            isFiring = Constants.GetActionDown(InputActions.Shoot);

        if (isFiring)
        {
            bool isFiringBurst = Player.CurrentWeapon is BurstFireWeapon && (Player.CurrentWeapon as BurstFireWeapon).IsFiringBurst;
            if (!isFiringBurst)
                Player.CurrentWeapon.Shoot();
        }

        if (Constants.GetActionDown(InputActions.Reload))
            Player.CurrentWeapon.Reload();

        if (Constants.GetActionDown(InputActions.ThrowGrenade))
            StartThrowingItem();

        if (Constants.GetAction(InputActions.ThrowGrenade))
            AimThrowable();

        if (Constants.GetActionUp(InputActions.ThrowGrenade))
            ThrowItem();

        if (IsThrowingItem)
            RenderThrowTrajectory();

        if (Constants.GetActionDown(InputActions.SwitchWeapon))
            SwitchWeapon();

        if (Constants.GetActionDown(InputActions.EquipPrimaryWeapon))
            SwitchWeapon(0);

        if (Constants.GetActionDown(InputActions.EquipSecondaryWeapon))
            SwitchWeapon(1);

        if (IsSwitchingWeapon)
            WeaponSwitchAnimation();
        else
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z));
            RotateTo(worldMousePos);
        }

        blinkingReloadText.gameObject.SetActive(Player.CurrentWeapon.NeedsReload());
        blinkingReloadText.transform.position = Player.transform.position + new Vector3(0, playerSprite.size.y * 0.7f);

        throwingContainerTransform.localScale = new Vector3(Player.CurrentWeapon.PlayerFlipDir * startThrowingContainerScale, startThrowingContainerScale, startThrowingContainerScale);

        handPalmSprite.flipY = IsAimingLeft;
        fingersSprite.flipY = IsAimingLeft;
    }

    /// <summary>
    /// Inicia o arremeço de item.
    /// </summary>
    private void StartThrowingItem()
    {
        if (IsThrowingItem || IsSwitchingWeapon || Player.Backpack.EquippedThrowableType == ThrowableTypes.None)
            return;

        if (Player.Backpack.EquippedThrowable.Count <= 0)
            return;

        IsThrowingItem = true;
        LineRenderer.enabled = true;

        playerAnimator.SetTrigger("Throw");
        playerAnimator.SetFloat("ThrowSpeedMultiplier", 0);
        Player.CurrentWeapon.IsActive = false;

        Player.Backpack.EquippedThrowable.Count--;

        var throwable = InstantiateThrowablePrefab(Player.Backpack.EquippedThrowableType);
        var rb = throwable.GetComponent<Rigidbody2D>();
        var collider = throwable.GetComponent<Collider2D>();
        rb.isKinematic = true;
        collider.enabled = false;
        Player.Backpack.ThrowingThrowable = throwable.GetComponent<BaseThrowable>();
    }

    /// <summary>
    /// Atualiza a trajetória de arremesso do item.
    /// </summary>
    private void AimThrowable()
    {
        if (itemThrown || !IsThrowingItem)
            return;

        lastThrowTrajectoryForce = Player.Backpack.ThrowingThrowable.transform.right.normalized * Player.Backpack.ThrowingThrowable.ThrowForce;
        lastThrowStartPoint = throwableSpawnPointTransform.position;
        LineRenderer.colorGradient = ThrowableLineRendererColor;

        if (Player.Backpack.ThrowingThrowable.StartFuseOnCook)
        {
            float timeLeft = (Player.Backpack.ThrowingThrowable.FuseTimeoutMs / 1000) - (Time.time - Player.Backpack.ThrowingThrowable.CookStartTime);
            float percentage = 1 - (timeLeft / (Player.Backpack.ThrowingThrowable.FuseTimeoutMs / 1000));
            Color color = Color.Lerp(Color.white, Color.red, percentage);
            LineRenderer.colorGradient = new Gradient()
            {
                colorKeys = new GradientColorKey[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
                alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(1, 0.2f), new GradientAlphaKey(1, 0.6f), new GradientAlphaKey(0, 1) }
            };

            if (percentage > 0.5)
            {
                float alpha = Mathf.PingPong(30 * percentage * (percentage / 1.5f), 1.0f);
                LineRenderer.enabled = alpha > 0.5f;
            }
            else
                LineRenderer.enabled = true;
        }
        else
        {
            LineRenderer.enabled = true;
            LineRenderer.colorGradient = ThrowableLineRendererColor;
        }
    }

    /// <summary>
    /// Arremessa o item.
    /// </summary>
    private void ThrowItem()
    {
        if (itemThrown || !IsThrowingItem)
            return;

        itemThrown = true;

        playerAnimator.SetFloat("ThrowSpeedMultiplier", 1);
        var throwableObj = throwableSpawnPointTransform.GetChild(0);
        var rb = throwableObj.GetComponent<Rigidbody2D>();
        var collider = throwableObj.GetComponent<Collider2D>();
        var throwable = throwableObj.GetComponent<BaseThrowable>();
        rb.isKinematic = false;
        collider.enabled = true;
        throwable.Throw();
    }

    /// <summary>
    /// Função chamada pelo Animator no último frame da animação de arremesso.
    /// </summary>
    public void OnThrowEnd()
    {
        playerAnimator.ResetTrigger("Throw");
        playerAnimator.Play($"{Player.Character}_Idle", 1);
        playerAnimator.SetFloat("ThrowSpeedMultiplier", 1);
        Player.CurrentWeapon.IsActive = true;
        IsThrowingItem = false;
        Player.Backpack.ThrowingThrowable = null;
        LineRenderer.enabled = false;
        itemThrown = false;
    }

    /// <summary>
    /// Renderiza a trajetória de arremesso do item.
    /// </summary>
    private void RenderThrowTrajectory()
    {
        var throwable = Player.Backpack.ThrowingThrowable;
        if (throwable == null)
        {
            OnThrowEnd();
            return;
        }

        Vector2[] trajectory = PlotTrajectory(lastThrowStartPoint, lastThrowTrajectoryForce, throwable.Rigidbody.gravityScale, throwable.Rigidbody.drag, ThrowTrajectorySteps, ThrowTrajectoryStepDistance);
        LineRenderer.positionCount = trajectory.Length;
        LineRenderer.SetPositions(trajectory.Select(x => (Vector3)x).ToArray());
        LineRenderer.sortingOrder = 11;
    }

    /// <summary>
    /// Cria os pontos da trajetória de arremesso do item.
    /// </summary>
    /// <param name="pos">O ponto de partida da trajetória.</param>
    /// <param name="velocity">A velocidade/força e direção da trajetória.</param>
    /// <param name="steps">O número de pontos a ser calculados.</param>
    /// <param name="stepDistance">A distância entre cada ponto.</param>
    /// <returns>Uma lista contento as posições de cada ponto da trajetória.</returns>
    public Vector2[] PlotTrajectory(Vector2 pos, Vector2 velocity, float gravityScale, float drag, int steps, float stepDistance)
    {
        Vector2[] results = new Vector2[steps];

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations * stepDistance;
        Vector2 gravityAccel = gravityScale * timestep * timestep * Physics2D.gravity;

        float _drag = 1f - timestep * drag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; ++i)
        {
            moveStep += gravityAccel;
            moveStep *= _drag;
            pos += moveStep;
            if (floor != null && pos.y < floor.position.y)
                break;
            results[i] = pos;
        }

        return results.Where(pos => pos.x != 0 && pos.y != 0).ToArray();
    }

    /// <summary>
    /// Alternar entre as armas primária e secundária equipadas.
    /// </summary>
    /// <param name="index">O índice da arma a ser equipada. 0 = primária, 1 = secundária. Null = inverter.</param>
    public void SwitchWeapon(int? index = null)
    {
        if (IsSwitchingWeapon || IsThrowingItem)
            return;

        if ((index == null || index != Player.Backpack.CurrentWeaponIndex) && Player.Backpack.HasPrimaryEquipped && Player.Backpack.HasSecondaryEquipped)
        {
            bool canSwitch = Player.CurrentWeapon.BeforeSwitchWeapon();
            if (!canSwitch)
                return;

            IsSwitchingWeapon = true;

            Player.Backpack.SwitchWeapon(index);

            Player.CurrentWeapon.AfterSwitchWeaponBack();
        }

        bool equippedPrimary = Player.Backpack.CurrentWeaponIndex == 0;

        if (Player.Backpack.HasPrimaryEquipped)
            Player.Backpack.EquippedPrimaryWeapon.IsActive = equippedPrimary;
        if (Player.Backpack.HasSecondaryEquipped)
            Player.Backpack.EquippedSecondaryWeapon.IsActive = !equippedPrimary;
    }

    /// <summary>
    /// Executa a rotação da mira ao trocar de arma.
    /// </summary>
    private void WeaponSwitchAnimation()
    {
        if (startSwitchTime == null)
            startSwitchTime = Time.time;

        var currentWeaponSwitchTimeMs = Player.CurrentWeapon.SwitchTimeMs;

        if (Time.time - startSwitchTime < currentWeaponSwitchTimeMs / 1000)
        {
            float t = (Time.time - startSwitchTime.Value) / (currentWeaponSwitchTimeMs / 1000);
            float animAngle;
            if (IsAimingLeft)
                animAngle = Mathf.Lerp(91, 180, t);
            else
                animAngle = Mathf.Lerp(89, 0, t);

            var rotation = Quaternion.AngleAxis(animAngle, Vector3.forward);
            var newPosition = transform.position + rotation * Vector3.right;
            RotateTo(newPosition);
        }
        else
        {
            startSwitchTime = null;
            IsSwitchingWeapon = false;
            Player.Backpack.EquippedPrimaryWeapon.IsSwitchingWeapon = false;
            Player.Backpack.EquippedSecondaryWeapon.IsSwitchingWeapon = false;
        }
    }


    public void SetWeaponOffset(Vector3 localPositionOffset)
    {
        transform.localPosition = StartLocalPosition + localPositionOffset;
    }

    /// <summary>
    /// Carrega o Prefab da arma do tipo especificado e o instancia na m�o do jogador.
    /// </summary>
    /// <param name="throwableType">O tipo de arma para instanciar.</param>
    /// <returns>O GameObject instanciado.</returns>
    public GameObject InstantiateThrowablePrefab(ThrowableTypes throwableType)
    {
        var throwablePrefab = Resources.Load<GameObject>($"Prefabs/Weapons/Throwables/{throwableType}");
        GameObject throwableObj = Instantiate(throwablePrefab, throwableSpawnPointTransform);
        throwableObj.name = throwableType.ToString();
        var throwable = throwableObj.GetComponent<BaseThrowable>();
        throwable.PlayerWeaponController = this;
        throwable.PlayerOwner = Player;
        return throwableObj;
    }

    /// <summary>
    /// Carrega o Prefab da arma do tipo especificado e o instancia na m�o do jogador.
    /// </summary>
    /// <param name="weaponType">O tipo de arma para instanciar.</param>
    /// <returns>O GameObject instanciado.</returns>
    public GameObject InstantiateWeaponPrefab(WeaponTypes weaponType)
    {
        var weaponPrefab = Resources.Load<GameObject>($"Prefabs/Weapons/{weaponType}");
        GameObject weaponObj = Instantiate(weaponPrefab, handTransform);
        weaponObj.GetComponent<BaseWeapon>().PlayerWeaponController = this;
        weaponObj.name = weaponType.ToString();
        return weaponObj;
    }

    /// <summary>
    /// Rotaciona o container da arma (c�rculo) para apontar a m�o do jogador em dire��o ao mouse.
    /// </summary>
    private void RotateTo(Vector3 point)
    {
        Vector3 direction = point - transform.position;

        AimAngleDegrees = Mathf.Atan2(direction.y, direction.x).RadToDeg();
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, AimAngleDegrees));

        SetHandAimOffset();
    }

    /// <summary>
    /// Ajusta a posição da mão do jogador para que ela fique na ponta do container de mira.
    /// </summary>
    private void SetHandAimOffset()
    {
        float orbitRadius = transform.localScale.x / 2;
        Vector3 offset = new Vector3(Mathf.Cos(AimAngleDegrees * Mathf.Deg2Rad), Mathf.Sin(AimAngleDegrees * Mathf.Deg2Rad), 0) * orbitRadius;
        handTransform.position = transform.position + offset;
        throwingContainerTransform.position = transform.position + offset;
    }
}
