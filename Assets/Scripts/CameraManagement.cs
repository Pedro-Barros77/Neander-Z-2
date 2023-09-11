using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    [SerializeField]
    public Transform cameraBoundary, player;
    [SerializeField]
    public HorizontalDir CameraAlignment = HorizontalDir.Center;


    Camera cam;
    SpriteRenderer playerSprite;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        playerSprite = player.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float boundaryLeft = cameraBoundary.position.x - cameraBoundary.transform.localScale.x / 2;
        float boundaryRight = cameraBoundary.position.x + cameraBoundary.transform.localScale.x / 2;

        float halfCamHeight = cam.orthographicSize;
        float halfCamWidth = cam.aspect * halfCamHeight;

        float playerWidth = playerSprite.bounds.size.x;

        float camX;
        switch (CameraAlignment)
        {
            case HorizontalDir.Left:
                camX = player.position.x + halfCamWidth - playerWidth;
                break;
            case HorizontalDir.Right:
                camX = player.position.x - halfCamWidth + playerWidth;
                break;
            default:
                camX = player.position.x;
                break;
        }

        gameObject.transform.position = new Vector3(
            Mathf.Clamp(camX, boundaryLeft + halfCamWidth, boundaryRight - halfCamWidth),
            gameObject.transform.position.y,
            gameObject.transform.position.z);
    }
}
