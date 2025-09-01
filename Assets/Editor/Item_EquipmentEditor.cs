using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Item_Equipment))]
public class Item_EquipmentEditor : Editor
{
    SerializedProperty icon;
    SerializedProperty itemDescription;
    SerializedProperty equipment_Status;
    SerializedProperty effectList;

    ReorderableList statusList;
    ReorderableList effectReorderableList;

    private void OnEnable()
    {
        icon = serializedObject.FindProperty("icon");
        itemDescription = serializedObject.FindProperty("itemDescription");
        equipment_Status = serializedObject.FindProperty("equipment_Status");
        effectList = serializedObject.FindProperty("effectList");

        // ────────────── 장비 능력치 리스트 ──────────────
        statusList = new ReorderableList(serializedObject, equipment_Status, true, true, true, true);

        statusList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "장비 능력치 목록");
        };

        statusList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = equipment_Status.GetArrayElementAtIndex(index);
            SerializedProperty type = element.FindPropertyRelative("type");
            SerializedProperty value = element.FindPropertyRelative("value");

            rect.y += 2;
            float halfWidth = rect.width / 2 - 5;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                type, GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + halfWidth + 10, rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                value, GUIContent.none);
        };

        // ────────────── 이펙트 리스트 ──────────────
        effectReorderableList = new ReorderableList(serializedObject, effectList, true, true, true, true);

        effectReorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "아이템 이펙트 목록");
        };

        effectReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = effectList.GetArrayElementAtIndex(index);

            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);
        };
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

        // 기본 프로퍼티들
        DrawPropertiesExcluding(serializedObject, "m_Script", "icon", "itemDescription", "equipment_Status", "effectList");

        // 아이템 설명 (큰 입력창)
        EditorGUILayout.LabelField("아이템 설명", EditorStyles.boldLabel);
        float height = EditorGUIUtility.singleLineHeight * 10;
        itemDescription.stringValue = EditorGUILayout.TextArea(itemDescription.stringValue, GUILayout.Height(height));

        EditorGUILayout.Space(10);

        // 장비 능력치
        statusList.DoLayoutList();

        EditorGUILayout.Space(10);

        // 이펙트 리스트
        effectReorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
