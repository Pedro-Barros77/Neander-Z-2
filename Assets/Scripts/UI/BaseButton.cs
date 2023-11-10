using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool Pressed { get; private set; }
    public bool IsPressing { get; private set; }
    public bool Released { get; private set; }
    public bool IsHovering { get; private set; }
    public Vector3 ClickPosition { get; private set; }
    [SerializeField]
    public CustomAudio ClickSound, HoverSound;
    [SerializeField]
    public delegate void OnHover(BaseButton button, bool hovered);
    public event OnHover HoverEvent;

    private Animator animator;
    public Button button;
    private AudioSource audioSource;
    private int touchId = -1;

    void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (IsPressing)
            UpdateClickPosition();
    }

    private void LateUpdate()
    {
        Pressed = false;
        Released = false;
    }

    /// <summary>
    /// Função chamada quando o botão é clicado.
    /// </summary>
    public void OnClick()
    {
        if (button != null && !button.interactable)
            return;

        ClickSound.PlayIfNotNull(audioSource, AudioTypes.UI);

        IsPressing = true;
        Pressed = true;
    }

    /// <summary>
    /// Função chamada quando o mouse passa por cima do botão.
    /// </summary>
    public void OnHoverIn()
    {
        if (button != null && !button.interactable)
            return;

        HoverSound.PlayIfNotNull(audioSource, AudioTypes.UI);

        IsHovering = true;
        HoverEvent?.Invoke(this, true);

        MenuController.Instance.SetCursor(Cursors.Pointer);
    }

    /// <summary>
    /// Função chamada quando o mouse sai de cima do botão.
    /// </summary>
    public void OnHoverOut()
    {
        if (button != null && !button.interactable)
            return;

        IsHovering = false;
        HoverEvent?.Invoke(this, false);

        MenuController.Instance.SetCursor(Cursors.Arrow);
    }

    /// <summary>
    /// Reinicia o estado e animação do botão.
    /// </summary>
    /// <param name="btnAnimator">Botão a ser reiniciado.</param>
    public void ResetButton()
    {
        if (button == null)
            return;

        if (animator == null)
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

        IsHovering = false;
    }
}
