using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StyleTab : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
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
                currentHatIndex = incrementIndex ? currentHatIndex + 1 : currentHatIndex - 1;
                SkinManager.CurrentHat = (SkinHatOptions)currentHatIndex;
                break;

            case SkinItemTypes.Hair:
                if (!Enum.IsDefined(typeof(SkinHairOptions), currentHairIndex + indexDelta))
                    return;
                currentHairIndex = incrementIndex ? currentHairIndex + 1 : currentHairIndex - 1;
                SkinManager.CurrentHair = (SkinHairOptions)currentHairIndex;
                break;

            case SkinItemTypes.Head:
                if (!Enum.IsDefined(typeof(SkinHeadOptions), currentHeadIndex + indexDelta))
                    return;
                currentHeadIndex = incrementIndex ? currentHeadIndex + 1 : currentHeadIndex - 1;
                SkinManager.CurrentHead = (SkinHeadOptions)currentHeadIndex;
                break;

            case SkinItemTypes.Shirt:
                if (!Enum.IsDefined(typeof(SkinShirtOptions), currentShirtIndex + indexDelta))
                    return;
                currentShirtIndex = incrementIndex ? currentShirtIndex + 1 : currentShirtIndex - 1;
                SkinManager.CurrentShirt = (SkinShirtOptions)currentShirtIndex;
                break;

            case SkinItemTypes.Torso:
                if (!Enum.IsDefined(typeof(SkinTorsoOptions), currentTorsoIndex + indexDelta))
                    return;
                currentTorsoIndex = incrementIndex ? currentTorsoIndex + 1 : currentTorsoIndex - 1;
                SkinManager.CurrentTorso = (SkinTorsoOptions)currentTorsoIndex;
                break;

            case SkinItemTypes.Pants:
                if (!Enum.IsDefined(typeof(SkinPantsOptions), currentPantsIndex + indexDelta))
                    return;
                currentPantsIndex = incrementIndex ? currentPantsIndex + 1 : currentPantsIndex - 1;
                SkinManager.CurrentPants = (SkinPantsOptions)currentPantsIndex;
                break;

            case SkinItemTypes.Legs:
                if (!Enum.IsDefined(typeof(SkinLegOptions), currentLegsIndex + indexDelta))
                    return;
                currentLegsIndex = incrementIndex ? currentLegsIndex + 1 : currentLegsIndex - 1;
                SkinManager.CurrentLegs = (SkinLegOptions)currentLegsIndex;
                break;

            case SkinItemTypes.Shoes:
                if (!Enum.IsDefined(typeof(SkinShoesOptions), currentShoesIndex + indexDelta))
                    return;
                currentShoesIndex = incrementIndex ? currentShoesIndex + 1 : currentShoesIndex - 1;
                SkinManager.CurrentShoes = (SkinShoesOptions)currentShoesIndex;
                break;
        }

        SkinManager.UpdateSkin();
    }

    public void SetAnimationPreview(int animationTypeIndex)
    {
        SkinManager.SetAnimation((AnimationTypes)animationTypeIndex);
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
        SkinManager.UpdateSkin(CurrentSkinColor);
    }
}
