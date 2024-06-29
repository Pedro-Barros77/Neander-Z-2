using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class StoreItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool IsSelected { get; private set; }
    public bool IsInventoryItem { get; set; }
    public Animator Animator { get; private set; }

    [SerializeField]
    public StoreItemData Data;
    [SerializeField]
    public TextMeshProUGUI TitleText, PriceText, CountText;
    [SerializeField]
    Image IconImage;
    [SerializeField]
    CustomAudio HoverSound, ClickSound;

    StoreScreen storeScreen;
    AudioSource audioSource;
    CanvasGroup canvasGroup;
    Canvas Canvas;
    GameObject DragClone;
    float startTitleFontSize = 18;
    bool IsInEditor => Application.isEditor && !Application.isPlaying;

    private void Awake()
    {
        OnUiUpdate();
    }

    void Start()
    {
        storeScreen = GameObject.Find("Screen").GetComponent<StoreScreen>();
        Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        Canvas = GetComponentInParent<Canvas>();
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
            Animator.SetTrigger("Selected");
            Animator.ResetTrigger("Normal");
        }

        if (Data == null)
            return;

        if (!IsInventoryItem)
            PriceText.text = $"$ {Data.Price - Data.Discount:N2}";

        if (storeScreen.PlayerData != null)
        {
            Data.CanAfford = storeScreen.PlayerData.Money >= Data.Price - Data.Discount;
            if (!IsInventoryItem)
                PriceText.color = Data.CanAfford ? Constants.Colors.GreenMoney : Constants.Colors.RedMoney;

            if (Data is StoreAmmoData)
                UpdateAmmo();

            if (Data is StoreWeaponData)
                UpdateWeapon();

            if (Data is StoreThrowableData)
                UpdateThrowable();

            if (Data is StoreTacticalAbilityData)
                UpdateTacticalAbility();

            if (Data is StorePassiveSkillData)
                UpdatePassiveSkill();

            if (Data is StoreSupportEquipmentData)
                UpdateSupportEquipment();

            UpdateSpecials();
        }
    }

    /// <summary>
    /// Function called everytime the UI needs to be updated.
    /// </summary>
    public void OnUiUpdate()
    {
        if (Data == null)
        {
#if UNITY_EDITOR
            if (PrefabUtility.HasPrefabInstanceAnyOverrides(gameObject, false))
                PrefabUtility.RevertPrefabInstance(gameObject, InteractionMode.AutomatedAction);
#endif

            gameObject.name = "StoreItem";
            return;
        }

        TitleText.text = Data.Title;
        TitleText.fontSize = startTitleFontSize * Data.IconTitleFontSizeMultiplier;
        PriceText.text = $"$ {Data.Price - Data.Discount:N2}";
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
        {
            storeScreen.SelectItem(this);
            ClickSound.PlayIfNotNull(audioSource, AudioTypes.UI);
        }
    }

    /// <summary>
    /// Function called when the mouse enters the item.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInEditor)
            return;

        //if (!button.interactable)
        //    return;

        HoverSound.PlayIfNotNull(audioSource, AudioTypes.UI);

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
        Animator.ResetTrigger("Selected");
        Animator.SetTrigger("Normal");
    }

    private void UpdateWeapon()
    {
        var data = Data as StoreWeaponData;

        if (IsInventoryItem)
        {
            var weapon = storeScreen.PlayerData.InventoryData.WeaponsSelection
                 .FirstOrDefault(x => x.Type == data.WeaponData.Type);
            PriceText.text = weapon.EquippedSlot switch
            {
                WeaponEquippedSlot.Primary => "Primary",
                WeaponEquippedSlot.Secondary => "Secondary",
                _ => ""
            };

            CountText.text = $"{storeScreen.PlayerData.InventoryData.GetAmmo(data.WeaponData.BulletType)}/{storeScreen.PlayerData.InventoryData.GetMaxAmmo(data.WeaponData.BulletType)}";
        }
        else
        {
            if (data.Purchased)
            {
                PriceText.text = "Purchased";
                PriceText.color = Constants.Colors.GreenMoney;
            }
        }

        Data.Purchased = storeScreen.PlayerData.InventoryData.HasWeapon(data.WeaponData.Type);
    }

    private void UpdateThrowable()
    {
        var data = Data as StoreThrowableData;
        var throwable = storeScreen.PlayerData.InventoryData.ThrowableItemsSelection
            .FirstOrDefault(x => x.Type == data.ThrowableData.Type);

        int diff = 0;

        if (throwable == null)
            diff = data.ThrowableData.MaxCount;
        else
            diff = throwable.MaxCount - throwable.Count;

        if (Constants.GetAction(InputActions.BuyMaxStoreItems))
            data.Discount = -(Data.Price * ((diff - Data.Amount) / Data.Amount));
        else
            data.Discount = 0;

        if (throwable == null)
            return;

        Data.MaxedUp = diff <= 0;


        if (IsInventoryItem)
        {
            PriceText.text = throwable.IsEquipped ? "Equipped" : "";
            CountText.text = $"{throwable.Count}/{throwable.MaxCount}";
        }
    }

    private void UpdateTacticalAbility()
    {
        var data = Data as StoreTacticalAbilityData;

        if (IsInventoryItem)
        {
            var tacticalAbility = storeScreen.PlayerData.InventoryData.TacticalAbilitiesSelection
                .FirstOrDefault(x => x.Type == data.AbilityType);
            PriceText.text = tacticalAbility.IsEquipped ? "Equipped" : "";
        }
        else
        {
            if (data.Purchased)
            {
                PriceText.text = "Purchased";
                PriceText.color = Constants.Colors.GreenMoney;
                return;
            }
        }

        data.Purchased = storeScreen.PlayerData.InventoryData.TacticalAbilitiesSelection.Any(x => x.Type == data.AbilityType);
    }

    private void UpdatePassiveSkill()
    {
        var data = Data as StorePassiveSkillData;

        if (IsInventoryItem)
        {
            var passiveSkill = storeScreen.PlayerData.InventoryData.PassiveSkillsSelection
                .FirstOrDefault(x => x.Type == data.SkillType);
            PriceText.text = passiveSkill.IsEquipped ? "Equipped" : "";
        }
        else
        {
            if (data.Purchased)
            {
                PriceText.text = "Purchased";
                PriceText.color = Constants.Colors.GreenMoney;
                return;
            }
        }

        data.Purchased = storeScreen.PlayerData.InventoryData.PassiveSkillsSelection.Any(x => x.Type == data.SkillType);
    }

    private void UpdateAmmo()
    {
        var data = Data as StoreAmmoData;

        int currentAmmo = storeScreen.PlayerData.InventoryData.GetAmmo(data.BulletType);
        int maxAmmo = storeScreen.PlayerData.InventoryData.GetMaxAmmo(data.BulletType);
        int diff = maxAmmo - currentAmmo;

        Data.MaxedUp = diff <= 0;

        if (Data.MaxedUp)
            return;

        if (Constants.GetAction(InputActions.BuyMaxStoreItems))
        {
            data.Discount = -(Data.Price * ((diff - Data.Amount) / Data.Amount));
            return;
        }

        if (currentAmmo + Data.Amount <= maxAmmo)
        {
            Data.Discount = 0;
            return;
        }

        float percentage = (diff * 100 / Data.Amount) / 100;

        Data.Discount = Data.Price * (1 - percentage);
    }

    private void UpdateSupportEquipment()
    {
        var data = Data as StoreSupportEquipmentData;
        var support = storeScreen.PlayerData.InventoryData.SupportEquipmentsSelection
            .FirstOrDefault(x => x.Type == data.SupportData.Type);

        int diff = 0;

        if (support == null)
            diff = data.SupportData.MaxCount;
        else
            diff = support.MaxCount - support.Count;

        if (Constants.GetAction(InputActions.BuyMaxStoreItems))
            data.Discount = -(Data.Price * ((diff - Data.Amount) / Data.Amount));
        else
            data.Discount = 0;

        if (support == null)
            return;

        Data.MaxedUp = diff <= 0;


        if (IsInventoryItem)
        {
            PriceText.text = support.IsEquipped ? "Equipped" : "";
            CountText.text = $"{support.Count}/{support.MaxCount}";
        }
    }

    private void UpdateSpecials()
    {
        switch (Data.name)
        {
            case "FirstAidKit":
            case "MedKit":
                Data.MaxedUp = storeScreen.PlayerData.Health >= storeScreen.PlayerData.MaxHealth;
                break;
            case "Locked":
                PriceText.text = "";
                break;
        }
    }

    /// <summary>
    /// Função chamada pelo evento do Unity quando o item começa a ser arrastado.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsInventoryItem)
            return;

        DragClone = Instantiate(gameObject, Canvas.transform);
        DragClone.GetComponent<StoreItem>().IsInventoryItem = true;

        canvasGroup.alpha = 0.3f;

        Image iconImage = DragClone.transform.Find("Icon").GetComponent<Image>();
        Image containerImage = DragClone.GetComponent<Image>();
        Image labelContainerImage = DragClone.transform.Find("LabelContainer").GetComponent<Image>();
        iconImage.raycastTarget = false;
        containerImage.raycastTarget = false;
        labelContainerImage.raycastTarget = false;

        storeScreen.OnStartDrag?.Invoke(this);
    }

    /// <summary>
    /// Função chamada pelo evento do Unity quando o item está sendo arrastado.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!IsInventoryItem)
            return;

        DragClone.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).WithZ(0);
    }

    /// <summary>
    /// Função chamada pelo evento do Unity quando o item termina de ser arrastado.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInventoryItem)
            return;

        Destroy(DragClone);
        canvasGroup.alpha = 1f;

        storeScreen.OnStartDrag?.Invoke(null);
    }
}
