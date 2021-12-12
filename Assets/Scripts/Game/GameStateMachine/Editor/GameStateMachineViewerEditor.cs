using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameStateMachineViewer))]
public class GameStateMachineViewerEditor : Editor
{
    private GameStateMachineViewer gameStateMachineViewer;

    private void OnEnable()
    {
        gameStateMachineViewer = (GameStateMachineViewer)target;
    }

    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle(EditorStyles.textField);
        style.fontStyle = FontStyle.Bold;

        EditorGUILayout.TextField("CurrentState", gameStateMachineViewer.GameStateMachine.CurrentState.GetType().Name, style);
    }
}
