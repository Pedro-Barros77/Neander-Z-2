using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class CustomDialog : MonoBehaviour
{
    public CustomDialogConfig Config { get; set; }

    [SerializeField]
    BaseButton BtnClose, BtnConfirm, BtnCancel;
    [SerializeField]
    InputField InputField;
    [SerializeField]
    Text InputFieldPlaceholder;
    [SerializeField]
    TextMeshProUGUI PromptText;
    Image BackdropImage;
    RectTransform DialogRect;

    bool initialized, isUiUpdatePending;
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Inicializa os valores iniciais do componente.
    /// </summary>
    void Initialize()
    {
        if (initialized || Config == null || (BtnClose.Button == null || BtnConfirm.Button == null))
            return;

        initialized = true;
        BackdropImage = GetComponent<Image>();
        DialogRect = transform.Find("Modal").GetComponent<RectTransform>();
        if (InputField != null)
            InputFieldPlaceholder = InputField.placeholder.GetComponent<Text>();

        BtnClose.Button.onClick.AddListener(() =>
        {
            Config.OnClose?.Invoke();
            if (Config.CloseOnClose)
                Close();
            if (Config.CancelOnClose)
                Config.OnCancel?.Invoke();
        });
        BtnCancel.Button.onClick.AddListener(() =>
        {
            Config.OnCancel?.Invoke();
            if (Config.CloseOnCancel)
                Close();
        });
        BtnConfirm.Button.onClick.AddListener(() =>
        {
            Config.OnConfirm?.Invoke(InputField.text);
            if (Config.CloseOnConfirm)
                Close();
        });
    }

    void Update()
    {
        if (!initialized)
            Initialize();

        if (initialized && Config != null && isUiUpdatePending)
            UpdateDialogUI();
    }

    /// <summary>
    /// Marca como pendente a atualização da UI.
    /// </summary>
    public void UpdateUI()
    {
        isUiUpdatePending = true;
    }

    /// <summary>
    /// Atualiza a UI do diálogo de acordo com as configurações.
    /// </summary>
    void UpdateDialogUI()
    {
        isUiUpdatePending = false;
        DialogRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Config.DialogSize.x);
        DialogRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Config.DialogSize.y);
        BackdropImage.enabled = Config.UseBackdrop;
        BtnClose.gameObject.SetActive(Config.UseCloseButton);
        BtnConfirm.gameObject.SetActive(Config.UseConfirmButton);
        BtnCancel.gameObject.SetActive(Config.UseCancelButton);
        InputField.gameObject.SetActive(Config.UseInputField);

        if (Config.BtnCloseTooltipText.Length > 0)
            BtnClose.SetToolTip(Config.BtnCloseTooltipText);
        if (Config.BtnConfirmTooltipText.Length > 0)
            BtnConfirm.SetToolTip(Config.BtnConfirmTooltipText);
        if (Config.BtnCancelTooltipText.Length > 0)
            BtnCancel.SetToolTip(Config.BtnCancelTooltipText);

        BtnConfirm.SetLabelText(Config.BtnConfirmText);
        BtnCancel.SetLabelText(Config.BtnCancelText);

        if (InputFieldPlaceholder != null)
            InputFieldPlaceholder.text = Config.InputFieldPlaceholderText;
        InputField.text = Config.InputFieldText;

        PromptText.text = Config.PromptText;
    }

    /// <summary>
    /// Fecha o diálogo.
    /// </summary>
    /// <param name="destroy">True para destruir a caixa de diálogo, false para apenas desativar.</param>
    public void Close(bool destroy = true)
    {
        if (destroy && gameObject != null)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Reabre o diálogo desativado.
    /// </summary>
    public void ReOpen()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Instancia um novo diálogo customizado e o abre.
    /// </summary>
    /// <param name="config">As configurações desse diálogo.</param>
    /// <param name="canvas">O canvas pai, onde o diálogo será instanciado.</param>
    public static void Open(CustomDialogConfig config, Canvas canvas = null)
    {
        if (canvas == null)
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        var dialogObj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/CustomDialog"), canvas.transform);
        var dialog = dialogObj.GetComponent<CustomDialog>();
        dialog.Config = config;
        dialog.Initialize();
        dialog.UpdateUI();
    }
}

public class CustomDialogConfig
{
    public bool UseBackdrop { get; set; } = true;
    public bool UseCloseButton { get; set; } = true;
    public bool UseConfirmButton { get; set; } = true;
    public bool UseCancelButton { get; set; } = true;
    public bool UseInputField { get; set; } = false;
    public string BtnConfirmText { get; set; } = "Confirm";
    public string BtnCancelText { get; set; } = "Cancel";
    public string PromptText { get; set; } = "Are you sure?";
    public string InputFieldPlaceholderText { get; set; } = "Enter text here";
    public string InputFieldText { get; set; } = "";
    public string BtnCloseTooltipText { get; set; } = "Close";
    public string BtnConfirmTooltipText { get; set; } = "Confirm";
    public string BtnCancelTooltipText { get; set; } = "Cancel";
    public bool CloseOnClose { get; set; } = true;
    public bool CloseOnCancel { get; set; } = true;
    public bool CloseOnConfirm { get; set; } = true;
    public bool CancelOnClose { get; set; } = true;
    public Vector2 DialogSize { get; set; } = new(300, 130);
    public Action<string> OnConfirm { get; set; }
    public Action OnCancel { get; set; }
    public Action OnClose { get; set; }
}
