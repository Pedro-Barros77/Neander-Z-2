using UnityEditor;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(KeybindConfig))]
public class KeybindConfigEditor : Editor
{
    KeybindConfig data;
    SerializedObject GetTarget;

    SerializedProperty MoveLeft;
    SerializedProperty MoveRight;
    SerializedProperty Jump;
    SerializedProperty Crouch;
    SerializedProperty Sprint;
    SerializedProperty Shoot;
    SerializedProperty ThrowGrenade;
    SerializedProperty TacticalAbility;
    SerializedProperty Reload;
    SerializedProperty PauseContinueGame;
    SerializedProperty SwitchWeapon;
    SerializedProperty EquipPrimaryWeapon;
    SerializedProperty EquipSecondaryWeapon;
    SerializedProperty BuyMaxStoreItems;
    SerializedProperty SelectSupportEquipment;
    SerializedProperty SelectDeployableEquipment;
    SerializedProperty Interact;
    ReorderableList MoveLeftList;
    ReorderableList MoveRightList;
    ReorderableList JumpList;
    ReorderableList CrouchList;
    ReorderableList SprintList;
    ReorderableList ShootList;
    ReorderableList ThrowGrenadeList;
    ReorderableList TacticalAbilityList;
    ReorderableList ReloadList;
    ReorderableList PauseContinueGameList;
    ReorderableList SwitchWeaponList;
    ReorderableList EquipPrimaryWeaponList;
    ReorderableList EquipSecondaryWeaponList;
    ReorderableList BuyMaxStoreItemsList;
    ReorderableList SelectSupportEquipmentList;
    ReorderableList SelectDeployableEquipmentList;
    ReorderableList InteractList;


    SerializedProperty DEBUG_IncreaseHealth;
    SerializedProperty DEBUG_DecreaseHealth;
    SerializedProperty DEBUG_IncreaseMoney;
    SerializedProperty DEBUG_DecreaseMoney;
    SerializedProperty DEBUG_SpawnRoger;
    SerializedProperty DEBUG_SpawnRonald;
    SerializedProperty DEBUG_SpawnRonaldo;
    SerializedProperty DEBUG_SpawnRaven;
    SerializedProperty DEBUG_SpawnRobert;
    SerializedProperty DEBUG_SpawnRaimundo;
    SerializedProperty DEBUG_SpawnRUI;
    SerializedProperty DEBUG_SpawnRute;
    SerializedProperty DEBUG_SpawnRat;
    SerializedProperty DEBUG_KillAllEnemiesAlive;
    SerializedProperty DEBUG_EndWave;
    SerializedProperty DEBUG_CenterEnemies;
    SerializedProperty DEBUG_RefillAllAmmo;
    ReorderableList DEBUG_IncreaseHealthList;
    ReorderableList DEBUG_DecreaseHealthList;
    ReorderableList DEBUG_IncreaseMoneyList;
    ReorderableList DEBUG_DecreaseMoneyList;
    ReorderableList DEBUG_SpawnRogerList;
    ReorderableList DEBUG_SpawnRonaldList;
    ReorderableList DEBUG_SpawnRonaldoList;
    ReorderableList DEBUG_SpawnRavenList;
    ReorderableList DEBUG_SpawnRobertList;
    ReorderableList DEBUG_SpawnRaimundoList;
    ReorderableList DEBUG_SpawnRUIList;
    ReorderableList DEBUG_SpawnRuteList;
    ReorderableList DEBUG_SpawnRatList;
    ReorderableList DEBUG_KillAllEnemiesAliveList;
    ReorderableList DEBUG_EndWaveList;
    ReorderableList DEBUG_CenterEnemiesList;
    ReorderableList DEBUG_RefillAllAmmoList;

    GUIStyle labelStyle;

    private void OnEnable()
    {
        data = (KeybindConfig)target;
        GetTarget = new SerializedObject(data);

        MoveLeft = GetTarget.FindProperty("MoveLeft");
        MoveRight = GetTarget.FindProperty("MoveRight");
        Jump = GetTarget.FindProperty("Jump");
        Crouch = GetTarget.FindProperty("Crouch");
        Sprint = GetTarget.FindProperty("Sprint");
        Shoot = GetTarget.FindProperty("Shoot");
        ThrowGrenade = GetTarget.FindProperty("ThrowGrenade");
        TacticalAbility = GetTarget.FindProperty("TacticalAbility");
        Reload = GetTarget.FindProperty("Reload");
        PauseContinueGame = GetTarget.FindProperty("PauseContinueGame");
        SwitchWeapon = GetTarget.FindProperty("SwitchWeapon");
        EquipPrimaryWeapon = GetTarget.FindProperty("EquipPrimaryWeapon");
        EquipSecondaryWeapon = GetTarget.FindProperty("EquipSecondaryWeapon");
        BuyMaxStoreItems = GetTarget.FindProperty("BuyMaxStoreItems");
        SelectSupportEquipment = GetTarget.FindProperty("SelectSupportEquipment");
        SelectDeployableEquipment = GetTarget.FindProperty("SelectDeployableEquipment");
        Interact = GetTarget.FindProperty("Interact");

        DEBUG_IncreaseHealth = GetTarget.FindProperty("DEBUG_IncreaseHealth");
        DEBUG_DecreaseHealth = GetTarget.FindProperty("DEBUG_DecreaseHealth");
        DEBUG_IncreaseMoney = GetTarget.FindProperty("DEBUG_IncreaseMoney");
        DEBUG_DecreaseMoney = GetTarget.FindProperty("DEBUG_DecreaseMoney");
        DEBUG_SpawnRoger = GetTarget.FindProperty("DEBUG_SpawnRoger");
        DEBUG_SpawnRonald = GetTarget.FindProperty("DEBUG_SpawnRonald");
        DEBUG_SpawnRonaldo = GetTarget.FindProperty("DEBUG_SpawnRonaldo");
        DEBUG_SpawnRaven = GetTarget.FindProperty("DEBUG_SpawnRaven");
        DEBUG_SpawnRobert = GetTarget.FindProperty("DEBUG_SpawnRobert");
        DEBUG_SpawnRaimundo = GetTarget.FindProperty("DEBUG_SpawnRaimundo");
        DEBUG_SpawnRUI = GetTarget.FindProperty("DEBUG_SpawnRUI");
        DEBUG_SpawnRute = GetTarget.FindProperty("DEBUG_SpawnRute");
        DEBUG_SpawnRat = GetTarget.FindProperty("DEBUG_SpawnRat");
        DEBUG_KillAllEnemiesAlive = GetTarget.FindProperty("DEBUG_KillAllEnemiesAlive");
        DEBUG_EndWave = GetTarget.FindProperty("DEBUG_EndWave");
        DEBUG_CenterEnemies = GetTarget.FindProperty("DEBUG_CenterEnemies");
        DEBUG_RefillAllAmmo = GetTarget.FindProperty("DEBUG_RefillAllAmmo");


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

        ReorderableList KeyItem(SerializedProperty list) => new(GetTarget, list, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => RenderKeybindItem(rect, index, list),
            headerHeight = 0
        };

        MoveLeftList = KeyItem(MoveLeft);
        MoveRightList = KeyItem(MoveRight);
        JumpList = KeyItem(Jump);
        CrouchList = KeyItem(Crouch);
        SprintList = KeyItem(Sprint);
        ShootList = KeyItem(Shoot);
        ThrowGrenadeList = KeyItem(ThrowGrenade);
        TacticalAbilityList = KeyItem(TacticalAbility);
        ReloadList = KeyItem(Reload);
        PauseContinueGameList = KeyItem(PauseContinueGame);
        SwitchWeaponList = KeyItem(SwitchWeapon);
        EquipPrimaryWeaponList = KeyItem(EquipPrimaryWeapon);
        EquipSecondaryWeaponList = KeyItem(EquipSecondaryWeapon);
        BuyMaxStoreItemsList = KeyItem(BuyMaxStoreItems);
        SelectSupportEquipmentList = KeyItem(SelectSupportEquipment);
        SelectDeployableEquipmentList = KeyItem(SelectDeployableEquipment);
        InteractList = KeyItem(Interact);

        DEBUG_IncreaseHealthList = KeyItem(DEBUG_IncreaseHealth);
        DEBUG_DecreaseHealthList = KeyItem(DEBUG_DecreaseHealth);
        DEBUG_IncreaseMoneyList = KeyItem(DEBUG_IncreaseMoney);
        DEBUG_DecreaseMoneyList = KeyItem(DEBUG_DecreaseMoney);
        DEBUG_SpawnRogerList = KeyItem(DEBUG_SpawnRoger);
        DEBUG_SpawnRonaldList = KeyItem(DEBUG_SpawnRonald);
        DEBUG_SpawnRonaldoList = KeyItem(DEBUG_SpawnRonaldo);
        DEBUG_SpawnRavenList = KeyItem(DEBUG_SpawnRaven);
        DEBUG_SpawnRobertList = KeyItem(DEBUG_SpawnRobert);
        DEBUG_SpawnRaimundoList = KeyItem(DEBUG_SpawnRaimundo);
        DEBUG_SpawnRUIList = KeyItem(DEBUG_SpawnRUI);
        DEBUG_SpawnRuteList = KeyItem(DEBUG_SpawnRute);
        DEBUG_SpawnRatList = KeyItem(DEBUG_SpawnRat);
        DEBUG_KillAllEnemiesAliveList = KeyItem(DEBUG_KillAllEnemiesAlive);
        DEBUG_EndWaveList = KeyItem(DEBUG_EndWave);
        DEBUG_CenterEnemiesList = KeyItem(DEBUG_CenterEnemies);
        DEBUG_RefillAllAmmoList = KeyItem(DEBUG_RefillAllAmmo);
    }

    public override void OnInspectorGUI()
    {
        GetTarget.Update();

        void FoldoutKeybind(SerializedProperty item, ReorderableList list)
        {
            item.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(item.isExpanded, $"{item.displayName} ({item.arraySize})");
            if (item.isExpanded)
                list.DoLayoutList();
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        FoldoutKeybind(MoveLeft, MoveLeftList);
        FoldoutKeybind(MoveRight, MoveRightList);
        FoldoutKeybind(Jump, JumpList);
        FoldoutKeybind(Crouch, CrouchList);
        FoldoutKeybind(Sprint, SprintList);
        FoldoutKeybind(Shoot, ShootList);
        FoldoutKeybind(ThrowGrenade, ThrowGrenadeList);
        FoldoutKeybind(TacticalAbility, TacticalAbilityList);
        FoldoutKeybind(Reload, ReloadList);
        FoldoutKeybind(PauseContinueGame, PauseContinueGameList);
        FoldoutKeybind(SwitchWeapon, SwitchWeaponList);
        FoldoutKeybind(EquipPrimaryWeapon, EquipPrimaryWeaponList);
        FoldoutKeybind(EquipSecondaryWeapon, EquipSecondaryWeaponList);
        FoldoutKeybind(BuyMaxStoreItems, BuyMaxStoreItemsList);
        FoldoutKeybind(SelectSupportEquipment, SelectSupportEquipmentList);
        FoldoutKeybind(SelectDeployableEquipment, SelectDeployableEquipmentList);
        FoldoutKeybind(Interact, InteractList);

        EditorGUILayout.Space(40);

        FoldoutKeybind(DEBUG_IncreaseHealth, DEBUG_IncreaseHealthList);
        FoldoutKeybind(DEBUG_DecreaseHealth, DEBUG_DecreaseHealthList);
        FoldoutKeybind(DEBUG_IncreaseMoney, DEBUG_IncreaseMoneyList);
        FoldoutKeybind(DEBUG_DecreaseMoney, DEBUG_DecreaseMoneyList);
        FoldoutKeybind(DEBUG_SpawnRoger, DEBUG_SpawnRogerList);
        FoldoutKeybind(DEBUG_SpawnRonald, DEBUG_SpawnRonaldList);
        FoldoutKeybind(DEBUG_SpawnRonaldo, DEBUG_SpawnRonaldoList);
        FoldoutKeybind(DEBUG_SpawnRaven, DEBUG_SpawnRavenList);
        FoldoutKeybind(DEBUG_SpawnRobert, DEBUG_SpawnRobertList);
        FoldoutKeybind(DEBUG_SpawnRaimundo, DEBUG_SpawnRaimundoList);
        FoldoutKeybind(DEBUG_SpawnRUI, DEBUG_SpawnRUIList);
        FoldoutKeybind(DEBUG_SpawnRute, DEBUG_SpawnRuteList);
        FoldoutKeybind(DEBUG_SpawnRat, DEBUG_SpawnRatList);
        FoldoutKeybind(DEBUG_KillAllEnemiesAlive, DEBUG_KillAllEnemiesAliveList);
        FoldoutKeybind(DEBUG_EndWave, DEBUG_EndWaveList);
        FoldoutKeybind(DEBUG_CenterEnemies, DEBUG_CenterEnemiesList);
        FoldoutKeybind(DEBUG_RefillAllAmmo, DEBUG_RefillAllAmmoList);

        GetTarget.ApplyModifiedProperties();
    }

    void RenderKeybindItem(Rect rect, int index, SerializedProperty list)
    {
        var item = list.GetArrayElementAtIndex(index);
        var action = item.FindPropertyRelative("Action");
        var key = item.FindPropertyRelative("Key");
        var isListening = item.FindPropertyRelative("IsListening");

        rect.y += 2;

        float marginX = 10;
        float x = rect.x;
        float y = rect.y;

        // Action
        EditorGUI.LabelField(new Rect(x, y, 40, EditorGUIUtility.singleLineHeight), "Action:", labelStyle);
        x += 40 + marginX;
        EditorGUI.PropertyField(
            new Rect(x, y, 150, EditorGUIUtility.singleLineHeight),
            action, GUIContent.none);

        x += 150 + marginX * 2;

        // Key
        EditorGUI.LabelField(new Rect(x, y, 20, EditorGUIUtility.singleLineHeight), "Key:", labelStyle);
        x += 20 + marginX;
        EditorGUI.PropertyField(
            new Rect(x, y, 150, EditorGUIUtility.singleLineHeight),
            key, GUIContent.none);

        x += 150 + marginX * 2;

        // Listen Button
        isListening.boolValue = EditorGUI.Toggle(new Rect(x, y, 100, EditorGUIUtility.singleLineHeight), isListening.boolValue, "Button");
        EditorGUI.LabelField(new Rect(x + 10, y, 80, EditorGUIUtility.singleLineHeight), isListening.boolValue ? "Listening..." : "Listen to key", labelStyle);

        if (isListening.boolValue)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                isListening.boolValue = false;
                key.SetEnumValue(e.keyCode);
            }
        }
    }
}
