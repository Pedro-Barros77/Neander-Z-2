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

    /// <summary>
    /// Converte os dados da skin para um objeto salvável.
    /// </summary>
    /// <returns>O objeto preparado para ser salvo.</returns>
    public Data Encode() => new()
    {
        CharacterName = CharacterName,
        Character = Character,
        SkinColor = SkinColor.ToHex(),
        HairColor = HairColor.ToHex(),
        EyeColor = EyeColor.ToHex(),
        Hat = Hat,
        Hair = Hair,
        Head = Head,
        Shirt = Shirt,
        Torso = Torso,
        Pants = Pants,
        Legs = Legs,
        Shoes = Shoes
    };

    public class Data
    {
        public string CharacterName;
        public CharacterTypes Character;

        public string SkinColor;
        public string HairColor;
        public string EyeColor;

        public SkinHatOptions Hat;
        public SkinHairOptions Hair;
        public SkinHeadOptions Head;
        public SkinShirtOptions Shirt;
        public SkinTorsoOptions Torso;
        public SkinPantsOptions Pants;
        public SkinLegOptions Legs;
        public SkinShoesOptions Shoes;

        /// <summary>
        /// Converte um objeto salvo para dados da skin.
        /// </summary>
        /// <param name="data">A skin a receber os valores salvos.</param>
        public void Seed(SkinData data)
        {
            data.CharacterName = CharacterName;
            data.Character = Character;
            data.SkinColor = SkinColor.FromHexToColor();
            data.HairColor = HairColor.FromHexToColor();
            data.EyeColor = EyeColor.FromHexToColor();
            data.Hat = Hat;
            data.Hair = Hair;
            data.Head = Head;
            data.Shirt = Shirt;
            data.Torso = Torso;
            data.Pants = Pants;
            data.Legs = Legs;
            data.Shoes = Shoes;
        }
    }
}
