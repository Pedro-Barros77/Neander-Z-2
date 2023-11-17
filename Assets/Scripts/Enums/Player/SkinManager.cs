using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public SkinHatOptions Hat;
    public SkinHairOptions Hair;
    public SkinHeadOptions Head;
    public SkinTorsoOptions Torso;
    public SkinShirtOptions Shirt;
    public SkinLegOptions Legs;
    public SkinPantsOptions Pants;
    public SkinShoesOptions Shoes;

    [SerializeField]
    SpriteRenderer HatSpriteRenderer, HairSpriteRenderer, HeadSpriteRenderer, TorsoSpriteRenderer, ShirtSpriteRenderer, LegsSpriteRenderer, PantsSpriteRenderer, ShoesSpriteRenderer;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
