using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy_Sound))]
public class EnemySoundEditor : Editor
{
    private SerializedProperty soundDataProp;

    private void OnEnable()
    {
        soundDataProp = serializedObject.FindProperty("soundData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("=== Sound Settings ===", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // �⺻ �ʵ� ǥ��
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSource"), true);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("--- Sound Data List ---", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (soundDataProp != null && soundDataProp.isArray)
        {
            for (int i = 0; i < soundDataProp.arraySize; i++)
            {
                SerializedProperty element = soundDataProp.GetArrayElementAtIndex(i);
                SerializedProperty keyProp = element.FindPropertyRelative("key");
                SerializedProperty clipProp = element.FindPropertyRelative("clip");

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField($"Sound Data {i + 1}", EditorStyles.boldLabel);
                EditorGUILayout.Space(3);

                EditorGUILayout.PropertyField(keyProp, new GUIContent("Key"));
                EditorGUILayout.PropertyField(clipProp, new GUIContent("Audio Clip"));

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("����", GUILayout.Width(60)))
                {
                    soundDataProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                if (GUILayout.Button("�Ʒ��� �߰�", GUILayout.Width(100)))
                {
                    soundDataProp.InsertArrayElementAtIndex(i + 1);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            if (GUILayout.Button("�� �׸� �߰�"))
            {
                soundDataProp.InsertArrayElementAtIndex(soundDataProp.arraySize);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}