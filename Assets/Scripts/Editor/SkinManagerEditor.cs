using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SkinManager))]
public class SkinManagerEditor : Editor
{
    SkinManager data;
    SerializedObject GetTarget;
    SerializedProperty HatItems, HairItems, HeadItems, TorsoItems, ShirtItems, LegsItems, PantsItems, ShoesItems;

    GUIStyle ammoLabelStyle, maxAmmoLabelStyle, labelStyle;
    ReorderableList HatItemsList, HairItemsList, HeadItemsList, TorsoItemsList, ShirtItemsList, LegsItemsList, PantsItemsList, ShoesItemsList;

    void OnEnable()
    {
        data = (SkinManager)target;
        GetTarget = new SerializedObject(data);
        HatItems = GetTarget.FindProperty("HatItems");
        HairItems = GetTarget.FindProperty("HairItems");
        HeadItems = GetTarget.FindProperty("HeadItems");
        TorsoItems = GetTarget.FindProperty("TorsoItems");
        ShirtItems = GetTarget.FindProperty("ShirtItems");
        LegsItems = GetTarget.FindProperty("LegsItems");
        PantsItems = GetTarget.FindProperty("PantsItems");
        ShoesItems = GetTarget.FindProperty("ShoesItems");


        labelStyle = new()
        {
            stretchWidth = false,
            alignment = TextAnchor.MiddleLeft,
            stretchHeight = true,
            normal = new GUIStyleState()
            {
                textColor = new Color32(185, 185, 185, 255)
            }
        };

        ammoLabelStyle = new(labelStyle)
        {
            fixedWidth = 100,
        };

        maxAmmoLabelStyle = new(ammoLabelStyle)
        {
            margin = new RectOffset(10, 0, 0, 0),
            fixedWidth = 30
        };

        ReorderableList SkinItemList(SerializedProperty prop, SkinItemTypes itemType) => new(serializedObject, prop, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderSkinItem(prop, rect, index, itemType),
            headerHeight = 0
        };

        HatItemsList = SkinItemList(HatItems, SkinItemTypes.Hat);
        HairItemsList = SkinItemList(HairItems, SkinItemTypes.Hair);
        HeadItemsList = SkinItemList(HeadItems, SkinItemTypes.Head);
        TorsoItemsList = SkinItemList(TorsoItems, SkinItemTypes.Torso);
        ShirtItemsList = SkinItemList(ShirtItems, SkinItemTypes.Shirt);
        LegsItemsList = SkinItemList(LegsItems, SkinItemTypes.Legs);
        PantsItemsList = SkinItemList(PantsItems, SkinItemTypes.Pants);
        ShoesItemsList = SkinItemList(ShoesItems, SkinItemTypes.Shoes);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        GetTarget.Update();

        if (GUILayout.Button("Update Skin"))
            data.UpdateSkin();

        EditorGUILayout.PropertyField(GetTarget.FindProperty("CurrentHat"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("CurrentHair"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("CurrentHead"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("CurrentTorso"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("CurrentShirt"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("CurrentLegs"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("CurrentPants"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("CurrentShoes"));

        EditorGUILayout.PropertyField(GetTarget.FindProperty("HatSpriteLibrary"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("HairSpriteLibrary"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("HeadSpriteLibrary"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("TorsoSpriteLibrary"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("ShirtSpriteLibrary"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("LegsSpriteLibrary"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("PantsSpriteLibrary"));
        EditorGUILayout.PropertyField(GetTarget.FindProperty("ShoesSpriteLibrary"));

        // Skin Hats
        HatItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(HatItems.isExpanded, "Hats");
        if (HatItems.isExpanded)
            HatItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Skin Hair
        HairItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(HairItems.isExpanded, "Hair");
        if (HairItems.isExpanded)
            HairItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Skin Head
        HeadItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(HeadItems.isExpanded, "Head");
        if (HeadItems.isExpanded)
            HeadItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Skin Torso
        TorsoItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(TorsoItems.isExpanded, "Torso");
        if (TorsoItems.isExpanded)
            TorsoItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Skin Shirt
        ShirtItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(ShirtItems.isExpanded, "Shirt");
        if (ShirtItems.isExpanded)
            ShirtItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Skin Legs
        LegsItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(LegsItems.isExpanded, "Legs");
        if (LegsItems.isExpanded)
            LegsItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Skin Pants
        PantsItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(PantsItems.isExpanded, "Pants");
        if (PantsItems.isExpanded)
            PantsItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Skin Shoes
        ShoesItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(ShoesItems.isExpanded, "Shoes");
        if (ShoesItems.isExpanded)
            ShoesItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        GetTarget.ApplyModifiedProperties();
    }

    /// <summary>
    /// Renderiza a linha de uma arma da lista.
    /// </summary>
    /// <param name="property">A propriedade da lista (primárias ou secundárias).</param>
    /// <param name="rect">Retângulo da lista, fornecido pelo callback.</param>
    /// <param name="index">Index desse item da lista, fornecido pelo callback.</param>
    /// <param name="isPrimary">Se esta arma é da lista de primárias ou secundárias.</param>
    void RenderSkinItem(SerializedProperty property, Rect rect, int index, SkinItemTypes itemType)
    {
        rect.y += 2;

        var skinItem = property.GetArrayElementAtIndex(index);

        var type = skinItem.FindPropertyRelative("Type");
        type.enumValueIndex = (int)itemType;
        var hatType = skinItem.FindPropertyRelative("HatType");
        var hairType = skinItem.FindPropertyRelative("HairType");
        var headType = skinItem.FindPropertyRelative("HeadType");
        var torsoType = skinItem.FindPropertyRelative("TorsoType");
        var shirtType = skinItem.FindPropertyRelative("ShirtType");
        var legsType = skinItem.FindPropertyRelative("LegsType");
        var pantsType = skinItem.FindPropertyRelative("PantsType");
        var shoesType = skinItem.FindPropertyRelative("ShoesType");
        var library = skinItem.FindPropertyRelative("Library");

        string hatLabel = "Hat",
            hairLabel = "Hair",
            headLabel = "Head",
            torsoLabel = "Torso",
            shirtLabel = "Shirt",
            legsLabel = "Legs",
            pantsLabel = "Pants",
            shoesLabel = "Shoes";

        float typeWidth = 120,
            lblHatWidth,
            lblHairWidth,
            lblHeadWidth,
            lblTorsoWidth,
            lblShirtWidth,
            lblLegsWidth,
            lblPantsWidth,
            lblShoesWidth;

        float marginX = 10;

        lblHatWidth = labelStyle.CalcSize(new GUIContent(hatLabel)).x;
        lblHairWidth = labelStyle.CalcSize(new GUIContent(hairLabel)).x;
        lblHeadWidth = labelStyle.CalcSize(new GUIContent(headLabel)).x;
        lblTorsoWidth = labelStyle.CalcSize(new GUIContent(torsoLabel)).x;
        lblShirtWidth = labelStyle.CalcSize(new GUIContent(shirtLabel)).x;
        lblLegsWidth = labelStyle.CalcSize(new GUIContent(legsLabel)).x;
        lblPantsWidth = labelStyle.CalcSize(new GUIContent(pantsLabel)).x;
        lblShoesWidth = labelStyle.CalcSize(new GUIContent(shoesLabel)).x;

        float x = rect.x;

        switch (itemType)
        {
            case SkinItemTypes.Hat:
                EditorGUI.LabelField(new Rect(x, rect.y, lblHatWidth, EditorGUIUtility.singleLineHeight), hatLabel, labelStyle);

                x += lblHatWidth + marginX;

                hatType.enumValueIndex = EditorGUI.Popup(
                    new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
                    hatType.enumValueIndex,
                    Enum.GetValues(typeof(SkinHatOptions)).Cast<SkinHatOptions>().Select(t => t.ToString()).ToArray());

                x += typeWidth + marginX;
                break;

            case SkinItemTypes.Hair:
                EditorGUI.LabelField(new Rect(x, rect.y, lblHairWidth, EditorGUIUtility.singleLineHeight), hairLabel, labelStyle);
                x += lblHairWidth + marginX;

                hairType.enumValueIndex = EditorGUI.Popup(
                    new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
                    hairType.enumValueIndex,
                    Enum.GetValues(typeof(SkinHairOptions)).Cast<SkinHairOptions>().Select(t => t.ToString()).ToArray());

                x += typeWidth + marginX;
                break;

            case SkinItemTypes.Head:
                EditorGUI.LabelField(new Rect(x, rect.y, lblHeadWidth, EditorGUIUtility.singleLineHeight), headLabel, labelStyle);
                x += lblHeadWidth + marginX;

                headType.enumValueIndex = EditorGUI.Popup(
                    new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
                    headType.enumValueIndex,
                    Enum.GetValues(typeof(SkinHeadOptions)).Cast<SkinHeadOptions>().Select(t => t.ToString()).ToArray());

                x += typeWidth + marginX;
                break;

            case SkinItemTypes.Torso:
                EditorGUI.LabelField(new Rect(x, rect.y, lblTorsoWidth, EditorGUIUtility.singleLineHeight), torsoLabel, labelStyle);
                x += lblTorsoWidth + marginX;

                torsoType.enumValueIndex = EditorGUI.Popup(
                    new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
                    torsoType.enumValueIndex,
                    Enum.GetValues(typeof(SkinTorsoOptions)).Cast<SkinTorsoOptions>().Select(t => t.ToString()).ToArray());

                x += typeWidth + marginX;
                break;

            case SkinItemTypes.Shirt:
                EditorGUI.LabelField(new Rect(x, rect.y, lblShirtWidth, EditorGUIUtility.singleLineHeight), shirtLabel, labelStyle);
                x += lblShirtWidth + marginX;

                shirtType.enumValueIndex = EditorGUI.Popup(
                    new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
                    shirtType.enumValueIndex,
                    Enum.GetValues(typeof(SkinShirtOptions)).Cast<SkinShirtOptions>().Select(t => t.ToString()).ToArray());

                x += typeWidth + marginX;
                break;

            case SkinItemTypes.Legs:
                EditorGUI.LabelField(new Rect(x, rect.y, lblLegsWidth, EditorGUIUtility.singleLineHeight), legsLabel, labelStyle);
                x += lblLegsWidth + marginX;

                legsType.enumValueIndex = EditorGUI.Popup(
                    new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
                    legsType.enumValueIndex,
                    Enum.GetValues(typeof(SkinLegOptions)).Cast<SkinLegOptions>().Select(t => t.ToString()).ToArray());

                x += typeWidth + marginX;
                break;

            case SkinItemTypes.Pants:
                EditorGUI.LabelField(new Rect(x, rect.y, lblPantsWidth, EditorGUIUtility.singleLineHeight), pantsLabel, labelStyle);
                x += lblPantsWidth + marginX;

                pantsType.enumValueIndex = EditorGUI.Popup(
                    new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
                    pantsType.enumValueIndex,
                    Enum.GetValues(typeof(SkinPantsOptions)).Cast<SkinPantsOptions>().Select(t => t.ToString()).ToArray());

                x += typeWidth + marginX;
                break;

            case SkinItemTypes.Shoes:
                EditorGUI.LabelField(new Rect(x, rect.y, lblShoesWidth, EditorGUIUtility.singleLineHeight), shoesLabel, labelStyle);
                x += lblShoesWidth + marginX;

                shoesType.enumValueIndex = EditorGUI.Popup(
                    new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
                    shoesType.enumValueIndex,
                    Enum.GetValues(typeof(SkinShoesOptions)).Cast<SkinShoesOptions>().Select(t => t.ToString()).ToArray());

                x += typeWidth + marginX;
                break;
        }

        EditorGUI.PropertyField(
            new Rect(x, rect.y, 150, EditorGUIUtility.singleLineHeight),
            library, GUIContent.none);
    }
}
