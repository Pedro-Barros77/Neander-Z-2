using UnityEngine;
using UnityEngine.XR;

public class PlayerWeaponController : MonoBehaviour
{
    /// <summary>
    /// O �ngulo de mira do jogador, em Radians (Come�ando da direita, sentido anti-hor�rio: direita 0, cima 90, esquerda 180, esquerda -180, baixo -90, direita 0).
    /// </summary>
    public float AimAngle { get; set; }
    public float AimAngleInDegrees => AimAngle * Mathf.Rad2Deg;

    /// <summary>
    /// O jogador pai deste controlador.
    /// </summary>
    public Player Player { get; set; }
    public Vector3 StartLocalScale { get; private set; }
    public Vector3 StartLocalPosition { get; private set; }

    Transform handTransform;
    Transform PrimaryWeaponObject, SecondaryWeaponObject;

    private void Awake()
    {
        handTransform = transform.Find("Hand");
    }

    // Start is called before the first frame update
    void Start()
    {
        Player = transform.parent.GetComponent<Player>();
        StartLocalScale = transform.localScale;
        StartLocalPosition = transform.localPosition;
        PrimaryWeaponObject = handTransform.Find(Player.Backpack.EquippedPrimaryType.ToString());
        SecondaryWeaponObject = handTransform.Find(Player.Backpack.EquippedSecondaryType.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (MenuController.Instance.IsGamePaused)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            Player.CurrentWeapon.Shoot();

        if (Input.GetKeyDown(KeyCode.R))
            Player.CurrentWeapon.Reload();

        if (Input.mouseScrollDelta.y != 0)
            SwitchWeapon();

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);

        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(1);

        RotateToMouse();
    }

    /// <summary>
    /// Alternar entre as armas primária e secundária equipadas.
    /// </summary>
    /// <param name="index">O índice da arma a ser equipada. 0 = primária, 1 = secundária. Null = inverter.</param>
    public void SwitchWeapon(int? index = null)
    {
        if (index == null || index != Player.Backpack.CurrentWeaponIndex)
            Player.Backpack.SwitchWeapon(index);

        bool equippedPrimary = Player.Backpack.CurrentWeaponIndex == 0;

        PrimaryWeaponObject = handTransform.Find(Player.Backpack.EquippedPrimaryType.ToString());
        SecondaryWeaponObject = handTransform.Find(Player.Backpack.EquippedSecondaryType.ToString());

        TogglePrimary(equippedPrimary);
        ToggleSecondary(!equippedPrimary);
    }


    public void SetWeaponOffset(Vector3 localPositionOffset)
    {
        transform.localPosition = StartLocalPosition + localPositionOffset;
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
        weaponObj.SetActive(false);
        return weaponObj;
    }

    /// <summary>
    /// Rotaciona o container da arma (c�rculo) para apontar a m�o do jogador em dire��o ao mouse.
    /// </summary>
    private void RotateToMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z));
        Vector3 direction = worldMousePos - transform.position;

        AimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, AimAngle));

        float orbitRadius = transform.localScale.x / 2;
        Vector3 offset = new Vector3(Mathf.Cos(AimAngle * Mathf.Deg2Rad), Mathf.Sin(AimAngle * Mathf.Deg2Rad), 0) * orbitRadius;
        handTransform.position = transform.position + offset;
    }

    /// <summary>
    /// Ativa/desativa a arma primária, se existir alguma equipada.
    /// </summary>
    /// <param name="active">Se a arma deve ser ativada ou desativada.</param>
    private void TogglePrimary(bool active)
    {
        if (PrimaryWeaponObject != null)
            PrimaryWeaponObject.gameObject.SetActive(active);
    }

    /// <summary>
    /// Ativa/desativa a arma secundária, se existir alguma equipada.
    /// </summary>
    /// <param name="active">Se a arma deve ser ativada ou desativada.</param>
    private void ToggleSecondary(bool active)
    {
        if (SecondaryWeaponObject != null)
            SecondaryWeaponObject.gameObject.SetActive(active);
    }
}
