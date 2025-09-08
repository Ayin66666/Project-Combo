using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Skill_Value_SO))]
public class Skill_Value_SOEditor : Editor
{
    private SerializedProperty valueList;

    private void OnEnable()
    {
        valueList = serializedObject.FindProperty("value_List");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("=== Skill Data ===", EditorStyles.boldLabel);

        if (valueList == null)
        {
            EditorGUILayout.HelpBox("Value List is missing.", MessageType.Warning);
            return;
        }

        // 리스트 사이즈 (최상단에 표시)
        EditorGUILayout.PropertyField(valueList.FindPropertyRelative("Array.size"));

        EditorGUILayout.Space();

        // 각 요소 그리기
        for (int i = 0; i < valueList.arraySize; i++)
        {
            SerializedProperty element = valueList.GetArrayElementAtIndex(i);
            SerializedProperty name = element.FindPropertyRelative("name");
            SerializedProperty type = element.FindPropertyRelative("type");
            SerializedProperty armor = element.FindPropertyRelative("armor");
            SerializedProperty attackEffect = element.FindPropertyRelative("attackEffect");
            SerializedProperty motionValue = element.FindPropertyRelative("motionValue");
            SerializedProperty hitCount = element.FindPropertyRelative("hitCount");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Skill Level {i}", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(name, new GUIContent("Name"));
            EditorGUILayout.PropertyField(type, new GUIContent("Damage Type"));
            EditorGUILayout.PropertyField(armor, new GUIContent("Armor Type"));
            EditorGUILayout.PropertyField(attackEffect, new GUIContent("Attack Effect"));
            EditorGUILayout.PropertyField(motionValue, new GUIContent("Motion Value"));
            EditorGUILayout.PropertyField(hitCount, new GUIContent("Hit Count"));

            EditorGUILayout.Space();

            // 제거 버튼
            if (GUILayout.Button("Remove This Entry"))
            {
                valueList.DeleteArrayElementAtIndex(i);
                break; // 바로 루프 탈출 (에러 방지)
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();

        // 추가 버튼
        if (GUILayout.Button("Add New Entry"))
        {
            int newIndex = valueList.arraySize;
            valueList.InsertArrayElementAtIndex(newIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
