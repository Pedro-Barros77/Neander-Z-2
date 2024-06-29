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
    public float InventorySlotIconScale = 1f;
    public bool IsSellable;
    public List<StoreItemTags> Tags;
    public float IconTitleFontSizeMultiplier = 1f;

    public bool CanAfford { get; set; }
    public bool MaxedUp { get; set; }
    public bool Purchased { get; set; }
}
