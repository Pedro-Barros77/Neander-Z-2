using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public bool IsPreviewAnimation { get; set; }

    public Color32 CurrentSkinColor, CurrentHairColor, CurrentEyeColor;

    public SkinHatOptions CurrentHat;
    public SkinHairOptions CurrentHair;
    public SkinHeadOptions CurrentHead;
    public SkinTorsoOptions CurrentTorso;
    public SkinShirtOptions CurrentShirt;
    public SkinLegOptions CurrentLegs;
    public SkinPantsOptions CurrentPants;
    public SkinShoesOptions CurrentShoes;

    [SerializeField]
    SpriteLibrary HatSpriteLibrary, HairSpriteLibrary, HeadSpriteLibrary, TorsoSpriteLibrary, ShirtSpriteLibrary, LegsSpriteLibrary, PantsSpriteLibrary, ShoesSpriteLibrary;
    SpriteLibrary HeadColorSpriteLibrary, EyesColorSpriteLibrary, TorsoColorSpriteLibrary, LegsColorSpriteLibrary;

    [SerializeField]
    List<SkinItem> HatItems, HairItems, HeadItems, TorsoItems, ShirtItems, LegsItems, PantsItems, ShoesItems;

    SpriteRenderer HatSpriteRenderer, HairSpriteRenderer, HeadSpriteRenderer, TorsoSpriteRenderer, ShirtSpriteRenderer, LegsSpriteRenderer, PantsSpriteRenderer, ShoesSpriteRenderer, HeadColorSpriteRenderer, EyesColorSpriteRenderer, TorsoColorSpriteRenderer, LegColorSpriteRenderer, HandPalmSpriteRenderer, HandFingersSpriteRenderer;
    Image HatImage, HairImage, HeadImage, TorsoImage, ShirtImage, LegsImage, PantsImage, ShoesImage;

    Player Player;

    void Start()
    {
        Player = GetComponentInParent<Player>();

        if (Player != null)
            LoadSkinData(Player.Data.SkinData);

        HatSpriteRenderer = HatSpriteLibrary.GetComponent<SpriteRenderer>();
        HairSpriteRenderer = HairSpriteLibrary.GetComponent<SpriteRenderer>();
        HeadSpriteRenderer = HeadSpriteLibrary.GetComponent<SpriteRenderer>();
        TorsoSpriteRenderer = TorsoSpriteLibrary.GetComponent<SpriteRenderer>();
        ShirtSpriteRenderer = ShirtSpriteLibrary.GetComponent<SpriteRenderer>();
        LegsSpriteRenderer = LegsSpriteLibrary.GetComponent<SpriteRenderer>();
        PantsSpriteRenderer = PantsSpriteLibrary.GetComponent<SpriteRenderer>();
        ShoesSpriteRenderer = ShoesSpriteLibrary.GetComponent<SpriteRenderer>();

        HeadColorSpriteLibrary = HeadSpriteLibrary.transform.Find("HeadColor").GetComponent<SpriteLibrary>();
        EyesColorSpriteLibrary = HeadSpriteLibrary.transform.Find("EyesColor").GetComponent<SpriteLibrary>();
        TorsoColorSpriteLibrary = TorsoSpriteLibrary.transform.Find("TorsoColor").GetComponent<SpriteLibrary>();
        LegsColorSpriteLibrary = LegsSpriteLibrary.transform.Find("LegsColor").GetComponent<SpriteLibrary>();

        HeadColorSpriteRenderer = HeadColorSpriteLibrary.GetComponent<SpriteRenderer>();
        EyesColorSpriteRenderer = EyesColorSpriteLibrary.GetComponent<SpriteRenderer>();
        TorsoColorSpriteRenderer = TorsoColorSpriteLibrary.GetComponent<SpriteRenderer>();
        LegColorSpriteRenderer = LegsColorSpriteLibrary.GetComponent<SpriteRenderer>();

        Animator = GetComponentInParent<Animator>();

        var weaponContainer = Animator.transform.Find("WeaponContainer");
        if (weaponContainer != null)
        {
            var throwingContainer = weaponContainer.Find("Hand").Find("ThrowingContainer");
            HandPalmSpriteRenderer = throwingContainer.Find("Palm").GetComponent<SpriteRenderer>();
            HandFingersSpriteRenderer = throwingContainer.Find("Fingers").GetComponent<SpriteRenderer>();
        }

        HatImage = HatSpriteLibrary.GetComponent<Image>();
        HairImage = HairSpriteLibrary.GetComponent<Image>();
        HeadImage = HeadSpriteLibrary.GetComponent<Image>();
        TorsoImage = TorsoSpriteLibrary.GetComponent<Image>();
        ShirtImage = ShirtSpriteLibrary.GetComponent<Image>();
        LegsImage = LegsSpriteLibrary.GetComponent<Image>();
        PantsImage = PantsSpriteLibrary.GetComponent<Image>();
        ShoesImage = ShoesSpriteLibrary.GetComponent<Image>();

        UpdateSkin();
    }

    void Update()
    {
        if (HatImage != null) HatImage.sprite = HatSpriteRenderer.sprite;
        if (HairImage != null) HairImage.sprite = HairSpriteRenderer.sprite;
        if (HeadImage != null) HeadImage.sprite = HeadSpriteRenderer.sprite;
        if (TorsoImage != null) TorsoImage.sprite = TorsoSpriteRenderer.sprite;
        if (ShirtImage != null) ShirtImage.sprite = ShirtSpriteRenderer.sprite;
        if (LegsImage != null) LegsImage.sprite = LegsSpriteRenderer.sprite;
        if (PantsImage != null) PantsImage.sprite = PantsSpriteRenderer.sprite;
        if (ShoesImage != null) ShoesImage.sprite = ShoesSpriteRenderer.sprite;

        if (IsPreviewAnimation)
        {
            Animator.fireEvents = false;
            Animator.SetBool("LoopCurrentAnimation", true);
        }
    }

    /// <summary>
    /// Carrega os dados da skin definida.
    /// </summary>
    /// <param name="skinData">Os dados da skin a ser carregada.</param>
    public void LoadSkinData(SkinData skinData)
    {
        CurrentSkinColor = skinData.SkinColor;
        CurrentHairColor = skinData.HairColor;
        CurrentEyeColor = skinData.EyeColor;

        CurrentHat = skinData.Hat;
        CurrentHair = skinData.Hair;
        CurrentHead = skinData.Head;
        CurrentShirt = skinData.Shirt;
        CurrentTorso = skinData.Torso;
        CurrentPants = skinData.Pants;
        CurrentLegs = skinData.Legs;
        CurrentShoes = skinData.Shoes;
    }

    /// <summary>
    /// Atualiza a skin do jogador com base nos dados carregados.
    /// </summary>
    public void UpdateSkin()
    {
        if (HatSpriteRenderer == null)
            return;

        HatSpriteRenderer.enabled = CurrentHat != SkinHatOptions.None;
        HairSpriteRenderer.enabled = CurrentHair != SkinHairOptions.None;
        ShirtSpriteRenderer.enabled = CurrentShirt != SkinShirtOptions.None;
        PantsSpriteRenderer.enabled = CurrentPants != SkinPantsOptions.None;
        ShoesSpriteRenderer.enabled = CurrentShoes != SkinShoesOptions.None;

        if (HatImage != null) HatImage.enabled = CurrentHat != SkinHatOptions.None;
        if (HairImage != null) HairImage.enabled = CurrentHair != SkinHairOptions.None;
        if (ShirtImage != null) ShirtImage.enabled = CurrentShirt != SkinShirtOptions.None;
        if (PantsImage != null) PantsImage.enabled = CurrentPants != SkinPantsOptions.None;
        if (ShoesImage != null) ShoesImage.enabled = CurrentShoes != SkinShoesOptions.None;

        var headItem = HeadItems.Find(x => x.Type == SkinItemTypes.Head && x.HeadType == CurrentHead);
        var torsoItem = TorsoItems.Find(x => x.Type == SkinItemTypes.Torso && x.TorsoType == CurrentTorso);
        var legsItem = LegsItems.Find(x => x.Type == SkinItemTypes.Legs && x.LegsType == CurrentLegs);

        HatSpriteLibrary.spriteLibraryAsset = HatItems.FirstOrDefault(x => x.Type == SkinItemTypes.Hat && x.HatType == CurrentHat)?.Library;
        HairSpriteLibrary.spriteLibraryAsset = HairItems.FirstOrDefault(x => x.Type == SkinItemTypes.Hair && x.HairType == CurrentHair)?.Library;
        HeadSpriteLibrary.spriteLibraryAsset = headItem.Library;
        TorsoSpriteLibrary.spriteLibraryAsset = torsoItem.Library;
        ShirtSpriteLibrary.spriteLibraryAsset = ShirtItems.FirstOrDefault(x => x.Type == SkinItemTypes.Shirt && x.ShirtType == CurrentShirt)?.Library;
        LegsSpriteLibrary.spriteLibraryAsset = legsItem.Library;
        PantsSpriteLibrary.spriteLibraryAsset = PantsItems.FirstOrDefault(x => x.Type == SkinItemTypes.Pants && x.PantsType == CurrentPants)?.Library;
        ShoesSpriteLibrary.spriteLibraryAsset = ShoesItems.FirstOrDefault(x => x.Type == SkinItemTypes.Shoes && x.ShoesType == CurrentShoes)?.Library;

        HeadColorSpriteLibrary.spriteLibraryAsset = headItem.SkinColorLibrary;
        EyesColorSpriteLibrary.spriteLibraryAsset = headItem.EyesColorLibrary;
        TorsoColorSpriteLibrary.spriteLibraryAsset = torsoItem.SkinColorLibrary;
        LegsColorSpriteLibrary.spriteLibraryAsset = legsItem.SkinColorLibrary;

        UpdateSkinColor(CurrentSkinColor);
        UpdateHairColor(CurrentHairColor);
        UpdateEyesColor(CurrentEyeColor);
    }

    /// <summary>
    /// Atualiza a cor da pele do jogador.
    /// </summary>
    /// <param name="color">A cor a ser aplicada na pele.</param>
    public void UpdateSkinColor(Color32 color)
    {
        CurrentSkinColor = color;
        HeadColorSpriteRenderer.color = CurrentSkinColor;
        TorsoColorSpriteRenderer.color = CurrentSkinColor;
        LegColorSpriteRenderer.color = CurrentSkinColor;
        if (HandPalmSpriteRenderer != null)
        {
            HandPalmSpriteRenderer.color = CurrentSkinColor;
            HandFingersSpriteRenderer.color = CurrentSkinColor;
        }
    }

    /// <summary>
    /// Atualiza a cor do cabelo do jogador.
    /// </summary>
    /// <param name="color">A cor a ser aplicada no cabelo.</param>
    public void UpdateHairColor(Color32 color)
    {
        CurrentHairColor = color;
        HairSpriteRenderer.color = CurrentHairColor;
    }

    /// <summary>
    /// Atualiza a cor dos olhos do jogador.
    /// </summary>
    /// <param name="color">A cor a ser aplicada nos olhos.</param>
    public void UpdateEyesColor(Color32 color)
    {
        CurrentEyeColor = color;
        EyesColorSpriteRenderer.color = CurrentEyeColor;
    }

    /// <summary>
    /// Define a animação do jogador.
    /// </summary>
    /// <param name="animationType">O tipo de animação a ser definido.</param>
    public void SetAnimation(AnimationTypes animationType)
    {
        Animator.Play($"Carlos_{animationType}");
        Animator.SetBool("isCrouching", animationType == AnimationTypes.Crouch);
    }

    [Serializable]
    public class SkinItem
    {
        public SkinItemTypes Type;
        public SkinHatOptions HatType;
        public SkinHairOptions HairType;
        public SkinHeadOptions HeadType;
        public SkinTorsoOptions TorsoType;
        public SkinShirtOptions ShirtType;
        public SkinLegOptions LegsType;
        public SkinPantsOptions PantsType;
        public SkinShoesOptions ShoesType;
        public SpriteLibraryAsset Library;
        public SpriteLibraryAsset SkinColorLibrary;
        public SpriteLibraryAsset EyesColorLibrary;
    }
}
