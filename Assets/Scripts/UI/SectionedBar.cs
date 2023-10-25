using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SectionedBar : MonoBehaviour
{
    public float MaxValue;
    public float Value;
    public float ModificationValue;
    public Color32 UpgradeColor, DowngradeColor;
    public bool BlinkModification;
    public float BlinkDelayMs;
    public bool HideOnFull;
    public int SectionsCount => backgroundImage?.transform.childCount ?? 0;

    Image backgroundImage;
    Image[,] Sections;
    CanvasGroup canvasGroup;
    Color32 StartFillColor;
    bool isBlinkingModification;

    void Start()
    {
        backgroundImage = transform.Find("Background").GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        LoadSections();
        CalculateSections();
    }

    void Update()
    {
        if (Value == MaxValue && HideOnFull && canvasGroup.alpha != 0)
            canvasGroup.alpha = 0;
        else if (Value < MaxValue && canvasGroup.alpha != 1)
            canvasGroup.alpha = 1;

        if (BlinkModification && !isBlinkingModification)
            StartCoroutine(BlinkModificationLoop());
    }

    /// <summary>
    /// Adiciona valor à barra.
    /// </summary>
    /// <param name="value">O valor a ser adicionado.</param>
    public void AddValue(float value)
    {
        if (value <= 0 || Value == MaxValue) return;

        Value = Mathf.Clamp(Value + value, 0, MaxValue);
        CalculateSections();
    }

    /// <summary>
    /// Remove valor da barra.
    /// </summary>
    /// <param name="value">O valor a ser removido.</param>
    public void RemoveValue(float value)
    {
        if (value <= 0 || Value == 0) return;

        Value = Mathf.Clamp(Value - value, 0, MaxValue);
        CalculateSections();
    }

    /// <summary>
    /// Calcula quais seções devem ser exibidas e quais cores devem ser aplicadas, baseado no Value, MaxValue e ModificationValue.
    /// </summary>
    public void CalculateSections()
    {
        int sectionsCount = Sections.GetLength(0);
        float fullSectionValue = MaxValue / sectionsCount;

        for (int i = 0; i < sectionsCount; i++)
        {
            float half1Value = fullSectionValue / 2 * (i * 2 + 1);
            float half2Value = half1Value + (fullSectionValue / 2);

            Image half1 = Sections[i, 0];
            Image half2 = Sections[i, 1];

            float modification = ModificationValue + Value;

            if (half1Value > Value && half1Value.LessOrAproxEqual(modification))
                half1.color = UpgradeColor;
            else if (half1Value <= Value && half1Value > modification)
                half1.color = DowngradeColor;
            else
                half1.color = StartFillColor;

            if (half2Value > Value && half2Value.LessOrAproxEqual(modification))
                half2.color = UpgradeColor;
            else if (half2Value <= Value && half2Value > modification)
                half2.color = DowngradeColor;
            else
                half2.color = StartFillColor;

            half1.enabled = Value.GreaterOrAproxEqual(half1Value) || half1.color != StartFillColor;
            half2.enabled = Value.GreaterOrAproxEqual(half2Value) || half2.color != StartFillColor;
        }
    }

    /// <summary>
    /// Realiza a animação de blink das barras de upgrade/downgrade.
    /// </summary>
    IEnumerator BlinkModificationLoop()
    {
        isBlinkingModification = true;

        IEnumerable<Image> GetModificationSections()
        {
            var query = from Image s in Sections
                        where s.color == UpgradeColor || s.color == DowngradeColor
                        select s;

            return query;
        }

        while (BlinkModification)
        {
            foreach (Image section in GetModificationSections())
                section.enabled = !section.enabled;

            yield return new WaitForSeconds(BlinkDelayMs / 1000);
        }

        foreach (Image section in GetModificationSections())
            section.enabled = true;

        isBlinkingModification = false;
    }

    /// <summary>
    /// Carrega todas as seções dentro do objeto para o array Sections.
    /// </summary>
    void LoadSections()
    {
        Sections = new Image[SectionsCount, 2];

        for (int i = 0; i < SectionsCount; i++)
        {
            var section = backgroundImage.transform.GetChild(i);
            var fill = section.Find("Fill");
            var half1 = fill.GetChild(0).GetComponent<Image>();
            var half2 = fill.GetChild(1).GetComponent<Image>();
            if (i == 0)
                StartFillColor = half1.color;

            Sections[i, 0] = half1;
            Sections[i, 1] = half2;
        }
    }
}
