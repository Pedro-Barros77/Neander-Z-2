using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Backpack Data", menuName = "Neander Z/Store/Backpack Data", order = 8)]
public class StoreBackpackData : StoreItemData
{
    public List<AmmoUpgrade> AmmoUpgrades;

    [Serializable]
    public class AmmoUpgrade
    {
        public float Price;
        public int PistolAmmo;
        public int ShotgunAmmo;
        public int RifleAmmo;
        public int SniperAmmo;
        public int RocketAmmo;
        public int FuelAmmo;
    }
}
