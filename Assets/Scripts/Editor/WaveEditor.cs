using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(WaveData))]
public class WaveEditor : Editor
{
    WaveData data;
    SerializedObject GetTarget;
    SerializedProperty EnemyGroups;

    GUIStyle labelStyle;
    ReorderableList EnemyGroupsList;
    float[] enemyGroupLineHeights;

    private void OnEnable()
    {
        data = (WaveData)target;
        GetTarget = new SerializedObject(data);
        EnemyGroups = GetTarget.FindProperty("EnemyGroups");

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

        enemyGroupLineHeights = new float[EnemyGroups.arraySize];

        EnemyGroupsList = new ReorderableList(serializedObject, EnemyGroups, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderEnemyGroup(rect, index),
            headerHeight = 0,
            elementHeightCallback = (int index) => EnemyGroups.GetArrayElementAtIndex(index).isExpanded ? enemyGroupLineHeights[index] : EditorGUIUtility.singleLineHeight + 2,
        };
    }

    public override void OnInspectorGUI()
    {
        GetTarget.Update();

        if (EnemyGroups.arraySize > enemyGroupLineHeights.Length)
            System.Array.Resize(ref enemyGroupLineHeights, EnemyGroups.arraySize);

        void DrawProp(string prop1Name, string? prop2Name = null)
        {
            SerializedProperty prop = GetTarget.FindProperty(prop1Name);

            GUILayout.BeginHorizontal();

            GUILayout.Label(prop.displayName, GUILayout.Width(130));
            if (prop2Name != null)
                GUILayout.FlexibleSpace();
            EditorGUILayout.PropertyField(prop, GUIContent.none, true, GUILayout.MinWidth(100));

            if (prop2Name != null)
            {
                SerializedProperty prop2 = GetTarget.FindProperty(prop2Name);
                GUILayout.Label(prop2.displayName, GUILayout.Width(130));
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(prop2, GUIContent.none, true, GUILayout.MinWidth(100));
            }

            GUILayout.EndHorizontal();
        }

        DrawProp("Number");
        DrawProp("Title");
        DrawProp("Description");
        DrawProp("MoneyMultiplier");

        EditorGUILayout.Space();

        DrawProp("MinEnemiesAlive", "MaxEnemiesAlive");
        DrawProp("MinSpawnDelayMs", "MaxSpawnDelayMs");
        DrawProp("MinSpawnCount", "MaxSpawnCount");
        DrawProp("StartDelayMs", "EndDelayMs");

        EditorGUILayout.Space();

        EnemyGroups.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(EnemyGroups.isExpanded, $"Enemy Groups (total: {data.EnemyGroups.Sum(x => x.Count)})");
        if (EnemyGroups.isExpanded)
            EnemyGroupsList.DoLayoutList();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();

        SerializedProperty isBossProp = GetTarget.FindProperty("IsBossWave");
        GUILayout.Label(isBossProp.displayName, GUILayout.Width(80));
        EditorGUILayout.PropertyField(isBossProp, GUIContent.none, true, GUILayout.MinWidth(20), GUILayout.MaxWidth(40));

        if (isBossProp.boolValue)
        {
            SerializedProperty bossIndexProp = GetTarget.FindProperty("BossGroupIndex");
            GUILayout.Label("Boss", GUILayout.Width(40));
            string[] groupLabels = data.EnemyGroups.Select(x => $"{x.EnemyType} ({x.Count})").ToArray();
            bossIndexProp.intValue = EditorGUILayout.Popup(bossIndexProp.intValue, groupLabels, GUILayout.MaxWidth(130));
        }

        GUILayout.EndHorizontal();

        GetTarget.ApplyModifiedProperties();
    }

    void RenderEnemyGroup(Rect rect, int index)
    {
        var item = EnemyGroups.GetArrayElementAtIndex(index);
        EnemyTypes enemyType = ((EnemyTypes)item.FindPropertyRelative("EnemyType").enumValueIndex);

        item.isExpanded = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y, 100, EditorGUIUtility.singleLineHeight), item.isExpanded, $" {enemyType} ({item.FindPropertyRelative("Count").intValue})", true);

        if (!item.isExpanded)
            return;

        rect.y += 2;

        string lblType = "Type", lblCount = "Count", lblIsInfinite = "Infinite",
               lblMaxHealth = "Max Health", lblMinHealth = "Min Health",
               lblMaxSpeed = "Max Speed", lblMinSpeed = "Min Speed",
               lblMaxDamage = "Max Damage", lblMinDamage = "Min Damage",
               lblMaxKillScore = "Max KillScore", lblMinKillScore = "Min KillScore",
               lblSpawnChance = "Spawn Chance", lblIsDisabled = "Disabled",
               //Enemy-specific:
               lblRonaldSpawnChance = "Ronald Spawn Chance",
               lblRaimundoHelmetHealth = "Raimundo Helmet Health",
               lblRavenAttackChance = "Raven Attack Chance",
               lblRavenAttackAttemptDelayMs = "Raven Attack Attempt Delay Ms",
               lblRuteBurningEffectDurationMs = "Rute Burning Effect Duration Ms", lblRuteBurningEffectTickIntervalMs = "Rute Burning Effect TickInterval Ms",
               lblRuteSelfBurningEffectDurationMs = "Rute Self Burning Effect Duration Ms", lblRuteSelfBurningEffectTickIntervalMs = "Rute Self Burning Effect TickInterval Ms",
               lblRuteSelfDamage = "Rute Self Damage", lblRuteFloorFlameDamage = "Rute Floor Flame Damage"
               ;

        float typeWidth = 108, countWidth = 30, isInfiniteWidth = 100,
                maxHealthWidth = 50, minHealthWidth = 50,
                maxSpeedWidth = 50, minSpeedWidth = 50,
                maxDamageWidth = 50, minDamageWidth = 50,
                maxKillScoreWidth = 50, minKillScoreWidth = 50,
                spawnChanceWidth = 50, isDisabledWidth = 100,
                //Enemy-specific:
                ronaldSpawnChanceWidth = 50,
                raimundoHelmetHealthWidth = 50,
                ravenAttackChanceWidth = 50,
                ravenAttackAttemptDelayMsWidth = 50,
                ruteBurningEffectDurationMsWidth = 50, ruteBurningEffectTickIntervalMsWidth = 50,
                ruteSelfBurningEffectDurationMsWidth = 50, ruteSelfBurningEffectTickIntervalMsWidth = 50,
                ruteSelfDamageWidth = 50, ruteFloorFlameDamageWidth = 50
                ;

        float lblTypeWidth, lblCountWidth, lblIsInfiniteWidth, lblMaxHealthWidth, lblMinHealthWidth,
              lblMaxSpeedWidth, lblMinSpeedWidth, lblMaxDamageWidth, lblMinDamageWidth, lblMaxKillScoreWidth, lblMinKillScoreWidth, lblSpawnChanceWidth, lblIsDisabledWidth,
              //Enemy-specific:
              lblRonaldSpawnChanceWidth, lblRaimundoHelmetHealthWidth, lblRavenAttackChanceWidth, lblRavenAttackAttemptDelayMsWidth, 
              lblRuteBurningEffectDurationMsWidth, lblRuteBurningEffectTickIntervalMsWidth, 
              lblRuteSelfBurningEffectDurationMsWidth, lblRuteSelfBurningEffectTickIntervalMsWidth,
              lblRuteSelfDamageWidth, lblRuteFloorFlameDamageWidth
              ;

        lblTypeWidth = labelStyle.CalcSize(new GUIContent(lblType)).x;
        lblCountWidth = labelStyle.CalcSize(new GUIContent(lblCount)).x;
        lblIsInfiniteWidth = labelStyle.CalcSize(new GUIContent(lblIsInfinite)).x;
        lblMaxHealthWidth = labelStyle.CalcSize(new GUIContent(lblMaxHealth)).x;
        lblMinHealthWidth = labelStyle.CalcSize(new GUIContent(lblMinHealth)).x;
        lblMaxSpeedWidth = labelStyle.CalcSize(new GUIContent(lblMaxSpeed)).x;
        lblMinSpeedWidth = labelStyle.CalcSize(new GUIContent(lblMinSpeed)).x;
        lblMaxDamageWidth = labelStyle.CalcSize(new GUIContent(lblMaxDamage)).x;
        lblMinDamageWidth = labelStyle.CalcSize(new GUIContent(lblMinDamage)).x;
        lblMaxKillScoreWidth = labelStyle.CalcSize(new GUIContent(lblMaxKillScore)).x;
        lblMinKillScoreWidth = labelStyle.CalcSize(new GUIContent(lblMinKillScore)).x;
        lblSpawnChanceWidth = labelStyle.CalcSize(new GUIContent(lblSpawnChance)).x;
        lblIsDisabledWidth = labelStyle.CalcSize(new GUIContent(lblIsDisabled)).x;
        //Enemy-specific:
        lblRonaldSpawnChanceWidth = labelStyle.CalcSize(new GUIContent(lblRonaldSpawnChance)).x;
        lblRaimundoHelmetHealthWidth = labelStyle.CalcSize(new GUIContent(lblRaimundoHelmetHealth)).x;
        lblRavenAttackChanceWidth = labelStyle.CalcSize(new GUIContent(lblRavenAttackChance)).x;
        lblRavenAttackAttemptDelayMsWidth = labelStyle.CalcSize(new GUIContent(lblRavenAttackAttemptDelayMs)).x;
        lblRuteBurningEffectDurationMsWidth = labelStyle.CalcSize(new GUIContent(lblRuteBurningEffectDurationMs)).x;
        lblRuteBurningEffectTickIntervalMsWidth = labelStyle.CalcSize(new GUIContent(lblRuteBurningEffectTickIntervalMs)).x;
        lblRuteSelfBurningEffectDurationMsWidth = labelStyle.CalcSize(new GUIContent(lblRuteSelfBurningEffectDurationMs)).x;
        lblRuteSelfBurningEffectTickIntervalMsWidth = labelStyle.CalcSize(new GUIContent(lblRuteSelfBurningEffectTickIntervalMs)).x;
        lblRuteSelfDamageWidth = labelStyle.CalcSize(new GUIContent(lblRuteSelfDamage)).x;
        lblRuteFloorFlameDamageWidth = labelStyle.CalcSize(new GUIContent(lblRuteFloorFlameDamage)).x;

        float maxLabelWidth = Mathf.Max(lblTypeWidth, lblCountWidth, lblIsInfiniteWidth, lblMaxHealthWidth, lblMinHealthWidth,
                                                   lblMaxSpeedWidth, lblMinSpeedWidth, lblMaxDamageWidth, lblMinDamageWidth, lblMaxKillScoreWidth, lblMinKillScoreWidth, lblSpawnChanceWidth, lblIsDisabledWidth,
                                                   //Enemy-specific:
                                                   lblRonaldSpawnChanceWidth, lblRaimundoHelmetHealthWidth,
                                                   lblRavenAttackChanceWidth, lblRavenAttackAttemptDelayMsWidth, 
                                                   lblRuteBurningEffectDurationMsWidth, lblRuteBurningEffectTickIntervalMsWidth, 
                                                   lblRuteSelfBurningEffectDurationMsWidth, lblRuteSelfBurningEffectTickIntervalMsWidth,
                                                   lblRuteSelfDamageWidth, lblRuteFloorFlameDamageWidth
                                                   );

        float marginX = 10;
        float x = rect.x;
        float startX = x;
        float y = rect.y + EditorGUIUtility.singleLineHeight;

        // Type
        EditorGUI.LabelField(new Rect(x, y, lblTypeWidth, EditorGUIUtility.singleLineHeight), lblType, labelStyle);
        x += lblTypeWidth + marginX;
        EditorGUI.PropertyField(
            new Rect(x, y, typeWidth, EditorGUIUtility.singleLineHeight),
            item.FindPropertyRelative("EnemyType"), GUIContent.none);
        x += typeWidth + marginX * 2;

        // Count
        EditorGUI.LabelField(new Rect(x, y, lblCountWidth, EditorGUIUtility.singleLineHeight), lblCount, labelStyle);
        x += lblCountWidth + marginX;
        EditorGUI.PropertyField(
                       new Rect(x, y, countWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("Count"), GUIContent.none);
        x += countWidth + marginX * 2;

        // Is Infinite
        EditorGUI.LabelField(new Rect(x, y, lblIsInfiniteWidth, EditorGUIUtility.singleLineHeight), lblIsInfinite, labelStyle);
        x += lblIsInfiniteWidth + marginX;
        EditorGUI.PropertyField(
                       new Rect(x, y, isInfiniteWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("IsInfinite"), GUIContent.none);
        x += isInfiniteWidth + marginX * 2;

        y += EditorGUIUtility.singleLineHeight + 2;
        x = startX;

        // Min Health
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblMinHealth, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, minHealthWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("MinHealth"), GUIContent.none);
        x += minHealthWidth + marginX * 2;

        // Max Health
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblMaxHealth, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, maxHealthWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("MaxHealth"), GUIContent.none);
        x += maxHealthWidth + marginX * 2;

        y += EditorGUIUtility.singleLineHeight + 2;
        x = startX;

        // Min Speed
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblMinSpeed, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, minSpeedWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("MinSpeed"), GUIContent.none);
        x += minSpeedWidth + marginX * 2;

        // Max Speed
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblMaxSpeed, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, maxSpeedWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("MaxSpeed"), GUIContent.none);
        x += maxSpeedWidth + marginX * 2;

        y += EditorGUIUtility.singleLineHeight + 2;
        x = startX;

        // Min Damage
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblMinDamage, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, minDamageWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("MinDamage"), GUIContent.none);
        x += minDamageWidth + marginX * 2;

        // Max Damage
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblMaxDamage, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, maxDamageWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("MaxDamage"), GUIContent.none);
        x += maxDamageWidth + marginX * 2;

        y += EditorGUIUtility.singleLineHeight + 2;
        x = startX;

        // Min Score
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblMinKillScore, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, minKillScoreWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("MinKillScore"), GUIContent.none);
        x += minKillScoreWidth + marginX * 2;

        // Max Score
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblMaxKillScore, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, maxKillScoreWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("MaxKillScore"), GUIContent.none);
        x += maxKillScoreWidth + marginX * 2;

        y += EditorGUIUtility.singleLineHeight + 2;
        x = startX;

        // Spawn Chance
        EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblSpawnChance, labelStyle);
        x += maxLabelWidth + marginX;
        EditorGUI.PropertyField(
                        new Rect(x, y, spawnChanceWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("SpawnChanceMultiplier"), GUIContent.none);
        x += spawnChanceWidth + marginX * 2;

        // Is Disabled
        EditorGUI.LabelField(new Rect(x, y, lblIsDisabledWidth, EditorGUIUtility.singleLineHeight), lblIsDisabled, labelStyle);
        x += lblIsDisabledWidth + marginX;
        EditorGUI.PropertyField(
                       new Rect(x, y, isDisabledWidth, EditorGUIUtility.singleLineHeight),
                                  item.FindPropertyRelative("IsDisabled"), GUIContent.none);
        x = startX;

        y += EditorGUIUtility.singleLineHeight + 2;

        //Enemy-specific:

        if (enemyType == EnemyTypes.Z_Ronald)
        {
            // Ronald Spawn Chance
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRonaldSpawnChance, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                           new Rect(x, y, ronaldSpawnChanceWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RonaldSpawnChance"), GUIContent.none);

            x += ronaldSpawnChanceWidth + marginX * 2;
            y += EditorGUIUtility.singleLineHeight + 2;
        }

        if (enemyType == EnemyTypes.Z_Raimundo)
        {
            // Raimundo Helmet Health:
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRaimundoHelmetHealth, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                           new Rect(x, y, raimundoHelmetHealthWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RaimundoHelmetHealth"), GUIContent.none);

            x += raimundoHelmetHealthWidth + marginX * 2;
            y += EditorGUIUtility.singleLineHeight + 2;
        }

        if (enemyType == EnemyTypes.Z_Raven)
        {
            // Raven Attack Chance:
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRavenAttackChance, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                           new Rect(x, y, ravenAttackChanceWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RavenAttackChance"), GUIContent.none);

            x += ravenAttackChanceWidth + marginX * 2;

            // Raven Attack Attempt Delay Ms
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRavenAttackAttemptDelayMs, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                           new Rect(x, y, ravenAttackAttemptDelayMsWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RavenAttackAttemptDelayMs"), GUIContent.none);

            x += ravenAttackAttemptDelayMsWidth + marginX * 2;
            y += EditorGUIUtility.singleLineHeight + 2;

        }

        if (enemyType == EnemyTypes.Z_Rute)
        {
            //Rute Burning Effect Duration Ms
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRuteBurningEffectDurationMs, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                            new Rect(x, y, ruteBurningEffectDurationMsWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RuteBurningEffectDurationMs"), GUIContent.none);
            x += ruteBurningEffectDurationMsWidth + marginX * 2;

            //Rute Burning Effect TickInterval Ms
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRuteBurningEffectTickIntervalMs, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                            new Rect(x, y, ruteBurningEffectTickIntervalMsWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RuteBurningEffectTickIntervalMs"), GUIContent.none);

            y += EditorGUIUtility.singleLineHeight + 2;
            x = startX;

            //Rute Self Burning Effect Duration Ms
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRuteSelfBurningEffectDurationMs, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                            new Rect(x, y, ruteSelfBurningEffectDurationMsWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RuteSelfBurningEffectDurationMs"), GUIContent.none);
            x += ruteSelfBurningEffectDurationMsWidth + marginX * 2;

            //Rute Self Burning Effect TickInterval Ms
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRuteSelfBurningEffectTickIntervalMs, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                            new Rect(x, y, ruteSelfBurningEffectTickIntervalMsWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RuteSelfBurningEffectTickIntervalMs"), GUIContent.none);

            y += EditorGUIUtility.singleLineHeight + 2;
            x = startX;

            //Rute Self Damage
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRuteSelfDamage, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                            new Rect(x, y, ruteSelfDamageWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RuteSelfDamage"), GUIContent.none);
            x += ruteSelfDamageWidth + marginX * 2;

            //Rute Floor Flame Damage
            EditorGUI.LabelField(new Rect(x, y, maxLabelWidth, EditorGUIUtility.singleLineHeight), lblRuteFloorFlameDamage, labelStyle);
            x += maxLabelWidth + marginX;
            EditorGUI.PropertyField(
                            new Rect(x, y, ruteFloorFlameDamageWidth, EditorGUIUtility.singleLineHeight),
                                      item.FindPropertyRelative("RuteFloorFlameDamage"), GUIContent.none);

            x = startX;
            x = ruteFloorFlameDamageWidth + marginX * 2;
            y += EditorGUIUtility.singleLineHeight + 2;
        }

        enemyGroupLineHeights[index] = y - rect.y + EditorGUIUtility.singleLineHeight + 2;
    }
}
