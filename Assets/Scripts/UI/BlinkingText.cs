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
    public bool DisableOnStartFinish;

    private TextMeshProUGUI textComponent;
    private Image backgroundImage;
    private bool isBlinking = false;
    private Color backgroundStartColor;

    Coroutine blinkRoutine;

    private void Start()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
            backgroundStartColor = backgroundImage.color;
        textComponent.color = FirstColor;
        if (DisableOnStartFinish)
        {
            this.enabled = false;
            textComponent.color = Color.white;
        }
        else
            RestartBlinking();
    }

    private void Update()
    {
        if (backgroundImage != null)
            backgroundImage.enabled = ShowBackground;
    }

    /// <summary>
    /// Para a rotina de animação atual e inicia uma nova, com os valores iniciais.
    /// </summary>
    public void RestartBlinking()
    {
        StopBlinking();

        if (textComponent != null)
            textComponent.color = FirstColor;
        if (backgroundImage != null && BlinkBackground)
            backgroundImage.color = backgroundStartColor;

        isBlinking = true;
        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    /// <summary>
    /// Para a rotina de animação atual.
    /// </summary>
    private void StopBlinking()
    {
        StopAllCoroutines();
        isBlinking = false;
        blinkRoutine = null;
    }

    /// <summary>
    /// Rotina de animação.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Realiza a interpolação da cor, de acordo com os parâmetros informados.
    /// </summary>
    /// <param name="startColor">A cor inicial da interpolação.</param>
    /// <param name="endColor">A cor final da interpolação.</param>
    /// <param name="durationMs">A duração da transição entre uma cor e outra.</param>
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
