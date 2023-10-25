using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(InventoryData))]
public class PlayerInventoryEditor : Editor
{
    InventoryData data;
    SerializedObject GetTarget;
    SerializedProperty PrimaryWeapons, SecondaryWeapons, ThrowableItems, TacticalAbilities;

    GUIStyle ammoLabelStyle, maxAmmoLabelStyle, weaponLabelStyle;
    ReorderableList PrimaryWeaponsList, SecondaryWeaponsList, ThrowableItemsList, TacticalAbilitiesList;

    void OnEnable()
    {
        data = (InventoryData)target;
        GetTarget = new SerializedObject(data);
        PrimaryWeapons = GetTarget.FindProperty("PrimaryWeaponsSelection");
        SecondaryWeapons = GetTarget.FindProperty("SecondaryWeaponsSelection");
        ThrowableItems = GetTarget.FindProperty("ThrowableItemsSelection");
        TacticalAbilities = GetTarget.FindProperty("TacticalAbilitiesSelection");

        weaponLabelStyle = new()
        {
            stretchWidth = false,
            alignment = TextAnchor.MiddleLeft,
            stretchHeight = true,
            normal = new GUIStyleState()
            {
                textColor = new Color32(185, 185, 185, 255)
            }
        };

        ammoLabelStyle = new(weaponLabelStyle)
        {
            fixedWidth = 100,
        };

        maxAmmoLabelStyle = new(ammoLabelStyle)
        {
            margin = new RectOffset(10, 0, 0, 0),
            fixedWidth = 30
        };

        PrimaryWeaponsList = new ReorderableList(serializedObject, PrimaryWeapons, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderWeapon(PrimaryWeapons, rect, index, true),
            headerHeight = 0
        };

        SecondaryWeaponsList = new ReorderableList(serializedObject, SecondaryWeapons, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderWeapon(SecondaryWeapons, rect, index, false),
            headerHeight = 0
        };

        ThrowableItemsList = new ReorderableList(serializedObject, ThrowableItems, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderThrowable(rect, index),
            headerHeight = 0
        };

        TacticalAbilitiesList = new ReorderableList(serializedObject, TacticalAbilities, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderTacticalAbility(rect, index),
            headerHeight = 0
        };
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GetTarget.Update();


        // Ammos
        void AddAmmo(string propName)
        {
            SerializedProperty prop = GetTarget.FindProperty(propName);
            SerializedProperty maxProp = GetTarget.FindProperty($"Max{propName}");

            GUILayout.BeginHorizontal();

            GUILayout.Label(prop.displayName, GUILayout.Width(ammoLabelStyle.fixedWidth));
            GUILayout.FlexibleSpace();
            EditorGUILayout.PropertyField(prop, GUIContent.none, true, GUILayout.MinWidth(100));

            GUILayout.Label("Max", GUILayout.Width(maxAmmoLabelStyle.fixedWidth));
            GUILayout.FlexibleSpace();
            EditorGUILayout.PropertyField(maxProp, GUIContent.none, true, GUILayout.MinWidth(100));

            GUILayout.EndHorizontal();

            if (prop.intValue > maxProp.intValue)
                prop.intValue = maxProp.intValue;
        }

        AddAmmo("PistolAmmo");
        AddAmmo("ShotgunAmmo");
        AddAmmo("RifleAmmo");
        AddAmmo("SniperAmmo");
        AddAmmo("RocketAmmo");

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        SerializedProperty upgradeIndex = GetTarget.FindProperty("UpgradeIndex");

        GUILayout.Label(upgradeIndex.displayName, GUILayout.Width(ammoLabelStyle.fixedWidth));
        EditorGUILayout.PropertyField(upgradeIndex, GUIContent.none, true, GUILayout.Width(50));
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Primary weapons
        PrimaryWeapons.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(PrimaryWeapons.isExpanded, "Primary Weapons");
        if (PrimaryWeapons.isExpanded)
            PrimaryWeaponsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Secondary weapons
        SecondaryWeapons.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(SecondaryWeapons.isExpanded, "Secondary Weapons");
        if (SecondaryWeapons.isExpanded)
            SecondaryWeaponsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Throwable items
        ThrowableItems.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(ThrowableItems.isExpanded, "Throwable Items");
        if (ThrowableItems.isExpanded)
            ThrowableItemsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Tactical Abilities
        TacticalAbilities.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(TacticalAbilities.isExpanded, "Tactical Abilities");
        if (TacticalAbilities.isExpanded)
            TacticalAbilitiesList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        GetTarget.ApplyModifiedProperties();
    }

    /// <summary>
    /// Renderiza a linha de uma arma da lista.
    /// </summary>
    /// <param name="property">A propriedade da lista (primárias ou secundárias).</param>
    /// <param name="rect">Retângulo da lista, fornecido pelo callback.</param>
    /// <param name="index">Index desse item da lista, fornecido pelo callback.</param>
    /// <param name="isPrimary">Se esta arma é da lista de primárias ou secundárias.</param>
    void RenderWeapon(SerializedProperty property, Rect rect, int index, bool isPrimary)
    {
        rect.y += 2;

        var weapon = property.GetArrayElementAtIndex(index);
        string typeLabel = "Type", slotLabel = "Slot";
        float marginX = 10;
        float typeWidth = 120, slotWidth = 100, lblTypeWidth, lblSlotWidth;

        lblTypeWidth = weaponLabelStyle.CalcSize(new GUIContent(typeLabel)).x;
        lblSlotWidth = weaponLabelStyle.CalcSize(new GUIContent(slotLabel)).x;

        var availableTypes = Enum.GetValues(typeof(WeaponTypes)).Cast<WeaponTypes>()
            .Where(t => Constants.IsPrimary(t) == isPrimary || t == WeaponTypes.None)
            .ToList();
        var availableSlots = Enum.GetValues(typeof(WeaponEquippedSlot)).Cast<WeaponEquippedSlot>().Where(t => !isPrimary || t != WeaponEquippedSlot.Secondary);

        float x = rect.x;

        EditorGUI.LabelField(new Rect(x, rect.y, lblTypeWidth, EditorGUIUtility.singleLineHeight), typeLabel, weaponLabelStyle);

        x += lblTypeWidth + marginX;

        int selectedTypeIndex = EditorGUI.Popup(
            new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
            availableTypes.IndexOf((WeaponTypes)weapon.FindPropertyRelative("Type").enumValueIndex),
                                             availableTypes.Select(t => t.ToString()).ToArray());

        weapon.FindPropertyRelative("Type").enumValueIndex = (int)availableTypes.ElementAt(selectedTypeIndex);

        x += typeWidth + marginX * 2;

        EditorGUI.LabelField(new Rect(x, rect.y, 0, EditorGUIUtility.singleLineHeight), slotLabel, weaponLabelStyle);

        x += lblSlotWidth + marginX;

        weapon.FindPropertyRelative("EquippedSlot").enumValueIndex = EditorGUI.Popup(
            new Rect(x, rect.y, slotWidth, EditorGUIUtility.singleLineHeight),
            availableSlots.ToList().IndexOf((WeaponEquippedSlot)weapon.FindPropertyRelative("EquippedSlot").enumValueIndex),
                                             availableSlots.Select(t => t.ToString()).ToArray());

        var newSlot = (WeaponEquippedSlot)weapon.FindPropertyRelative("EquippedSlot").enumValueIndex;


        void UnnequipAll(SerializedProperty prop, bool primaries)
        {
            for (int i = 0; i < prop.arraySize; i++)
            {
                var itemSlot = prop.GetArrayElementAtIndex(i).FindPropertyRelative("EquippedSlot");
                if (primaries && itemSlot.enumValueIndex == (int)WeaponEquippedSlot.Primary)
                    itemSlot.enumValueIndex = (int)WeaponEquippedSlot.None;
                else if (!primaries && itemSlot.enumValueIndex == (int)WeaponEquippedSlot.Secondary)
                    itemSlot.enumValueIndex = (int)WeaponEquippedSlot.None;
            }
        }

        if (newSlot != WeaponEquippedSlot.None)
        {
            if (newSlot == WeaponEquippedSlot.Primary)
            {
                UnnequipAll(PrimaryWeapons, true);
                UnnequipAll(SecondaryWeapons, true);
            }

            if (newSlot == WeaponEquippedSlot.Secondary)
                UnnequipAll(SecondaryWeapons, false);

            weapon.FindPropertyRelative("EquippedSlot").enumValueIndex = (int)newSlot;
        }
    }

    /// <summary>
    /// Renderiza a linha de um item da lista de itens arremessáveis.
    /// </summary>
    /// <param name="rect">Retângulo da lista, fornecido pelo callback.</param>
    /// <param name="index">Index desse item da lista, fornecido pelo callback.</param>
    void RenderThrowable(Rect rect, int index)
    {
        rect.y += 2;

        var item = ThrowableItems.GetArrayElementAtIndex(index);
        string typeLabel = "Type", countLabel = "Count", maxLabel = "Max", isEquippedLabel = "Equipped";
        float marginX = 10;
        float typeWidth = 120, countWidth = 30, maxWidth = 30, isEquippedWidth = 100, lblTypeWidth, lblCountWidth, lblMaxWidth, lblIsEquippedWidth;

        lblTypeWidth = weaponLabelStyle.CalcSize(new GUIContent(typeLabel)).x;
        lblCountWidth = weaponLabelStyle.CalcSize(new GUIContent(countLabel)).x;
        lblMaxWidth = weaponLabelStyle.CalcSize(new GUIContent(maxLabel)).x;
        lblIsEquippedWidth = weaponLabelStyle.CalcSize(new GUIContent(isEquippedLabel)).x;

        var countProp = item.FindPropertyRelative("Count");
        var maxProp = item.FindPropertyRelative("MaxCount");

        float x = rect.x;

        EditorGUI.LabelField(new Rect(x, rect.y, lblTypeWidth, EditorGUIUtility.singleLineHeight), typeLabel, weaponLabelStyle);

        x += lblTypeWidth + marginX;

        EditorGUI.PropertyField(
            new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
            item.FindPropertyRelative("Type"), GUIContent.none);

        x += typeWidth + marginX * 2;

        EditorGUI.LabelField(new Rect(x, rect.y, 0, EditorGUIUtility.singleLineHeight), countLabel, weaponLabelStyle);

        x += lblCountWidth + marginX;

        EditorGUI.PropertyField(
            new Rect(x, rect.y, countWidth, EditorGUIUtility.singleLineHeight),
            countProp, GUIContent.none);

        x += countWidth + marginX * 2;

        EditorGUI.LabelField(new Rect(x, rect.y, 0, EditorGUIUtility.singleLineHeight), maxLabel, weaponLabelStyle);

        x += lblMaxWidth + marginX;

        EditorGUI.PropertyField(
            new Rect(x, rect.y, maxWidth, EditorGUIUtility.singleLineHeight),
            maxProp, GUIContent.none);

        x += maxWidth + marginX * 2;

        EditorGUI.LabelField(new Rect(x, rect.y, 0, EditorGUIUtility.singleLineHeight), isEquippedLabel, weaponLabelStyle);

        x += lblIsEquippedWidth + marginX;

        item.FindPropertyRelative("IsEquipped").boolValue = EditorGUI.Toggle(
            new Rect(x, rect.y + 2, isEquippedWidth, EditorGUIUtility.singleLineHeight),
            item.FindPropertyRelative("IsEquipped").boolValue, EditorStyles.radioButton);

        if (item.FindPropertyRelative("IsEquipped").boolValue)
        {
            for (int i = 0; i < ThrowableItems.arraySize; i++)
            {
                ThrowableItems.GetArrayElementAtIndex(i).FindPropertyRelative("IsEquipped").boolValue = false;
            }
            item.FindPropertyRelative("IsEquipped").boolValue = true;
        }

        if (countProp.intValue > maxProp.intValue)
            countProp.intValue = maxProp.intValue;
    }

    /// <summary>
    /// Renderiza a linha de um item da lista de habilidades taticas.
    /// </summary>
    /// <param name="rect">Retângulo da lista, fornecido pelo callback.</param>
    /// <param name="index">Index desse item da lista, fornecido pelo callback.</param>
    void RenderTacticalAbility(Rect rect, int index)
    {
        rect.y += 2;

        var item = TacticalAbilities.GetArrayElementAtIndex(index);
        string typeLabel = "Type", isEquippedLabel = "Equipped";
        float marginX = 10;
        float typeWidth = 120, isEquippedWidth = 100, lblTypeWidth, lblIsEquippedWidth;

        lblTypeWidth = weaponLabelStyle.CalcSize(new GUIContent(typeLabel)).x;
        lblIsEquippedWidth = weaponLabelStyle.CalcSize(new GUIContent(isEquippedLabel)).x;

        float x = rect.x;

        EditorGUI.LabelField(new Rect(x, rect.y, lblTypeWidth, EditorGUIUtility.singleLineHeight), typeLabel, weaponLabelStyle);

        x += lblTypeWidth + marginX;

        EditorGUI.PropertyField(
            new Rect(x, rect.y, typeWidth, EditorGUIUtility.singleLineHeight),
            item.FindPropertyRelative("Type"), GUIContent.none);

        x += typeWidth + marginX * 2;

        EditorGUI.LabelField(new Rect(x, rect.y, 0, EditorGUIUtility.singleLineHeight), isEquippedLabel, weaponLabelStyle);

        x += lblIsEquippedWidth + marginX;

        item.FindPropertyRelative("IsEquipped").boolValue = EditorGUI.Toggle(
            new Rect(x, rect.y + 2, isEquippedWidth, EditorGUIUtility.singleLineHeight),
            item.FindPropertyRelative("IsEquipped").boolValue, EditorStyles.radioButton);

        if (item.FindPropertyRelative("IsEquipped").boolValue)
        {
            for (int i = 0; i < TacticalAbilities.arraySize; i++)
            {
                TacticalAbilities.GetArrayElementAtIndex(i).FindPropertyRelative("IsEquipped").boolValue = false;
            }
            item.FindPropertyRelative("IsEquipped").boolValue = true;
        }
    }
}
