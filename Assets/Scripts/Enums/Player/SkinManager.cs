using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public bool IsPreviewAnimation { get; set; }

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
    SpriteLibrary HeadColorSpriteLibrary, TorsoColorSpriteLibrary, LegsColorSpriteLibrary;

    [SerializeField]
    List<SkinItem> HatItems, HairItems, HeadItems, TorsoItems, ShirtItems, LegsItems, PantsItems, ShoesItems;

    SpriteRenderer HatSpriteRenderer, HairSpriteRenderer, HeadSpriteRenderer, TorsoSpriteRenderer, ShirtSpriteRenderer, LegsSpriteRenderer, PantsSpriteRenderer, ShoesSpriteRenderer, HeadColorSpriteRenderer, TorsoColorSpriteRenderer, LegColorSpriteRenderer;
    Image HatImage, HairImage, HeadImage, TorsoImage, ShirtImage, LegsImage, PantsImage, ShoesImage;


    void Start()
    {
        HatSpriteRenderer = HatSpriteLibrary.GetComponent<SpriteRenderer>();
        HairSpriteRenderer = HairSpriteLibrary.GetComponent<SpriteRenderer>();
        HeadSpriteRenderer = HeadSpriteLibrary.GetComponent<SpriteRenderer>();
        TorsoSpriteRenderer = TorsoSpriteLibrary.GetComponent<SpriteRenderer>();
        ShirtSpriteRenderer = ShirtSpriteLibrary.GetComponent<SpriteRenderer>();
        LegsSpriteRenderer = LegsSpriteLibrary.GetComponent<SpriteRenderer>();
        PantsSpriteRenderer = PantsSpriteLibrary.GetComponent<SpriteRenderer>();
        ShoesSpriteRenderer = ShoesSpriteLibrary.GetComponent<SpriteRenderer>();

        HeadColorSpriteLibrary = HeadSpriteLibrary.transform.Find("HeadColor").GetComponent<SpriteLibrary>();
        TorsoColorSpriteLibrary = TorsoSpriteLibrary.transform.Find("TorsoColor").GetComponent<SpriteLibrary>();
        LegsColorSpriteLibrary = LegsSpriteLibrary.transform.Find("LegsColor").GetComponent<SpriteLibrary>();

        HeadColorSpriteRenderer = HeadColorSpriteLibrary.GetComponent<SpriteRenderer>();
        TorsoColorSpriteRenderer = TorsoColorSpriteLibrary.GetComponent<SpriteRenderer>();
        LegColorSpriteRenderer = LegsColorSpriteLibrary.GetComponent<SpriteRenderer>();

        Animator = GetComponentInParent<Animator>();

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

    public void UpdateSkin(Color32? skinColor = null)
    {
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

        HatSpriteLibrary.spriteLibraryAsset = HatItems.FirstOrDefault(x => x.Type == SkinItemTypes.Hat && x.HatType == CurrentHat)?.Library;
        HairSpriteLibrary.spriteLibraryAsset = HairItems.FirstOrDefault(x => x.Type == SkinItemTypes.Hair && x.HairType == CurrentHair)?.Library;
        HeadSpriteLibrary.spriteLibraryAsset = HeadItems.Find(x => x.Type == SkinItemTypes.Head && x.HeadType == CurrentHead).Library;
        TorsoSpriteLibrary.spriteLibraryAsset = TorsoItems.Find(x => x.Type == SkinItemTypes.Torso && x.TorsoType == CurrentTorso).Library;
        ShirtSpriteLibrary.spriteLibraryAsset = ShirtItems.FirstOrDefault(x => x.Type == SkinItemTypes.Shirt && x.ShirtType == CurrentShirt)?.Library;
        LegsSpriteLibrary.spriteLibraryAsset = LegsItems.Find(x => x.Type == SkinItemTypes.Legs && x.LegsType == CurrentLegs).Library;
        PantsSpriteLibrary.spriteLibraryAsset = PantsItems.FirstOrDefault(x => x.Type == SkinItemTypes.Pants && x.PantsType == CurrentPants)?.Library;
        ShoesSpriteLibrary.spriteLibraryAsset = ShoesItems.FirstOrDefault(x => x.Type == SkinItemTypes.Shoes && x.ShoesType == CurrentShoes)?.Library;

        HeadColorSpriteLibrary.spriteLibraryAsset = HeadItems.Find(x => x.Type == SkinItemTypes.Head && x.HeadType == CurrentHead).SkinColorLibrary;
        TorsoColorSpriteLibrary.spriteLibraryAsset = TorsoItems.Find(x => x.Type == SkinItemTypes.Torso && x.TorsoType == CurrentTorso).SkinColorLibrary;
        LegsColorSpriteLibrary.spriteLibraryAsset = LegsItems.Find(x => x.Type == SkinItemTypes.Legs && x.LegsType == CurrentLegs).SkinColorLibrary;

        if (skinColor != null)
        {
            HeadColorSpriteRenderer.color = skinColor.Value;
            TorsoColorSpriteRenderer.color = skinColor.Value;
            LegColorSpriteRenderer.color = skinColor.Value;
        }
    }

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
    }
}
