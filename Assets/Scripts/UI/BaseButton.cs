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

    public delegate void OnHover(BaseButton button, bool hovered);
    public event OnHover HoverEvent;

    private Animator animator;
    private AudioSource audioSource;
    private int touchId = -1;

    bool isInteractable;

    void Start()
    {
        animator = GetComponent<Animator>();
        Button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
        if (Tooltip != null)
        {
            Tooltip.HoverDelayMs = 1000f;
            if (TooltipText.IsNullOrEmpty())
            {
                var btnLabel = GetComponentInChildren<TextMeshProUGUI>();
                if (btnLabel != null)
                    Tooltip.SetText(btnLabel.text);
            }
            else
                Tooltip.SetText(TooltipText);
        }
        if (Button != null)
            isInteractable = Button.interactable;
    }

    void Update()
    {
        if (IsPressing)
            UpdateClickPosition();

        if (Button != null)
        {
            if (Button.interactable != isInteractable && Tooltip != null)
                HoverEvent?.Invoke(this, false);
            isInteractable = Button.interactable;
        }
    }

    private void LateUpdate()
    {
        Pressed = false;
        Released = false;
    }

    /// <summary>
    /// Fun��o chamada quando o bot�o � clicado.
    /// </summary>
    public void OnClick()
    {
        if (Button != null && !Button.interactable)
            return;

        if (!IsClickable)
            return;

        ClickSound.PlayIfNotNull(audioSource, AudioTypes.UI);

        IsPressing = true;
        Pressed = true;
    }

    /// <summary>
    /// Fun��o chamada quando o mouse passa por cima do bot�o.
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
    /// Fun��o chamada quando o mouse sai de cima do bot�o.
    /// </summary>
    public void OnHoverOut()
    {
        if (Button != null && !Button.interactable)
            return;

        IsHovering = false;
        HoverEvent?.Invoke(this, false);

        if (ChangesCursorOnHover)
            MenuController.Instance.SetCursor(Cursors.Arrow);
    }

    /// <summary>
    /// Reinicia o estado e anima��o do bot�o.
    /// </summary>
    /// <param name="btnAnimator">Bot�o a ser reiniciado.</param>
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
