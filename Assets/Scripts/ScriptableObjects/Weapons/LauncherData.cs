using UnityEngine;

[CreateAssetMenu(fileName = "New Launcher", menuName = "Neander Z/Weapons/Launcher Data", order = 6)]
public class LauncherData : BaseWeaponData
{
    public float ExplosionMaxDamageRadius;
    public float ExplosionMinDamageRadius;
    public float ExplosionSize = 1;
}
