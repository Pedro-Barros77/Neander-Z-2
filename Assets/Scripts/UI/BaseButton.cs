using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button Button { get; private set; }
    public bool Pressed { get; private set; }
    public bool IsPressing { get; private set; }
    public bool Released { get; private set; }
    public bool IsHovering { get; private set; }
    public Vector3 ClickPosition { get; private set; }
    [SerializeField]
    public CustomAudio ClickSound, HoverSound;
    [SerializeField]
    bool ChangesCursorOnHover = true;
    [SerializeField]
    bool IsClickable = true;
    [SerializeField]
    string TooltipText;
    [SerializeField]
    Tooltip Tooltip;
    TextMeshProUGUI BtnLabel;

    public delegate void OnHover(BaseButton button, bool hovered);
    public event OnHover HoverEvent;

    private Animator animator;
    private AudioSource audioSource;
    private int touchId = -1;

    void Start()
    {
        animator = GetComponent<Animator>();
        Button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
        BtnLabel = GetComponentInChildren<TextMeshProUGUI>();
        if (Tooltip != null)
        {
            Tooltip.HoverDelayMs = 1000f;
            if (TooltipText.IsNullOrEmpty())
            {
                if (BtnLabel != null)
                    Tooltip.SetText(BtnLabel.text);
            }
            else
                Tooltip.SetText(TooltipText);
        }
    }

    void Update()
    {
        if (IsPressing)
            UpdateClickPosition();

        if (Tooltip != null && Button != null && !Button.interactable)
            Tooltip.SetVisible(false);
    }

    private void LateUpdate()
    {
        Pressed = false;
        Released = false;
    }

    /// <summary>
    /// Define o texto da tooltip do botão.
    /// </summary>
    /// <param name="text">O texto a ser definido.</param>
    public void SetToolTip(string text)
    {
        if (Tooltip != null)
            Tooltip.SetText(text);
    }

    /// <summary>
    /// Define o texto do botão.
    /// </summary>
    /// <param name="text">O texto a ser definido.</param>
    public void SetLabelText(string text)
    {
        if (BtnLabel != null)
            BtnLabel.text = text;
    }

    /// <summary>
    /// Função chamada quando o botão é clicado.
    /// </summary>
    public void OnClick()
    {
        if (Button != null && !Button.interactable)
            return;

        if (!IsClickable)
            return;

        ClickSound.PlayIfNotNull(audioSource, AudioTypes.UI);
        if (Tooltip != null)
            Tooltip.SetVisible(false);

        IsPressing = true;
        Pressed = true;
    }

    /// <summary>
    /// Função chamada quando o mouse passa por cima do botão.
    /// </summary>
    public void OnHoverIn()
    {
        if (Button != null && !Button.interactable)
            return;

        HoverSound.PlayIfNotNull(audioSource, AudioTypes.UI);

        IsHovering = true;
        HoverEvent?.Invoke(this, true);

        if (ChangesCursorOnHover)
            MenuController.Instance.SetCursor(Cursors.Pointer);
    }

    /// <summary>
    /// Função chamada quando o mouse sai de cima do botão.
    /// </summary>
    public void OnHoverOut()
    {
        if (ChangesCursorOnHover)
            MenuController.Instance.SetCursor(Cursors.Arrow);

        if (Button != null && !Button.interactable)
            return;

        IsHovering = false;
        HoverEvent?.Invoke(this, false);

    }

    /// <summary>
    /// Reinicia o estado e animação do botão.
    /// </summary>
    /// <param name="btnAnimator">Botão a ser reiniciado.</param>
    public void ResetButton()
    {
        if (Button == null)
            return;

        if (animator == null)
            return;

        if (!IsClickable)
            return;

        animator.ResetTrigger("Pressed");
        animator.ResetTrigger("Selected");
        animator.SetTrigger("Normal");
        animator.transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnDestroy()
    {
        var menuController = MenuController.Instance;
        if (menuController != null)
            menuController.SetCursor(Cursors.Arrow);

        if (Tooltip != null)
            Tooltip.SetVisible(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchId = eventData.pointerId;
        Pressed = true;
        IsPressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == touchId)
        {
            IsPressing = false;
            touchId = -1;
            Released = true;
        }
    }

    void UpdateClickPosition()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).fingerId == touchId)
            {
                ClickPosition = Input.GetTouch(i).position;
                return;
            }
        }

        ClickPosition = Input.mousePosition;
    }

    private void OnDisable()
    {
        if (IsHovering)
            MenuController.Instance.SetCursor(Cursors.Arrow);

        if (Tooltip != null)
            Tooltip.SetVisible(false);

        IsHovering = false;
    }
}
