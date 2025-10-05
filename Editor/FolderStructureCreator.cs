using UnityEngine;
using UnityEditor;
using System.IO;

public class FolderStructureCreator
{
    [MenuItem("Tools/Generate Folder Structure")]
    public static void CreateFolders()
    {
        string[] folders = new string[]
        {
            "Assets/_Project",
            "Assets/_Project/Scripts/Player",
            "Assets/_Project/Scripts/AI",
            "Assets/_Project/Scripts/Environment",
            "Assets/_Project/Scripts/Systems",
            "Assets/_Project/Prefabs/Environment",
            "Assets/_Project/Prefabs/Characters",
            "Assets/_Project/Prefabs/UI",
            "Assets/_Project/Materials",
            "Assets/_Project/Scenes",
            "Assets/_Project/Shaders",
            "Assets/_Project/Audio/Music",
            "Assets/_Project/Audio/SFX",
            "Assets/_Project/Audio/Voice",
            "Assets/_Project/Animation/Characters",
            "Assets/_Project/Animation/Environment",
            "Assets/_Project/UI/Canvases",
            "Assets/_Project/UI/Sprites",
            "Assets/_Project/UI/Fonts",
            "Assets/_Project/Textures",
            "Assets/_Project/VFX",
            "Assets/_Project/Data/ScriptableObjects",
            "Assets/_Project/Data/Localization",
            "Assets/_Project/Dialogue",
            "Assets/_Project/Quests",
            "Assets/_Project/Plugins",
            "Assets/_ExternalAssets/AI_Assets",
            "Assets/_ExternalAssets/AssetStoreTools",
            "Assets/_ExternalAssets/ThirdParty",
            "Assets/_Settings/RenderPipeline",
            "Assets/_Settings/Input",
            "Assets/_Settings/TagsAndLayers",
            "Assets/_Settings/Physics"
        };

        foreach (string folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Debug.Log("Created folder: " + folder);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ Folder structure generated successfully!");
    }
}
