using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(StateMasterData))]
[CanEditMultipleObjects]
public class StateMasterDataEditor : Editor
{
    private SerializedProperty id;
    private SerializedProperty className;
    private SerializedProperty lifespan;
    private SerializedProperty overlapResolveType;
    private SerializedProperty stateAttributes;
    private SerializedProperty statusEffects;

    private int index = 0;
    private string[] classNames;

    private void Awake()
    {
        List<string> classNameList = new List<string>();
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(State.StateBase).IsAssignableFrom(type))
                {
                    classNameList.Add(type.ToString());
                }
            }
        }
        classNames = classNameList.ToArray();
    }

    private void OnEnable()
    {
        id = serializedObject.FindProperty("id");
        className = serializedObject.FindProperty("className");
        lifespan = serializedObject.FindProperty("lifespan");
        overlapResolveType = serializedObject.FindProperty("overlapResolveType");
        stateAttributes = serializedObject.FindProperty("stateAttributes");
        statusEffects = serializedObject.FindProperty("statusEffects");

        index = 0;
        for (int i = 0; i < classNames.Length; ++i)
        {
            if (classNames[i] == className.stringValue)
            {
                index = i;
                break;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(id);

        index = EditorGUILayout.Popup("ClassName", index, classNames);
        className.stringValue = classNames[index];

        EditorGUILayout.PropertyField(lifespan);
        EditorGUILayout.PropertyField(overlapResolveType);
        EditorGUILayout.PropertyField(stateAttributes);
        EditorGUILayout.PropertyField(statusEffects);

        serializedObject.ApplyModifiedProperties();
    }
}
