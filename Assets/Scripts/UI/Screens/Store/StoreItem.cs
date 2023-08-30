using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class StoreItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsSelected { get; private set; }

    [SerializeField]
    public StoreItemData Data;
    [SerializeField]
    TextMeshProUGUI TitleText, PriceText;
    [SerializeField]
    Image IconImage;

    StoreScreen storeScreen;
    Animator animator;
    bool IsInEditor => Application.isEditor && !Application.isPlaying;

    private void Awake()
    {
        OnUiUpdate();
    }

    void Start()
    {
        storeScreen = GameObject.Find("Screen").GetComponent<StoreScreen>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (IsInEditor)
        {
            OnUiUpdate();
            return;
        }

        if (IsSelected)
        {
            animator.SetTrigger("Selected");
            animator.ResetTrigger("Normal");
        }

        if (Data == null)
            return;

        if (storeScreen.Player != null)
        {
            Data.CanAfford = storeScreen.Player.Money >= Data.Price;
            PriceText.color = Data.CanAfford ? storeScreen.GreenMoney : storeScreen.RedMoney;
        }
    }

    /// <summary>
    /// Function called everytime the UI needs to be updated.
    /// </summary>
    public void OnUiUpdate()
    {
        if (Data == null)
        {
            if (PrefabUtility.HasPrefabInstanceAnyOverrides(gameObject, false))
                PrefabUtility.RevertPrefabInstance(gameObject, InteractionMode.AutomatedAction);
            gameObject.name = "StoreItem";
            return;
        }

        TitleText.text = Data.Title;
        PriceText.text = $"$ {Data.Price:N2}";
        IconImage.sprite = Data.Icon;
        IconImage.transform.localScale = Vector3.one * Data.IconScale;
        gameObject.name = Data.name;
    }

    /// <summary>
    /// Function called when the item is clicked.
    /// </summary>
    public void OnClick()
    {
        if (IsInEditor)
            return;

        if (!IsSelected)
            storeScreen.SelectItem(this);
    }

    /// <summary>
    /// Function called when the mouse enters the item.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInEditor)
            return;

        MenuController.Instance.SetCursor(Cursors.Pointer);
    }

    /// <summary>
    /// Function called when the mouse exits the item.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsInEditor)
            return;

        MenuController.Instance.SetCursor(Cursors.Arrow);
    }

    /// <summary>
    /// Selects this item in the store;
    /// </summary>
    public void Select()
    {
        IsSelected = true;
    }

    /// <summary>
    /// Deselects this item in the store;
    /// </summary>
    public void Deselect()
    {
        IsSelected = false;
        animator.ResetTrigger("Selected");
        animator.SetTrigger("Normal");
    }
}
