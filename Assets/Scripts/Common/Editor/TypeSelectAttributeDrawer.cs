using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomPropertyDrawer(typeof(TypeSelectAttribute), false)]
public class TypeSelectAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var typeSelectAttribute = attribute as TypeSelectAttribute;
		var propertyTypename = TrimTypename(property.managedReferenceFullTypename);
		Vector2 size = GUI.skin.textArea.CalcSize(new GUIContent(propertyTypename));

		var bgColor = GUI.backgroundColor;
		GUI.backgroundColor = Color.green;
		{
			var index = EditorGUI.Popup(
				new Rect(position.x + position.width - size.x - 20, position.y, size.x + 20, size.y),
				Array.FindIndex(typeSelectAttribute.SelectableTypes, type => type.ToString() == propertyTypename),
				typeSelectAttribute.SelectableTypes.Select(type => type.ToString()).ToArray()
				);

			if (index == -1)
            {
				property.managedReferenceValue = Activator.CreateInstance(typeSelectAttribute.SelectableTypes.First());
			}
			else
            {
				var selectType = typeSelectAttribute.SelectableTypes[index];
				if (propertyTypename != selectType.ToString())
				{
					property.managedReferenceValue = Activator.CreateInstance(selectType);
				}
			}
		}
		GUI.backgroundColor = bgColor;

		EditorGUI.PropertyField(position, property, label, true);
	}

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

	private string TrimTypename(string typename)
	{
		if (string.IsNullOrEmpty(typename))
        {
			return typename;
		}

		var index = typename.LastIndexOf(' ');
		if (index >= 0)
        {
			return typename.Substring(index + 1);
		}

		index = typename.LastIndexOf('.');
		if (index >= 0)
        {
			return typename.Substring(index + 1);
		}

		return typename;
	}
}
