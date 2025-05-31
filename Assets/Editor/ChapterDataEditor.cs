using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Chapter_Data_SO))]
public class ChapterDataEditor : Editor
{
    private SerializedProperty chapterNameProp;
    private SerializedProperty stageDataProp;

    private void OnEnable()
    {
        chapterNameProp = serializedObject.FindProperty("chapterName");
        stageDataProp = serializedObject.FindProperty("stageData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(chapterNameProp, new GUIContent("Chapter Name"));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Stage List", EditorStyles.boldLabel);

        for (int i = 0; i < stageDataProp.arraySize; i++)
        {
            SerializedProperty stageProp = stageDataProp.GetArrayElementAtIndex(i);
            SerializedProperty stageTypeProp = stageProp.FindPropertyRelative("stageType");
            SerializedProperty stageImageProp = stageProp.FindPropertyRelative("stageImage");
            SerializedProperty stageNameProp = stageProp.FindPropertyRelative("stageName");
            SerializedProperty stageLevelProp = stageProp.FindPropertyRelative("stageLevel");
            SerializedProperty stageSummationProp = stageProp.FindPropertyRelative("stageSummation");
            SerializedProperty stageDescriptionProp = stageProp.FindPropertyRelative("stageDescription");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Stage {i + 1}", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(stageTypeProp);
            EditorGUILayout.PropertyField(stageNameProp);
            EditorGUILayout.PropertyField(stageLevelProp);

            // 이미지 필드와 미리보기
            EditorGUILayout.PropertyField(stageImageProp);
            Sprite sprite = (Sprite)stageImageProp.objectReferenceValue;
            if (sprite != null)
            {
                Rect spriteRect = GUILayoutUtility.GetRect(128, 128, GUILayout.ExpandWidth(false));
                EditorGUI.DrawPreviewTexture(spriteRect, sprite.texture, null, ScaleMode.ScaleToFit);
            }

            // 넓은 텍스트 필드
            EditorGUILayout.LabelField("Summation");
            stageSummationProp.stringValue = EditorGUILayout.TextArea(stageSummationProp.stringValue, GUILayout.MinHeight(60));

            EditorGUILayout.LabelField("Description");
            stageDescriptionProp.stringValue = EditorGUILayout.TextArea(stageDescriptionProp.stringValue, GUILayout.MinHeight(100));

            if (GUILayout.Button("Remove Stage"))
            {
                stageDataProp.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add New Stage"))
        {
            stageDataProp.InsertArrayElementAtIndex(stageDataProp.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
