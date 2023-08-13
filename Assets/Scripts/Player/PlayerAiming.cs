using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public float AimAngle { get; set; }

    Transform handTransform;

    // Start is called before the first frame update
    void Start()
    {
        handTransform = transform.Find("Hand");
    }

    // Update is called once per frame
    void Update()
    {
        RotateToMouse();
    }

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
