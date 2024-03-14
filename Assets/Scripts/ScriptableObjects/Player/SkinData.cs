using UnityEngine;

[CreateAssetMenu(fileName = "New Skin Data", menuName = "Neander Z/Player/Skin Data", order = 4)]
public class SkinData : AutoRevertSO
{
    public string CharacterName;
    public CharacterTypes Character;

    public Color32 SkinColor;
    public Color32 HairColor;
    public Color32 EyeColor;

    public SkinHatOptions Hat;
    public SkinHairOptions Hair;
    public SkinHeadOptions Head;
    public SkinShirtOptions Shirt;
    public SkinTorsoOptions Torso;
    public SkinPantsOptions Pants;
    public SkinLegOptions Legs;
    public SkinShoesOptions Shoes;
}
