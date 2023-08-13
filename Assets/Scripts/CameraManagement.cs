using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    [SerializeField]
    public Transform cameraBoundary, player;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float boundaryLeft = cameraBoundary.position.x - cameraBoundary.transform.localScale.x / 2;
        float boundaryRight = cameraBoundary.position.x + cameraBoundary.transform.localScale.x / 2;

        float halfCamHeight = cam.orthographicSize;
        float halfCamWidth = cam.aspect * halfCamHeight;

        gameObject.transform.position = new Vector3(
            Mathf.Clamp(player.position.x, boundaryLeft + halfCamWidth, boundaryRight - halfCamWidth),
            gameObject.transform.position.y,
            gameObject.transform.position.z);
    }
}
