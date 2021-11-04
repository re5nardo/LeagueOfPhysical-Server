using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;

public class MasterDataDefineGenerator
{
    [MenuItem("Assets/GenerateMasterDataDefine")]
    private static void GenerateMasterDataDefine()
    {
        GenerateMasterDataDefine(Selection.activeObject.GetType());
    }
    
    [MenuItem("Assets/GenerateMasterDataDefine", true)]
    private static bool GenerateMasterDataDefineValidation()
    {
        return Selection.activeObject is MasterDataBase;
    }

    private static void GenerateMasterDataDefine(Type type)
    {
        StringBuilder stringBuilder = new StringBuilder()
        .AppendLine()
        .AppendLine("namespace Define")
        .AppendLine("{")
        .AppendTab().AppendLine("namespace MasterData")
        .AppendTab().AppendLine("{")
        .AppendTab().AppendTab().AppendLine($"public class {type.Name.Replace("MasterData", "")}Id")
        .AppendTab().AppendTab().AppendLine("{")
        ;

        string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");
        foreach (string guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<MasterDataBase>(assetPath);

            stringBuilder.AppendTab().AppendTab().AppendTab().AppendLine($"public const int {asset.name} = {asset.id};");
        }

        stringBuilder.AppendTab().AppendTab().AppendLine("}")
        .AppendTab().AppendLine("}")
        .AppendLine("}")
        ;

        string path = Path.Combine(Application.dataPath, "Scripts/MasterData/GeneratedSourceCodeFile", $"Define.{type.Name}.Generated.cs");

        File.WriteAllText(path, stringBuilder.ToString());

        AssetDatabase.Refresh();
    }
}
