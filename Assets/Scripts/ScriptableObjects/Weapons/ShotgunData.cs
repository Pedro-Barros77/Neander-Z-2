using UnityEngine;

[CreateAssetMenu(fileName = "New Shotgun", menuName = "Neander Z/Weapons/Shotgun Data", order = 4)]
public class ShotgunData : BaseWeaponData
{
    public int ShellPelletsCount;
    public float PelletsDispersion;
    public bool UseMagazine;
}
