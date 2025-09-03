using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Item_Consumable))]
public class Item_ConsumableEditor : Editor
{
    SerializedProperty icon;
    SerializedProperty itemDescription;

    SerializedProperty key;
    SerializedProperty consumableType;
    SerializedProperty timeCooldown;
    SerializedProperty effect_duration;
    SerializedProperty heal_interval;

    SerializedProperty healing;
    SerializedProperty stamina;
    SerializedProperty awakening;

    SerializedProperty recoveryVFX;

    private void OnEnable()
    {
        icon = serializedObject.FindProperty("icon");
        itemDescription = serializedObject.FindProperty("itemDescription");

        key = serializedObject.FindProperty("key");
        consumableType = serializedObject.FindProperty("consumableType");
        timeCooldown = serializedObject.FindProperty("timeCooldown");
        effect_duration = serializedObject.FindProperty("effect_duration");
        heal_interval = serializedObject.FindProperty("heal_interval");

        healing = serializedObject.FindProperty("healing");
        stamina = serializedObject.FindProperty("stamina");
        awakening = serializedObject.FindProperty("awakening");

        recoveryVFX = serializedObject.FindProperty("recoveryVFX");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ────────────── 아이콘 미리보기 ──────────────
        EditorGUILayout.LabelField("아이템 아이콘", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(icon, new GUIContent("아이콘 이미지"));

        if (icon.objectReferenceValue != null)
        {
            Texture2D previewTex = AssetPreview.GetAssetPreview(icon.objectReferenceValue);
            if (previewTex != null)
            {
                float size = 80f; // 미리보기 크기
                GUILayout.Label(previewTex, GUILayout.Width(size), GUILayout.Height(size));
            }
        }

        EditorGUILayout.Space(10);

        // ────────────── 기본 정보 ──────────────
        DrawPropertiesExcluding(serializedObject, "m_Script", "icon", "itemDescription",
            "key", "consumableType", "timeCooldown", "effect_duration", "heal_interval",
            "healing", "stamina", "awakening", "recoveryVFX");

        // ────────────── 아이템 설명 (큰 입력창) ──────────────
        EditorGUILayout.LabelField("아이템 설명", EditorStyles.boldLabel);
        float height = EditorGUIUtility.singleLineHeight * 6;
        itemDescription.stringValue = EditorGUILayout.TextArea(itemDescription.stringValue, GUILayout.Height(height));

        EditorGUILayout.Space(10);

        // ────────────── 소비 아이템 전용 ──────────────
        EditorGUILayout.LabelField("소비 아이템 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(key);
        EditorGUILayout.PropertyField(consumableType);
        EditorGUILayout.PropertyField(timeCooldown);

        if ((Item_Consumable.ConsumableType)consumableType.enumValueIndex == Item_Consumable.ConsumableType.persistence)
        {
            EditorGUILayout.PropertyField(effect_duration, new GUIContent("효과 지속 시간"));
            EditorGUILayout.PropertyField(heal_interval, new GUIContent("회복 간격"));
        }

        EditorGUILayout.Space(10);

        // ────────────── 회복 수치 ──────────────
        EditorGUILayout.LabelField("회복 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(healing, new GUIContent("HP 회복량"));
        EditorGUILayout.PropertyField(stamina, new GUIContent("스태미나 회복량"));
        EditorGUILayout.PropertyField(awakening, new GUIContent("각성 게이지 회복량"));

        EditorGUILayout.Space(10);

        // ────────────── 이펙트 ──────────────
        EditorGUILayout.LabelField("회복 이펙트", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(recoveryVFX);

        serializedObject.ApplyModifiedProperties();
    }
}