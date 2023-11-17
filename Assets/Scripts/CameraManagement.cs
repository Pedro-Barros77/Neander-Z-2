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

    private bool isFocusing;
    private float zoomLevel;
    private float startZoom;
    private float focusStartTime;
    private float cameraMovementDurationMs;
    private Vector3 focusPosition, focusStartPosition;

    Camera cam;
    SpriteRenderer playerSprite;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        startCameraY = transform.position.y;
        startZoom = cam.orthographicSize;
    }

    void Update()
    {
        if (isFocusing)
            Camera.main.transform.position = GetCameraPosition(focusPosition);
        else if (!isShaking)
            transform.position = GetCameraPosition();
    }
    /// <summary>
    /// Função responsável por retornar a posição da câmera de acordo com a posição do player.
    /// </summary>
    /// <returns></returns>
    Vector3 GetCameraPosition(Vector3? basePosition = null)
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

        Vector3 result = new(
            Mathf.Clamp(basePosition != null ? basePosition.Value.x : camX, boundaryLeft + halfCamWidth, boundaryRight - halfCamWidth),
            basePosition != null ? basePosition.Value.y : startCameraY,
            Camera.main.transform.position.z);

        if (isFocusing)
        {
            if (Time.unscaledTime < (cameraMovementDurationMs / 1000) + focusStartTime)
            {
                float percentage = (Time.unscaledTime - focusStartTime) / (cameraMovementDurationMs / 1000);
                percentage = percentage * percentage * (3f - 2f * percentage);
                cam.orthographicSize = Mathf.Lerp(startZoom, zoomLevel, percentage);
                return Vector3.Lerp(focusStartPosition, result, percentage);
            }
            else if (cameraMovementDurationMs == 0)
                cam.orthographicSize = zoomLevel;
        }
        return result;

    }
    /// <summary>
    /// Função responsável por fazer a câmera tremer.
    /// </summary>
    /// <param name="durationMs">Duração em milessegundos</param>
    /// <param name="strenght">Mutiplicador do efeito de tremor</param>
    /// <returns></returns>
    public IEnumerator ShakeCameraEffect(float durationMs, float strenght = 1, bool useFadeOut = true)
    {
        float elapsedTime = 0f;
        isShaking = true;

        while (elapsedTime < durationMs / 1000)
        {
            if (MenuController.Instance.IsGamePaused)
                break;
            elapsedTime += Time.unscaledDeltaTime;
            float curveValue = useFadeOut ? ScreenShakeCurve.Evaluate(elapsedTime / (durationMs / 1000)) : 0.3f;
            Vector3 defaultPosition = isFocusing ? GetCameraPosition(focusPosition) : GetCameraPosition();
            Vector3 resultPos = defaultPosition + Random.insideUnitSphere * strenght * curveValue;
            Camera.main.transform.position = new(resultPos.x, resultPos.y, Camera.main.transform.position.z);
            yield return null;
        }
        isShaking = false;
    }
    /// <summary>
    /// Função responsável por fazer a câmera focar em uma posição.
    /// </summary>
    /// <param name="targetPosition">Posição do alvo a focar </param>
    /// <param name="zoom">O zoom que câmera amplia</param>
    /// <param name="cameraMovementDurationMs">Tempo que demora para mover a câmera até o foco </param>
    public void FocusOnPosition(Vector3 targetPosition, float zoom, float? cameraMovementDurationMs = null)
    {
        this.cameraMovementDurationMs = cameraMovementDurationMs ?? 0;
        focusStartTime = Time.unscaledTime;
        focusStartPosition = Camera.main.transform.position;
        zoomLevel = Mathf.Max(zoom, 1.0f);
        focusPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        isFocusing = true;
    }
    /// <summary>
    /// Função responsável por fazer a câmera desfocar no player.
    /// </summary>
    public void Unfocus()
    {
        cam.orthographicSize = startZoom;
        isFocusing = false;
        Camera.main.transform.position = GetCameraPosition();
    }

}
