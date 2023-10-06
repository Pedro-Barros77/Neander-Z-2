using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Neander Z/Store/Item Data", order = 1)]
public class StoreItemData : AutoRevertSO
{
    public Sprite Icon;
    public string Title;
    public string Description;
    public float Price;
    public float Discount;
    public float Amount;
    public float IconScale = 1f;
    public float PreviewIconScale = 1f;
    public List<StoreItemTags> Tags;

    public bool IsWeapon => this is StoreWeaponData;
    public bool IsAmmo => this is StoreAmmoData;
    public bool IsThrowable => this is StoreThrowableData;
    public bool IsTacticalAbility => this is StoreTacticalAbilityData;
    public bool CanAfford { get; set; }
    public bool MaxedUp { get; set; }
    public bool Purchased { get; set; }
}
