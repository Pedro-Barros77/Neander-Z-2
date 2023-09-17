using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    public string Text
    {
        get => textComponent.text;
        set => textComponent.text = value;
    }

    public Color32 FirstColor = new Color32(255, 0, 0, 255);
    public float FirstColorDurationMs = 200f;

    public Color32 SecondColor = new Color32(255, 0, 0, 0);
    public float SecondColorDurationMs = 100f;

    public float FirstTransitionDurationMs = 200f;
    public float SecondTransitionDurationMs = 200f;

    public bool ShowBackground;
    public bool BlinkBackground;

    private TextMeshProUGUI textComponent;
    private Image backgroundImage;
    private bool isBlinking = false;
    private Color backgroundStartColor;

    Coroutine blinkRoutine;

    private void Start()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        backgroundImage = GetComponent<Image>();
        backgroundStartColor = backgroundImage.color;
        textComponent.color = FirstColor;
        RestartBlinking();
    }

    private void Update()
    {
        backgroundImage.enabled = ShowBackground;
    }

    public void RestartBlinking()
    {
        StopAllCoroutines();
        isBlinking = true;
        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    private void StopBlinking()
    {
        StopAllCoroutines();
        isBlinking = false;
        blinkRoutine = null;
    }

    private IEnumerator BlinkRoutine()
    {
        while (isBlinking)
        {
            yield return new WaitForSeconds(FirstColorDurationMs / 1000f);
            yield return LerpColor(FirstColor, SecondColor, FirstTransitionDurationMs);
            yield return new WaitForSeconds(SecondColorDurationMs / 1000f);
            yield return LerpColor(SecondColor, FirstColor, SecondTransitionDurationMs);
        }
    }

    private IEnumerator LerpColor(Color32 startColor, Color32 endColor, float durationMs)
    {
        float startTime = Time.time;
        float endTime = startTime + (durationMs / 1000f);

        while (Time.time < endTime)
        {
            if (textComponent == null)
            {
                StopBlinking();
                yield break;
            }
            float t = (Time.time - startTime) / (endTime - startTime);
            textComponent.color = Color32.Lerp(startColor, endColor, t);
            if (BlinkBackground)
                backgroundImage.color = new Color(backgroundStartColor.r, backgroundStartColor.g, backgroundStartColor.b, backgroundStartColor.a * textComponent.color.a);
            yield return null;
        }

        if (BlinkBackground)
            backgroundImage.color = new Color(backgroundStartColor.r, backgroundStartColor.g, backgroundStartColor.b, backgroundStartColor.a * textComponent.color.a);

        textComponent.color = endColor;

    }

    private void OnDestroy()
    {
        if (blinkRoutine != null)
            StopBlinking();
    }

    private void OnDisable()
    {
        if (blinkRoutine != null)
            StopBlinking();
    }

    private void OnEnable()
    {
        if (blinkRoutine == null)
            RestartBlinking();
    }
}
