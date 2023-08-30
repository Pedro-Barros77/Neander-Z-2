using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Neander Z/Store/Item Data", order = 1)]
public class StoreItemData : ScriptableObject
{
    public Sprite Icon;
    public string Title;
    public string Description;
    public float Price;
    public float Ammount;
    public float IconScale = 1f;
    public List<StoreItemTags> Tags;
    public bool CanAfford;

    public readonly bool IsWeapon;

    public StoreItemData(bool isWeapon = false)
    {
        IsWeapon = isWeapon;
    }
}
