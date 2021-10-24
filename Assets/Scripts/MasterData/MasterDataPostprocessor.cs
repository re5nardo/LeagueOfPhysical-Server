using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;

class MasterDataPostprocessor : AssetPostprocessor
{
    private const string TARGET_EXTENSION = ".asset";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var importedAsset in importedAssets)
        {
            OnPostprocessImportedAsset(importedAsset);
        }
    }

    private static void OnPostprocessImportedAsset(string importedAssetPath)
    {
        if (!ShouldProcess(importedAssetPath))
        {
            return;
        }

        var importedAsset = AssetDatabase.LoadAssetAtPath<MasterDataBase>(importedAssetPath);

        HashSet<int> ids = new HashSet<int>();
        string[] guids = AssetDatabase.FindAssets($"t:{importedAsset.GetType().Name}");
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path == importedAssetPath)
            {
                continue;
            }

            ids.Add(AssetDatabase.LoadAssetAtPath<MasterDataBase>(path).id);
        }

        if (ids.Contains(importedAsset.id))
        {
            int newId = ids.Count == 0 ? 0 : ids.Max() + 1;
            Debug.LogWarning($"The id is duplicated (id: {importedAsset.id}). New id is assgined (id: {newId}).");
            importedAsset.id = newId;
            EditorUtility.SetDirty(importedAsset);
        }
    }

    private static bool ShouldProcess(string importedAssetPath)
    {
        if (Path.GetExtension(importedAssetPath) != TARGET_EXTENSION)
        {
            return false;
        }

        if (!AssetDatabase.GetMainAssetTypeAtPath(importedAssetPath).IsSubclassOf(typeof(MasterDataBase)))
        {
            return false;
        }

        return true;
    }
}
