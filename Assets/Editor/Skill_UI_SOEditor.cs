using UnityEngine;
using UnityEditor;
using UnityEngine.Video;

[CustomEditor(typeof(Skill_UI_SO))]
public class Skill_UI_SOEditor : Editor
{
    private SerializedProperty iconImage;
    private SerializedProperty clip;
    private SerializedProperty skillName;
    private SerializedProperty skillDescription;

    private void OnEnable()
    {
        iconImage = serializedObject.FindProperty("iconImage");
        clip = serializedObject.FindProperty("clip");
        skillName = serializedObject.FindProperty("skillName");
        skillDescription = serializedObject.FindProperty("skillDescription");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("=== Skill UI Data ===", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 아이콘
        EditorGUILayout.PropertyField(iconImage, new GUIContent("Icon Image"));
        // 영상
        EditorGUILayout.PropertyField(clip, new GUIContent("Skill Video Clip"));
        // 이름
        EditorGUILayout.PropertyField(skillName, new GUIContent("Skill Name"));

        EditorGUILayout.Space();

        // skillDescription 배열
        EditorGUILayout.LabelField("Skill Description", EditorStyles.boldLabel);

        if (skillDescription != null)
        {
            // 배열 크기 조절
            EditorGUILayout.PropertyField(skillDescription.FindPropertyRelative("Array.size"), new GUIContent("Description Count"));

            EditorGUILayout.Space();

            // 각 요소 그리기 (TextArea)
            for (int i = 0; i < skillDescription.arraySize; i++)
            {
                SerializedProperty element = skillDescription.GetArrayElementAtIndex(i);

                EditorGUILayout.LabelField($"Line {i}", EditorStyles.miniBoldLabel);

                // TextArea 사용, 높이 넓게 지정
                element.stringValue = EditorGUILayout.TextArea(element.stringValue, GUILayout.Height(240));

                EditorGUILayout.Space();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}