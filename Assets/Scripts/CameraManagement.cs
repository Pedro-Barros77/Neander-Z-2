using System.Collections;
using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    [SerializeField]
    public Transform cameraBoundary, player;
    [SerializeField]
    public HorizontalDir CameraAlignment = HorizontalDir.Center;

    [SerializeField]
    public AnimationCurve ScreenShakeCurve;

    bool isShaking;
    float startCameraY;

    Camera cam;
    SpriteRenderer playerSprite;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        startCameraY = transform.position.y;
    }

    void Update()
    {
        if (!isShaking)
            transform.position = GetCameraPosition();
    }
    /// <summary>
    /// Função responsável por retornar a posição da câmera de acordo com a posição do player.
    /// </summary>
    /// <returns></returns>
    Vector3 GetCameraPosition()
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

        return new Vector3(
            Mathf.Clamp(camX, boundaryLeft + halfCamWidth, boundaryRight - halfCamWidth),
            startCameraY,
            gameObject.transform.position.z);

    }
    /// <summary>
    /// Função responsável por fazer a câmera tremer.
    /// </summary>
    /// <param name="durationMs">Duração em milessegundos</param>
    /// <param name="strenght">Mutiplicador do efeito de tremor</param>
    /// <returns></returns>
    public IEnumerator ShakeCameraEffect(float durationMs, float strenght = 1)
    {
        float elapsedTime = 0f;
        isShaking = true;

        while (elapsedTime < durationMs / 1000)
        {
            if (MenuController.Instance.IsGamePaused)
                break;
            elapsedTime += Time.deltaTime;
            float curveValue = ScreenShakeCurve.Evaluate(elapsedTime / durationMs);
            Camera.main.transform.position = GetCameraPosition() + Random.insideUnitSphere * strenght * curveValue;
            yield return null;
        }
        isShaking = false;
    }

}
