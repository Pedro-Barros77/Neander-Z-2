using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ProgressBar : MonoBehaviour
{
    public float MaxValue { get; private set; }
    public float Value { get; private set; }
    private Color _valueFillColor = Color.red;
    public Color ValueFillColor
    {
        get { return _valueFillColor; }
        set
        {
            _valueFillColor = value;
            fillImage.color = value;
        }
    }

    private bool _useAnimation = true;
    public bool UseAnimation
    {
        get { return _useAnimation; }
        set
        {
            _useAnimation = value;
            animationFill.SetActive(value);
        }
    }
    public float AnimationSpeed { get; set; } = 10f;
    public Color AnimationIncreaseColor { get; set; } = Color.green;
    public Color AnimationDecreaseColor { get; set; } = Color.yellow;
    public bool HideOnFull { get; set; }
    private bool _useShadows = true;
    public bool UseShadows
    {
        get { return _useShadows; }
        set
        {
            _useShadows = value;
            shadows.SetActive(value);
        }
    }
    private bool _useOutline = true;
    public bool UseOutline
    {
        get { return _useOutline; }
        set
        {
            _useOutline = value;
            outline.SetActive(value);
        }
    }


    Slider slider;
    Slider animationSlider;
    GameObject background, animationFill, fill, outline, shadows;
    Image animationFillImage, fillImage;
    float targetValue;
    float startAnimationValue;

    private void Awake()
    {
        var animationBar = transform.Find("AnimationBar");
        animationSlider = animationBar.GetComponent<Slider>();
        slider = GetComponent<Slider>();
        animationFill = animationBar.Find("AnimationFill").gameObject;
        outline = transform.Find("Outline").gameObject;
        shadows = transform.Find("Shadows").gameObject;
        fill = transform.Find("Fill").gameObject;
        fillImage = fill.GetComponent<Image>();
    }

    void Start()
    {
        background = transform.Find("Background").gameObject;
        animationFillImage = animationFill.GetComponent<Image>();

        animationFill.SetActive(UseAnimation);
        outline.SetActive(UseOutline);
        shadows.SetActive(UseShadows);
    }

    void Update()
    {
        if (Value == MaxValue && HideOnFull && fill.activeSelf)
            SetVisible(false);
        else if(Value < MaxValue && !fill.activeSelf)
            SetVisible(true);
    }

    private void FixedUpdate()
    {
        if (UseAnimation)
            UpdateAnimation();
    }

    /// <summary>
    /// Adiciona valor à barra.
    /// </summary>
    /// <param name="value">O valor a ser adicionado.</param>
    public void AddValue(float value)
    {
        if (value <= 0 || Value == MaxValue) return;

        if (UseAnimation)
        {
            startAnimationValue = Value;
            targetValue = Mathf.Clamp(targetValue + value, 0, MaxValue);
            animationSlider.value = targetValue;
        }
        else
        {
            Value = Mathf.Clamp(Value + value, 0, MaxValue);
            slider.value = Value;
        }
    }

    /// <summary>
    /// Remove valor da barra.
    /// </summary>
    /// <param name="value">O valor a ser removido.</param>
    public void RemoveValue(float value)
    {
        if (value <= 0 || Value == 0) return;

        if (UseAnimation)
        {
            startAnimationValue = Value;
            targetValue = Mathf.Clamp(targetValue - value, 0, MaxValue);
            animationSlider.value = targetValue;
        }
        else
        {
            Value = Mathf.Clamp(Value - value, 0, MaxValue);
            slider.value = Value;
        }
    }

    /// <summary>
    /// Define o valor da barra quando completa.
    /// </summary>
    /// <param name="value">O valor a ser difinido como total (100%).</param>
    /// <param name="setValue">Se o valor atual da barra também deve ser preenchido com o total.</param>
    public void SetMaxValue(float value, bool setValue = false)
    {
        MaxValue = value;
        targetValue = value;
        slider.maxValue = value;
        animationSlider.maxValue = value;
        if (setValue)
        {
            Value = value;
            slider.value = value;
            animationSlider.value = value;
        }
    }

    /// <summary>
    /// Atualiza a animação da barra.
    /// </summary>
    private void UpdateAnimation()
    {
        float diff = targetValue - Value;
        float speedProportionMultiplier = 10;
        float speedProportion = Mathf.Clamp(1 + (Mathf.Abs(diff) / Mathf.Max(targetValue, startAnimationValue) * speedProportionMultiplier), 0, speedProportionMultiplier);

        if (diff > 0)
        {
            Value = Mathf.Clamp(Value + AnimationSpeed * speedProportion * Time.deltaTime, 0, targetValue);
            slider.value = Value;

            animationSlider.transform.SetSiblingIndex(1);
            animationFillImage.color = AnimationIncreaseColor;
            fillImage.color = ValueFillColor;
        }
        else if (diff < 0)
        {
            Value = Mathf.Clamp(Value - AnimationSpeed * speedProportion * Time.deltaTime, targetValue, MaxValue);
            slider.value = Value;

            animationSlider.transform.SetSiblingIndex(2);
            animationFillImage.color = ValueFillColor;
            fillImage.color = AnimationDecreaseColor;
        }
        else
        {
            animationSlider.transform.SetSiblingIndex(1);
            fillImage.color = ValueFillColor;
        }
    }

    /// <summary>
    /// Define se a barra deve ser visível ou não.
    /// </summary>
    /// <param name="visible"></param>
    private void SetVisible(bool visible)
    {
        if (UseAnimation || !visible)
            animationFill.SetActive(visible);

        background.SetActive(visible);
        fill.SetActive(visible);

        if (UseOutline || !visible)
            outline.SetActive(visible);
        if (UseShadows || !visible)
            shadows.SetActive(visible);
    }
}
