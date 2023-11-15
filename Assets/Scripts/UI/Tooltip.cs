using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public string Text { get; private set; }
    [SerializeField]
    public float HoverDelayMs = 600f;
    [SerializeField]
    BaseButton Button;
    [SerializeField]
    GameObject TextContainer, Arrow;
    [SerializeField]
    TextMeshProUGUI Label;

    Coroutine ActivationCoroutine;

    void Start()
    {
        if (Button != null)
            Button.HoverEvent += OnHover;
    }

    public void SetText(string text)
    {
        Text = text;
        Label.text = text;
    }

    void OnHover(BaseButton button, bool hovered)
    {
        if (hovered)
            ActivationCoroutine = StartCoroutine(SetVisibleDelayed(hovered));
        else
        {
            if (ActivationCoroutine != null)
                StopCoroutine(ActivationCoroutine);
            SetVisible(false);
        }
    }

    IEnumerator SetVisibleDelayed(bool visible)
    {
        yield return new WaitForSecondsRealtime(HoverDelayMs / 1000f);
        SetVisible(visible);
    }

    public void SetVisible(bool visible)
    {
        if (TextContainer != null)
            TextContainer.SetActive(visible);

        if (Arrow != null)
            Arrow.SetActive(visible);
    }
}
