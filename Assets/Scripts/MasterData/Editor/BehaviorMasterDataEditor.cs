using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BehaviorMasterData))]
[CanEditMultipleObjects]
public class BehaviorMasterDataEditor : Editor
{
    private SerializedProperty id;
    private SerializedProperty className;
    private SerializedProperty lifespan;
    private SerializedProperty classParams;
    private SerializedProperty statusEffects;
    private SerializedProperty compatibleBehaviors;

    private int index = 0;
    private string[] classNames;

    private void Awake()
    {
        List<string> classNameList = new List<string>();
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(Behavior.BehaviorBase).IsAssignableFrom(type))
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
        classParams = serializedObject.FindProperty("classParams");
        statusEffects = serializedObject.FindProperty("statusEffects");
        compatibleBehaviors = serializedObject.FindProperty("compatibleBehaviors");

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
        EditorGUILayout.PropertyField(classParams);
        EditorGUILayout.PropertyField(statusEffects);
        EditorGUILayout.PropertyField(compatibleBehaviors);

        serializedObject.ApplyModifiedProperties();
    }
}
