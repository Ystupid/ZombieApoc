using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(KeyEventInfo))]
public class KeyEventInfoEditor : Editor
{
    private SerializedProperty _keyEvent;
    private SerializedProperty _intValue;
    private SerializedProperty _floatValue;

    private void OnEnable()
    {
        _keyEvent = serializedObject.FindProperty("EKeyEvent");
        _intValue = serializedObject.FindProperty("IntValue");
        _floatValue = serializedObject.FindProperty("FloatValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_keyEvent);

        var eventType = (EKeyEvent)_keyEvent.enumValueFlag;

        if (eventType == EKeyEvent.None)
        {
        }
        else
        {
            EditorGUILayout.PropertyField(_intValue);
            EditorGUILayout.PropertyField(_floatValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
