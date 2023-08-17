using UnityEngine;
using UnityEngine.XR;

public class PlayerWeaponController : MonoBehaviour
{
    /// <summary>
    /// O �ngulo de mira do jogador, em Radians (Come�ando da direita, sentido anti-hor�rio: direita 0, cima 90, esquerda 180, esquerda -180, baixo -90, direita 0).
    /// </summary>
    public float AimAngle { get; set; }

    /// <summary>
    /// O jogador pai deste controlador.
    /// </summary>
    public Player Player { get; set; }
    public Vector3 StartLocalScale { get; private set; }
    public Vector3 StartLocalPosition{ get; private set; }

    Transform handTransform;

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

        RotateToMouse();
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
}
