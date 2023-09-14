using TMPro;
using UnityEngine;

public class PopupSystem : MonoBehaviour
{
    private TextMeshProUGUI PopupText;
    private Color TextColor;
    private float LifeSpanMs;
    private Vector3 StartPosition;
    private string Text;
    private RectTransform rectTransform;
    void Start()
    {
        PopupText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (PopupText != null)
        {
            PopupText.text = Text;
            PopupText.color = TextColor;
        }
    }

    void Update()
    {
    }

    /// <summary>
    /// Função para inicializar o popup.
    /// </summary>
    /// <param name="text">O texto a ser exibido</param>
    /// <param name="position">A posição inicial</param>
    /// <param name="lifeSpanMs">O tempo para o popup desaparecer(ser destruído)</param>
    /// <param name="textColor">A cor que o popup vai ser exibido</param>
    /// <param name="scale">A escala (tamanho) do popup. O padrão é 1.</param>
    public void Init(string text, Vector3 position, float lifeSpanMs, Color32? textColor = null, float scale = 1)
    {
        Text = text;
        TextColor = textColor ?? Color.white;
        StartPosition = position;
        LifeSpanMs = lifeSpanMs;
        Destroy(gameObject, LifeSpanMs / 1000f);

        if (PopupText != null)
        {
            PopupText.text = Text;
            PopupText.color = TextColor;
        }
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one * scale;
    }

}
