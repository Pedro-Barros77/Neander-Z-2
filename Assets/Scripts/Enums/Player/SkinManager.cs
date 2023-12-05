using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SkinManager : MonoBehaviour
{
    public SkinHatOptions CurrentHat;
    public SkinHairOptions CurrentHair;
    public SkinHeadOptions CurrentHead;
    public SkinTorsoOptions CurrentTorso;
    public SkinShirtOptions CurrentShirt;
    public SkinLegOptions CurrentLegs;
    public SkinPantsOptions CurrentPants;
    public SkinShoesOptions CurrentShoes;

    [SerializeField]
    SpriteRenderer HatSpriteRenderer, HairSpriteRenderer, HeadSpriteRenderer, TorsoSpriteRenderer, ShirtSpriteRenderer, LegsSpriteRenderer, PantsSpriteRenderer, ShoesSpriteRenderer;

    [SerializeField]
    List<SkinItem> HatItems, HairItems, HeadItems, TorsoItems, ShirtItems, LegsItems, PantsItems, ShoesItems;
    void Start()
    {
        
    }

    void Update()
    {
        
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
    }
}
