using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupSystem : MonoBehaviour
{
    //public static PopupSystem Instance { get; private set; }
    //[SerializeField] private GameObject popupTextPrefab;
    private TextMeshProUGUI PopupText;
    private Color TextColor;
    private float LifeSpanMs;
    private Vector3 StartPosition;
    private string Text;
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
    public void Init(string text, Vector3 position, float lifeSpanMs, Color32? textColor = null)
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
    }

}
