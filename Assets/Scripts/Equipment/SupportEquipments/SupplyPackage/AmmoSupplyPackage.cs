public class AmmoSupplyPackage : BaseSupplyPackage
{
    protected override void OnInteract()
    {
        base.OnInteract();

        if (Player == null)
            return;

        if (Player.Backpack.EquippedPrimaryWeapon != null)
        {
            BulletTypes primaryBulletType = Player.Backpack.EquippedPrimaryWeapon.BulletType;
            Player.Backpack.SetAmmo(primaryBulletType, Player.Backpack.GetMaxAmmo(primaryBulletType));
        }

        if (Player.Backpack.EquippedSecondaryWeapon != null)
        {
            BulletTypes secondaryBulletType = Player.Backpack.EquippedSecondaryWeapon.BulletType;
            Player.Backpack.SetAmmo(secondaryBulletType, Player.Backpack.GetMaxAmmo(secondaryBulletType));
        }

        if (Player.Backpack.EquippedThrowableType != ThrowableTypes.None && Player.Backpack.EquippedThrowable != null)
            Player.Backpack.EquippedThrowable.SetCount(Player.Backpack.EquippedThrowable.Count + 3);
        else
            Player.Data.InventoryData.ThrowableItemsSelection.Add(new(ThrowableTypes.FragGrenade, 3, 5, true));
    }
}
