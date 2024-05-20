public class HealthSupplyPackage : BaseSupplyPackage
{
    protected override void OnInteract()
    {
        base.OnInteract();

        Player.GetHealth(Player.MaxHealth);
    }
}
