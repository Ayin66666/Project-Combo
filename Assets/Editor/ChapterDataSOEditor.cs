using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Chapter_Data_SO))]
public class CapterDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Chapter_Data_SO data = (Chapter_Data_SO)target;

        EditorGUILayout.LabelField("Chapter Name", EditorStyles.boldLabel);
        data.chapterName = EditorGUILayout.TextField(data.chapterName);

        EditorGUILayout.Space();

        if (data.stageData == null)
            data.stageData = new List<Chapter_Data_SO.Stage>();

        for (int i = 0; i < data.stageData.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            var stage = data.stageData[i];

            stage.stageName = EditorGUILayout.TextField("Stage Name", stage.stageName);

            stage.stageImage = (Sprite)EditorGUILayout.ObjectField("Stage Image", stage.stageImage, typeof(Sprite), false);

            /*
            // ��� �̹��� �̸�����
            if (stage.stageImage != null)
            {
                Texture2D tex = AssetPreview.GetAssetPreview(stage.stageImage);
                if (tex != null)
                {
                    GUILayout.Label(tex, GUILayout.Width(120), GUILayout.Height(120));
                }
            }
            */
            EditorGUILayout.LabelField("Stage Description");
            stage.stageDescription = EditorGUILayout.TextArea(stage.stageDescription, GUILayout.Height(120));


            // ���� ��ư
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("����", GUILayout.Width(80)))
            {
                data.stageData.RemoveAt(i);
                break;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            data.stageData[i] = stage;

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("�������� �߰�"))
        {
            data.stageData.Add(new Chapter_Data_SO.Stage());
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
