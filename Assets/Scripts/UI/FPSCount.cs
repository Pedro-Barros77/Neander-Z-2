using TMPro;
using UnityEngine;

public class FPSCount : MonoBehaviour
{
    private TextMeshProUGUI FpsText;
    int frameCount = 0;
    float elapsedTime = 0.0f;
    float fps = 0.0f;
    public float refreshRate = 0.5f;

    void Start()
    {
        FpsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        CalculateFPS();
    }
    /// <summary>
    /// Função que calcula o FPS atual.
    /// </summary>
    private void CalculateFPS()
    {
        if (elapsedTime < refreshRate)
        {
            elapsedTime += Time.deltaTime;
            frameCount++;
            return;
        }

        fps = frameCount / elapsedTime;
        frameCount = 0;
        elapsedTime = 0.0f;

        FpsText.text = $"FPS: {Mathf.Floor(fps)}";
    }
}
