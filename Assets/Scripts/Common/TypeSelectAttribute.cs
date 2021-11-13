using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class TypeSelectAttribute : PropertyAttribute
{
    public Type[] SelectableTypes { get; private set; }

	public TypeSelectAttribute(params Type[] types)
	{
        HashSet<Type> selectableTypes = new HashSet<Type>();

        foreach (var type in types)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var candidate in assembly.GetTypes())
                {
                    if (type.IsAssignableFrom(candidate))
                    {
                        selectableTypes.Add(candidate);
                    }
                }
            }
        }

        SelectableTypes = selectableTypes.ToArray();
    }
}
