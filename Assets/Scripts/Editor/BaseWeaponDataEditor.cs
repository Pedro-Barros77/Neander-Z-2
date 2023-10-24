using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BaseWeaponData), true)]
public class BaseWeaponDataEditor : Editor
{
    BaseWeaponData data;
    SerializedObject GetTarget;

    SerializedProperty Upgrades;
    ReorderableList UpgradesList;

    GUIStyle labelStyle;
    List<float> upgradesGroupLineHeights = new();

    private void OnEnable()
    {
        data = (BaseWeaponData)target;
        GetTarget = new SerializedObject(data);

        Upgrades = GetTarget.FindProperty("Upgrades");

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

        upgradesGroupLineHeights.AddRange(new float[Upgrades.arraySize]);

        UpgradesList = new(GetTarget, Upgrades, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderUpgradeGroup(rect, index, Upgrades),
            headerHeight = 0,
            elementHeightCallback = (int index) => Upgrades.GetArrayElementAtIndex(index).FindPropertyRelative("UpgradeSteps").isExpanded ? upgradesGroupLineHeights[index] : EditorGUIUtility.singleLineHeight + 2,
        };
    }

    public override void OnInspectorGUI()
    {
        GetTarget.Update();
        DrawPropertiesExcluding(GetTarget, "Upgrades");

        GUILayout.Space(10);

        Upgrades.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(Upgrades.isExpanded, $"{Upgrades.displayName} ({Upgrades.arraySize})");
        if (Upgrades.isExpanded)
            UpgradesList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        GetTarget.ApplyModifiedProperties();
    }

    void RenderUpgradeGroup(Rect rect, int index, SerializedProperty list)
    {
        var item = list.GetArrayElementAtIndex(index);
        var attribute = item.FindPropertyRelative("Attribute");
        var upgradeSteps = item.FindPropertyRelative("UpgradeSteps");

        var stepsList = new ReorderableList(GetTarget, upgradeSteps, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderUpgradeStep(rect, index, upgradeSteps),
            headerHeight = 0
        };

        rect.y += 2;

        float marginX = 20;
        float x = rect.x + 10;
        float y = rect.y;

        upgradeSteps.isExpanded = EditorGUI.Foldout(new Rect(x, y, 20, EditorGUIUtility.singleLineHeight), upgradeSteps.isExpanded, ((WeaponAttributes)attribute.enumValueIndex).ToString());// {index+1:D2}
        if (upgradeSteps.isExpanded)
        {
            y += EditorGUIUtility.singleLineHeight + 2;
            x += marginX / 2;

            // Attribute
            EditorGUI.LabelField(new Rect(x, y, 50, EditorGUIUtility.singleLineHeight), "Attribute:", labelStyle);
            x += 40 + marginX;
            EditorGUI.PropertyField(
                new Rect(x, y, 140, EditorGUIUtility.singleLineHeight),
                attribute, GUIContent.none);

            y += EditorGUIUtility.singleLineHeight + 2;
            x -= 40 + marginX;

            stepsList.DoList(new Rect(x, y, rect.width - 20, 200));
        }

        if(upgradesGroupLineHeights.Count < list.arraySize)
            upgradesGroupLineHeights.AddRange(new float[list.arraySize - upgradesGroupLineHeights.Count]);

        upgradesGroupLineHeights[index] = y - rect.y + stepsList.GetHeight() + 2;
    }

    void RenderUpgradeStep(Rect rect, int index, SerializedProperty list)
    {
        var item = list.GetArrayElementAtIndex(index);

        var price = item.FindPropertyRelative("Price");
        var value = item.FindPropertyRelative("Value");

        rect.y += 2;

        float marginX = 20;
        float x = rect.x;
        float y = rect.y;

        // Price
        EditorGUI.LabelField(new Rect(x, y, 30, EditorGUIUtility.singleLineHeight), "Price:", labelStyle);
        x += 20 + marginX;
        EditorGUI.PropertyField(
            new Rect(x, y, 80, EditorGUIUtility.singleLineHeight),
        price, GUIContent.none);

        x += 80 + marginX;

        // Value
        EditorGUI.LabelField(new Rect(x, y, 30, EditorGUIUtility.singleLineHeight), "Value:", labelStyle);
        x += 20 + marginX;
        EditorGUI.PropertyField(
            new Rect(x, y, 80, EditorGUIUtility.singleLineHeight),
            value, GUIContent.none);
    }
}
