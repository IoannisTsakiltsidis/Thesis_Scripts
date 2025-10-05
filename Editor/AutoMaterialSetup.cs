using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

// Utility to clean up unwanted textures in Textures_External folder
public static class TextureCleaner
{
    [MenuItem("Tools/Clean External Textures")]
    public static void CleanExternalTextures()
    {
        string texturesFolderRelative = "Assets/3D Models/Textures_External";
        string texturesFolderAbsolute = Path.Combine(Application.dataPath, "3D Models/Textures_External");
        if (!Directory.Exists(texturesFolderAbsolute))
        {
            Debug.LogWarning($"Texture folder not found: {texturesFolderAbsolute}");
            return;
        }
        int deletedCount = 0;
        foreach (var file in Directory.GetFiles(texturesFolderAbsolute))
        {
            string name = Path.GetFileNameWithoutExtension(file);
            if (name.All(char.IsDigit) || name.ToLower().Contains("thumbnail"))
            {
                string assetPath = Path.Combine(texturesFolderRelative, Path.GetFileName(file)).Replace("\\", "/");
                if (AssetDatabase.DeleteAsset(assetPath)) deletedCount++;
            }
        }
        AssetDatabase.Refresh();
        Debug.Log($"Cleaned {deletedCount} unwanted textures.");
    }
}
