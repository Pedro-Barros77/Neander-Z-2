using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    /// <summary>
    /// O Item que está no slot.
    /// </summary>
    public StoreItem Item { get; private set; }
    /// <summary>
    /// Os dados do item que está no slot.
    /// </summary>
    public StoreItemData Data => Item?.Data ?? null;

    [SerializeField]
    public InventorySlots SlotType;

    [SerializeField]
    Image IconImage, AmmoIconImage;
    [SerializeField]
    TextMeshProUGUI AmmoText, MaxAmmoText;
    [SerializeField]
    GameObject BtnClearSlot;

    StoreScreen storeScreen;
    InventoryTab inventoryTab;
    Vector3 startIconScale;
    CanvasGroup canvasGroup;

    InventoryData Inventory => storeScreen.PlayerData.InventoryData;

    private void Awake()
    {
        var screen = GameObject.Find("Screen");
        storeScreen = screen.GetComponent<StoreScreen>();
        inventoryTab = screen.GetComponent<InventoryTab>();
        startIconScale = IconImage.transform.localScale;
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        storeScreen.OnStartDrag += CheckDropHighlight;
    }

    void Update()
    {
        UpdateUI();
    }

    /// <summary>
    /// Função chamada pelo evento do Unity ao soltar um item arrastado em cima do slot.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        if (obj == null) return;

        StoreItem item = obj.GetComponent<StoreItem>();
        if (item == null) return;

        DropItem(item);
    }

    /// <summary>
    /// Limpa o slot.
    /// </summary>
    public void ClearSlot()
    {
        Item = null;
        IconImage.gameObject.SetActive(false);
        AmmoText.transform.parent.gameObject.SetActive(false);
        BtnClearSlot.SetActive(false);

        switch (SlotType)
        {
            case InventorySlots.Primary:
                var primWeapon = Inventory.PrimaryWeaponsSelection
                    .FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Primary);
                if (primWeapon != null)
                    primWeapon.EquippedSlot = WeaponEquippedSlot.None;

                break;
            case InventorySlots.Secondary:
                var secWeapon = Inventory.SecondaryWeaponsSelection
                    .FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Secondary);
                if (secWeapon != null)
                    secWeapon.EquippedSlot = WeaponEquippedSlot.None;

                break;
            case InventorySlots.Grenade:
                Inventory.ThrowableItemsSelection
                    .FirstOrDefault(x => x.IsEquipped)
                        .IsEquipped = false;
                break;
            case InventorySlots.TacticalAbility:
                Inventory.TacticalAbilitiesSelection
                    .FirstOrDefault(x => x.IsEquipped)
                        .IsEquipped = false;
                break;
        }
    }

    /// <summary>
    /// Aplica o item no slot.
    /// </summary>
    /// <param name="item">O item a ser aplicado no slot.</param>
    public void DropItem(StoreItem item)
    {
        if (!CanDrop(item)) return;

        IconImage.gameObject.SetActive(true);
        BtnClearSlot.SetActive(true);

        StoreItemData oldData = Data;

        Item = item;
        IconImage.sprite = Data.Icon;
        IconImage.transform.localScale = startIconScale * Data.IconScale;
        bool isWeapon = SlotType == InventorySlots.Primary || SlotType == InventorySlots.Secondary;
        bool isThrowable = SlotType == InventorySlots.Grenade;
        bool isTacticalAbility = SlotType == InventorySlots.TacticalAbility;

        if (isWeapon && Data is StoreWeaponData weaponData)
        {
            AmmoText.transform.parent.gameObject.SetActive(true);

            AmmoIconImage.sprite = weaponData.WeaponData.BulletType switch
            {
                BulletTypes.Pistol => storeScreen.PistolBulletIcon,
                BulletTypes.Shotgun => storeScreen.ShotgunBulletIcon,
                BulletTypes.AssaultRifle => storeScreen.RifleAmmoIcon,
                BulletTypes.Sniper => storeScreen.SniperAmmoIcon,
                BulletTypes.Rocket => storeScreen.RocketAmmoIcon,
                BulletTypes.Melee => storeScreen.MeleeAmmoIcon,
                _ => null,
            };

            if (oldData != null)
            {
                var oldWeapon = oldData as StoreWeaponData;

                Inventory.PrimaryWeaponsSelection
                    .Concat(Inventory.SecondaryWeaponsSelection)
                    .FirstOrDefault(x => x.Type == oldWeapon.WeaponData.Type)
                        .EquippedSlot = WeaponEquippedSlot.None;
            }

            // Prevents the same weapon from being in primary slot and secondary slot
            if (SlotType == InventorySlots.Primary && inventoryTab.SecondarySlot.Data is StoreWeaponData secData && secData.WeaponData.Type == weaponData.WeaponData.Type)
                inventoryTab.SecondarySlot.ClearSlot();
            else if (SlotType == InventorySlots.Secondary && inventoryTab.PrimarySlot.Data is StoreWeaponData primData && primData.WeaponData.Type == weaponData.WeaponData.Type)
                inventoryTab.PrimarySlot.ClearSlot();

            Inventory.PrimaryWeaponsSelection
                .Concat(Inventory.SecondaryWeaponsSelection)
                .FirstOrDefault(x => x.Type == weaponData.WeaponData.Type)
                    .EquippedSlot = SlotType switch
                    {
                        InventorySlots.Primary => WeaponEquippedSlot.Primary,
                        InventorySlots.Secondary => WeaponEquippedSlot.Secondary,
                        _ => WeaponEquippedSlot.None,
                    };
        }
        else if (isThrowable && Data is StoreThrowableData throwableData)
        {
            AmmoText.transform.parent.gameObject.SetActive(true);

            if (oldData != null)
            {
                var oldThrowable = oldData as StoreThrowableData;

                Inventory.ThrowableItemsSelection
                    .FirstOrDefault(x => x.Type == oldThrowable.ThrowableType)
                        .IsEquipped = false;
            }

            Inventory.ThrowableItemsSelection
                .FirstOrDefault(x => x.Type == throwableData.ThrowableType)
                    .IsEquipped = true;
        }
        else if (isTacticalAbility && Data is StoreTacticalAbilityData tacticalAbilityData)
        {
            if (oldData != null)
            {
                var oldTacticalAbility = oldData as StoreTacticalAbilityData;

                Inventory.TacticalAbilitiesSelection
                    .FirstOrDefault(x => x.Type == oldTacticalAbility.AbilityType)
                        .IsEquipped = false;
            }

            Inventory.TacticalAbilitiesSelection
                .FirstOrDefault(x => x.Type == tacticalAbilityData.AbilityType)
                    .IsEquipped = true;
        }

        inventoryTab.OnSlotChanged();
    }

    /// <summary>
    /// Atualiza a UI do slot.
    /// </summary>
    void UpdateUI()
    {
        if (Data == null)
            return;

        if (Data is StoreWeaponData weaponData)
        {
            AmmoText.color = Constants.GetAlertColor(Inventory.GetAmmo(weaponData.WeaponData.BulletType), Inventory.GetMaxAmmo(weaponData.WeaponData.BulletType), 0.2f);
            AmmoText.text = storeScreen.PlayerData.InventoryData.GetAmmo(weaponData.WeaponData.BulletType).ToString();
            MaxAmmoText.text = $"/{storeScreen.PlayerData.InventoryData.GetMaxAmmo(weaponData.WeaponData.BulletType)}";
        }
        else if (Data is StoreThrowableData throwableData)
        {
            var throwable = Inventory.ThrowableItemsSelection.FirstOrDefault(x => x.Type == throwableData.ThrowableType);

            AmmoText.color = Constants.GetAlertColor(throwable.Count, throwable.MaxCount, 0.2f);
            AmmoText.text = throwable.Count.ToString();
            MaxAmmoText.text = $"/{throwable.MaxCount}";
        }
    }

    /// <summary>
    /// Destaca/esconde os slots de acordo com o item sendo arrastado.
    /// </summary>
    /// <param name="item">O item que está sendo arrastado, ou null para resetar o destaque.</param>
    void CheckDropHighlight(StoreItem item)
    {
        if (item == null)
        {
            canvasGroup.alpha = 1f;
            return;
        }

        if (CanDrop(item))
        {
            //todo highlight effect
        }
        else
        {
            canvasGroup.alpha = 0.2f;
        }
    }

    /// <summary>
    /// Verifica se o item pode ser aplicado neste slot.
    /// </summary>
    /// <param name="item">O item sendo arrastado.</param>
    /// <returns>True se o item pode ser aplicado neste slot, senão, false.</returns>
    bool CanDrop(StoreItem item)
    {
        if (item == null)
            return false;

        bool canDrop = false;

        if (item.Data is StoreWeaponData weapondata && (SlotType == InventorySlots.Primary || (SlotType == InventorySlots.Secondary && !weapondata.WeaponData.IsPrimary)))
            canDrop = true;

        if (item.Data is StoreThrowableData && SlotType == InventorySlots.Grenade)
            canDrop = true;

        if (item.Data is StoreTacticalAbilityData && SlotType == InventorySlots.TacticalAbility)
            canDrop = true;

        // todo passive skill
        // if(item.Data is StorePassiveSkillData passiveSkillData && SlotType == InventorySlots.PassiveSkill)
        //     canDrop = true;

        // todo support
        // if(item.Data is StoreSupportData supportData && SlotType == InventorySlots.Support)
        //     canDrop = true;

        // todo deployable
        // if(item.Data is StoreDeployableData deployableData && SlotType == InventorySlots.Deployable)
        //     canDrop = true;

        return canDrop;
    }
}
