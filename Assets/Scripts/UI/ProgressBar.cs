using UnityEngine;
using UnityEngine.UI;

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

    private bool _useAnimation;
    public bool UseAnimation
    {
        get { return _useAnimation; }
        set
        {
            _useAnimation = value;
            animationFill.SetActive(value);
        }
    }
    public float AnimationSpeed { get; set; }
    public Color AnimationIncreaseColor { get; set; } = Color.green;
    public Color AnimationDecreaseColor { get; set; } = Color.yellow;
    public bool HideOnFull { get; set; }

    Slider slider;
    Slider animationSlider;
    GameObject background, animationFill, fill, outline, shadows;
    Image animationFillImage, fillImage;
    float targetValue;
    float startAnimationValue;

    void Start()
    {
        var animationBar = transform.Find("AnimationBar");
        animationSlider = animationBar.GetComponent<Slider>();
        slider = GetComponent<Slider>();

        animationFill = animationBar.Find("AnimationFill").gameObject;
        background = transform.Find("Background").gameObject;
        fill = transform.Find("Fill").gameObject;
        outline = transform.Find("Outline").gameObject;
        shadows = transform.Find("Shadows").gameObject;

        fillImage = fill.GetComponent<Image>();
        animationFillImage = animationFill.GetComponent<Image>();
    }

    void Update()
    {
        if (Value == MaxValue && HideOnFull && fill.activeSelf)
            SetVisible(false);
    }

    private void FixedUpdate()
    {
        if (UseAnimation)
            UpdateAnimation();
    }

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

    public void RemoveValue(float value)
    {
        if (value <= 0 || Value == 0) return;

        if (HideOnFull && Value == MaxValue)
            SetVisible(true);

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

    private void SetVisible(bool visible)
    {
        if (UseAnimation || !visible)
            animationFill.SetActive(visible);
        background.SetActive(visible);
        fill.SetActive(visible);
        outline.SetActive(visible);
        shadows.SetActive(visible);
    }
}
