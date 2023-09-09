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

    Transform handTransform, throwingContainerTransform, throwableSpawnPointTransform;
    float startThrowingContainerScale;
    float? startSwitchTime;
    SpriteRenderer playerSprite;
    Animator playerAnimator;

    readonly FireModes[] HoldTriggerFireModes = { FireModes.FullAuto, FireModes.Melee };

    private void Awake()
    {
        handTransform = transform.Find("Hand");
        throwingContainerTransform = handTransform.Find("ThrowingContainer");
        throwableSpawnPointTransform = throwingContainerTransform.Find("Palm").Find("ThrowableSpawnPoint");
    }

    // Start is called before the first frame update
    void Start()
    {
        Player = transform.parent.GetComponent<Player>();
        playerSprite = Player.GetComponent<SpriteRenderer>();
        playerAnimator = Player.GetComponent<Animator>();
        StartLocalPosition = transform.localPosition;
        startThrowingContainerScale = throwingContainerTransform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.IsAlive)
            return;

        if (MenuController.Instance.IsGamePaused)
            return;

        bool isFiring;

        if (HoldTriggerFireModes.Contains(Player.CurrentWeapon.FireMode))
            isFiring = Input.GetKey(KeyCode.Mouse0);
        else
            isFiring = Input.GetKeyDown(KeyCode.Mouse0);

        if (isFiring)
        {
            bool isFiringBurst = Player.CurrentWeapon is BurstFireWeapon && (Player.CurrentWeapon as BurstFireWeapon).IsFiringBurst;
            if (!isFiringBurst)
                Player.CurrentWeapon.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
            Player.CurrentWeapon.Reload();

        if (Input.GetKey(KeyCode.G))
            StartThrowingItem();

        if (Input.GetKeyUp(KeyCode.G))
            ThrowItem();

        if (Input.mouseScrollDelta.y != 0)
            SwitchWeapon();

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);

        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
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
    }

    private void StartThrowingItem()
    {
        playerAnimator.SetTrigger("Throw");
        playerAnimator.SetFloat("ThrowSpeedMultiplier", 0);
        Player.CurrentWeapon.IsActive = false;
        if (!IsThrowingItem)
        {
            var throwable = InstantiateThrowablePrefab(ThrowableTypes.FragGrenade);
            var rb = throwable.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            var collider = throwable.GetComponent<Collider2D>();
            collider.enabled = false;
        }
        IsThrowingItem = true;
    }

    private void ThrowItem()
    {
        playerAnimator.SetFloat("ThrowSpeedMultiplier", 1);
    }

    public void OnItemThrown()
    {
        var throwableObj = throwableSpawnPointTransform.GetChild(0);
        var rb = throwableObj.GetComponent<Rigidbody2D>();
        var collider = throwableObj.GetComponent<Collider2D>();
        var throwable = throwableObj.GetComponent<BaseThrowable>();
        rb.isKinematic = false;
        collider.enabled = true;
        throwable.Throw();
    }

    public void OnThrowEnd()
    {
        Player.CurrentWeapon.IsActive = true;
        IsThrowingItem = false;
        playerAnimator.ResetTrigger("Throw");
    }

    /// <summary>
    /// Alternar entre as armas primária e secundária equipadas.
    /// </summary>
    /// <param name="index">O índice da arma a ser equipada. 0 = primária, 1 = secundária. Null = inverter.</param>
    public void SwitchWeapon(int? index = null)
    {
        if (IsSwitchingWeapon)
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
            float animAngle = Mathf.Lerp(90, IsAimingLeft ? 180 : 0, t);

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
        throwableObj.GetComponent<BaseThrowable>().PlayerWeaponController = this;
        throwableObj.name = throwableType.ToString();
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
