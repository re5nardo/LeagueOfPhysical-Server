using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SubGameStateMachineViewer))]
public class SubGameStateMachineViewerEditor : Editor
{
    private SubGameStateMachineViewer subGameStateMachineViewer;

    private void OnEnable()
    {
        subGameStateMachineViewer = (SubGameStateMachineViewer)target;
    }

    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle(EditorStyles.textField);
        style.fontStyle = FontStyle.Bold;

        EditorGUILayout.TextField("CurrentState", subGameStateMachineViewer.SubGameStateMachine.CurrentState.GetType().Name, style);
    }
}
