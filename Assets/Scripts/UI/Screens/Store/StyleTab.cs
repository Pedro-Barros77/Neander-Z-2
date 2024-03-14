using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StyleTab : MonoBehaviour
{
    StoreScreen storeScreen;

    public bool IsSkinDirty { get; private set; }

    [SerializeField]
    public SkinManager SkinManager;
    [SerializeField]
    TMP_Dropdown AnimationDropdown;
    [SerializeField]
    Slider SkinColorSlider;
    [SerializeField]
    Sprite SkinGradientSprite;

    [SerializeField]
    Button HatPrevBtn, HatNextBtn, HairPrevBtn, HairNextBtn, HeadPrevBtn, HeadNextBtn, TorsoPrevBtn, TorsoNextBtn, ShirtPrevBtn, ShirtNextBtn, LegsPrevBtn, LegsNextBtn, PantsPrevBtn, PantsNextBtn, ShoesPrevBtn, ShoesNextBtn;

    int currentHatIndex = 0, currentHairIndex = 0, currentHeadIndex = 0, currentTorsoIndex = 0, currentShirtIndex = 0, currentLegsIndex = 0, currentPantsIndex = 0, currentShoesIndex = 0;
    Color32 CurrentSkinColor;

    void Start()
    {
        storeScreen = GetComponent<StoreScreen>();

        SkinManager.LoadSkinData(storeScreen.PlayerData.SkinData);

        currentHatIndex = (int)SkinManager.CurrentHat;
        currentHairIndex = (int)SkinManager.CurrentHair;
        currentHeadIndex = (int)SkinManager.CurrentHead;
        currentTorsoIndex = (int)SkinManager.CurrentTorso;
        currentShirtIndex = (int)SkinManager.CurrentShirt;
        currentLegsIndex = (int)SkinManager.CurrentLegs;
        currentPantsIndex = (int)SkinManager.CurrentPants;
        currentShoesIndex = (int)SkinManager.CurrentShoes;

        AnimationDropdown.options.Clear();
        AnimationDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(AnimationTypes))));
        AnimationDropdown.value = (int)AnimationTypes.Idle;
        AnimationDropdown.onValueChanged.AddListener(SetAnimationPreview);

        GenerateSkinColorSliderGradient();
        SkinColorSlider.onValueChanged.AddListener(SetSkinColor);

        SkinManager.IsPreviewAnimation = true;
    }

    void Update()
    {
        HatPrevBtn.interactable = currentHatIndex > 0;
        HatNextBtn.interactable = currentHatIndex < Enum.GetNames(typeof(SkinHatOptions)).Length - 1;
        HairPrevBtn.interactable = currentHairIndex > 0;
        HairNextBtn.interactable = currentHairIndex < Enum.GetNames(typeof(SkinHairOptions)).Length - 1;
        HeadPrevBtn.interactable = currentHeadIndex > 1;
        HeadNextBtn.interactable = currentHeadIndex < Enum.GetNames(typeof(SkinHeadOptions)).Length - 1;
        TorsoPrevBtn.interactable = currentTorsoIndex > 1;
        TorsoNextBtn.interactable = currentTorsoIndex < Enum.GetNames(typeof(SkinTorsoOptions)).Length - 1;
        ShirtPrevBtn.interactable = currentShirtIndex > 0;
        ShirtNextBtn.interactable = currentShirtIndex < Enum.GetNames(typeof(SkinShirtOptions)).Length - 1;
        LegsPrevBtn.interactable = currentLegsIndex > 1;
        LegsNextBtn.interactable = currentLegsIndex < Enum.GetNames(typeof(SkinLegOptions)).Length - 1;
        PantsPrevBtn.interactable = currentPantsIndex > 0;
        PantsNextBtn.interactable = currentPantsIndex < Enum.GetNames(typeof(SkinPantsOptions)).Length - 1;
        ShoesPrevBtn.interactable = currentShoesIndex > 0;
        ShoesNextBtn.interactable = currentShoesIndex < Enum.GetNames(typeof(SkinShoesOptions)).Length - 1;
    }

    public void NextSkinItem(int itemTypeIndex) => ChangeSkinItem((SkinItemTypes)itemTypeIndex, true);
    public void PreviousSkinItem(int itemTypeIndex) => ChangeSkinItem((SkinItemTypes)itemTypeIndex, false);

    public void ChangeSkinItem(SkinItemTypes itemType, bool incrementIndex = true)
    {
        int indexDelta = incrementIndex ? 1 : -1;
        switch (itemType)
        {
            case SkinItemTypes.Hat:
                if (!Enum.IsDefined(typeof(SkinHatOptions), currentHatIndex + indexDelta))
                    return;
                currentHatIndex += indexDelta;
                SkinManager.CurrentHat = (SkinHatOptions)currentHatIndex;
                break;

            case SkinItemTypes.Hair:
                if (!Enum.IsDefined(typeof(SkinHairOptions), currentHairIndex + indexDelta))
                    return;
                currentHairIndex += indexDelta;
                SkinManager.CurrentHair = (SkinHairOptions)currentHairIndex;
                break;

            case SkinItemTypes.Head:
                if (!Enum.IsDefined(typeof(SkinHeadOptions), currentHeadIndex + indexDelta))
                    return;
                currentHeadIndex += indexDelta;
                SkinManager.CurrentHead = (SkinHeadOptions)currentHeadIndex;
                break;

            case SkinItemTypes.Shirt:
                if (!Enum.IsDefined(typeof(SkinShirtOptions), currentShirtIndex + indexDelta))
                    return;
                currentShirtIndex += indexDelta;
                SkinManager.CurrentShirt = (SkinShirtOptions)currentShirtIndex;
                break;

            case SkinItemTypes.Torso:
                if (!Enum.IsDefined(typeof(SkinTorsoOptions), currentTorsoIndex + indexDelta))
                    return;
                currentTorsoIndex += indexDelta;
                SkinManager.CurrentTorso = (SkinTorsoOptions)currentTorsoIndex;
                break;

            case SkinItemTypes.Pants:
                if (!Enum.IsDefined(typeof(SkinPantsOptions), currentPantsIndex + indexDelta))
                    return;
                currentPantsIndex += indexDelta;
                SkinManager.CurrentPants = (SkinPantsOptions)currentPantsIndex;
                break;

            case SkinItemTypes.Legs:
                if (!Enum.IsDefined(typeof(SkinLegOptions), currentLegsIndex + indexDelta))
                    return;
                currentLegsIndex += indexDelta;
                SkinManager.CurrentLegs = (SkinLegOptions)currentLegsIndex;
                break;

            case SkinItemTypes.Shoes:
                if (!Enum.IsDefined(typeof(SkinShoesOptions), currentShoesIndex + indexDelta))
                    return;
                currentShoesIndex += indexDelta;
                SkinManager.CurrentShoes = (SkinShoesOptions)currentShoesIndex;
                break;
        }

        SkinManager.UpdateSkin();
        IsSkinDirty = true;
    }

    public void SetAnimationPreview(int animationTypeIndex)
    {
        SkinManager.SetAnimation((AnimationTypes)animationTypeIndex);
    }

    public void SaveCurrentSkinData()
    {
        if (!IsSkinDirty)
            return;

        storeScreen.PlayerData.SkinData.SkinColor = CurrentSkinColor;
        //storeScreen.PlayerData.SkinData.HairColor = SkinManager.CurrentHairColor;
        //storeScreen.PlayerData.SkinData.EyeColor = SkinManager.CurrentEyeColor;

        storeScreen.PlayerData.SkinData.Hair = SkinManager.CurrentHair;
        storeScreen.PlayerData.SkinData.Hat = SkinManager.CurrentHat;
        storeScreen.PlayerData.SkinData.Head = SkinManager.CurrentHead;
        storeScreen.PlayerData.SkinData.Shirt = SkinManager.CurrentShirt;
        storeScreen.PlayerData.SkinData.Torso = SkinManager.CurrentTorso;
        storeScreen.PlayerData.SkinData.Pants = SkinManager.CurrentPants;
        storeScreen.PlayerData.SkinData.Legs = SkinManager.CurrentLegs;
        storeScreen.PlayerData.SkinData.Shoes = SkinManager.CurrentShoes;

        IsSkinDirty = false;
    }

    void GenerateSkinColorSliderGradient()
    {
        const int WIDTH = 256;
        Texture2D texture = new(WIDTH, 1);

        Color32[] gradientColors = Enumerable.Range(0, WIDTH)
           .Select(x => (Color32)Color.Lerp(Constants.Colors.SkinLightestColor, Constants.Colors.SkinDarkestColor, x / (float)WIDTH))
           .ToArray();

        texture.SetPixels32(gradientColors);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        Image imageComponent = SkinColorSlider.transform.Find("Background").GetComponent<Image>();
        Sprite gradientSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        imageComponent.sprite = gradientSprite;
    }

    void SetSkinColor(float value)
    {
        CurrentSkinColor = Color.Lerp(Constants.Colors.SkinLightestColor, Constants.Colors.SkinDarkestColor, value);
        SkinManager.CurrentSkinColor = CurrentSkinColor;
        SkinManager.UpdateSkinColor();
        IsSkinDirty = true;
    }
}
