using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Dialog_Data_SO))]
public class DialogDataSOEditor : Editor
{
    /*
    private ReorderableList dataList;
    private SerializedProperty datasProp;

    private void OnEnable()
    {
        datasProp = serializedObject.FindProperty("datas");

        dataList = new ReorderableList(serializedObject, datasProp, true, true, true, true);

        dataList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Dialog Data List");
        };

        dataList.elementHeightCallback = (int index) =>
        {
            SerializedProperty element = datasProp.GetArrayElementAtIndex(index);
            SerializedProperty eventDatasProp = element.FindPropertyRelative("eventDatas");
            int eventLineCount = eventDatasProp.arraySize * 3;

            float height = 0f;
            height += EditorGUIUtility.singleLineHeight * 2; // dialogName + spacing
            height += EditorGUIUtility.singleLineHeight;     // ---Event---
            height += EditorGUIUtility.singleLineHeight * eventLineCount;
            height += EditorGUIUtility.singleLineHeight + 6f; // Add EventData Button
            height += EditorGUIUtility.singleLineHeight;     // ---Dialog---
            height += EditorGUIUtility.singleLineHeight * 2; // dialog_Daley + characterName
            height += EditorGUIUtility.singleLineHeight * 6; // dialog TextArea (¾à 5ÁÙ)
            height += 20f; // margin
            return height;
        };

        dataList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = datasProp.GetArrayElementAtIndex(index);
            float y = rect.y + 2f;
            float fullWidth = rect.width;
            float halfWidth = fullWidth / 2 - 5;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;

            EditorGUI.PropertyField(new Rect(rect.x, y, fullWidth, lineHeight),
                element.FindPropertyRelative("dialogName"));
            y += lineHeight + spacing + 2;

            EditorGUI.LabelField(new Rect(rect.x, y, fullWidth, lineHeight), "--- Event ---");
            y += lineHeight + spacing;

            SerializedProperty eventDatas = element.FindPropertyRelative("eventDatas");
            for (int i = 0; i < eventDatas.arraySize; i++)
            {
                SerializedProperty ev = eventDatas.GetArrayElementAtIndex(i);
                SerializedProperty evType = ev.FindPropertyRelative("evnetType");
                SerializedProperty evPos = ev.FindPropertyRelative("evnetPos");
                SerializedProperty evIdx = ev.FindPropertyRelative("typeIndex");
                SerializedProperty evOnOff = ev.FindPropertyRelative("typeOnOff");

                EditorGUI.PropertyField(new Rect(rect.x, y, halfWidth, lineHeight), evType, GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + halfWidth + 10, y, halfWidth, lineHeight), evPos, GUIContent.none);
                y += lineHeight + spacing;

                EditorGUI.PropertyField(new Rect(rect.x, y, halfWidth, lineHeight), evIdx, GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + halfWidth + 10, y, halfWidth, lineHeight), evOnOff, GUIContent.none);
                y += lineHeight + spacing + 2;
            }

            if (GUI.Button(new Rect(rect.x, y, fullWidth, lineHeight), "+ Add EventData"))
            {
                eventDatas.InsertArrayElementAtIndex(eventDatas.arraySize);
            }
            y += lineHeight + spacing + 4;

            EditorGUI.LabelField(new Rect(rect.x, y, fullWidth, lineHeight), "--- Dialog ---");
            y += lineHeight + spacing;

            EditorGUI.PropertyField(new Rect(rect.x, y, fullWidth, lineHeight),
                element.FindPropertyRelative("dialog_Daley"));
            y += lineHeight + spacing;

            EditorGUI.PropertyField(new Rect(rect.x, y, fullWidth, lineHeight),
                element.FindPropertyRelative("characterName"));
            y += lineHeight + spacing;

            var dialogProp = element.FindPropertyRelative("dialog");
            float textAreaHeight = lineHeight * 5;
            dialogProp.stringValue = EditorGUI.TextArea(
                new Rect(rect.x, y, fullWidth, textAreaHeight),
                dialogProp.stringValue
            );
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        dataList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
    */
}
