using UnityEngine;

[CreateAssetMenu(fileName = "New Support Equipment", menuName = "Neander Z/Equipments/Support Equipment", order = 1)]
public class BaseSupportEquipmentData : AutoRevertSO
{
    public SupportEquipmentTypes Type;
    public int MaxCount;
}
