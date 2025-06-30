using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;


public class ModelImporterProcess : AssetPostprocessor
{

    public void OnPreprocessModel()
    {
        ModelImporter importer = assetImporter as ModelImporter;
        if (importer == null)
        {
            return;
        }
        if (importer.assetPath.StartsWith("Assets/Res/Fbx/Model"))
        {
            SetPreModelImporter_1(importer);
            return;
        }
    }
    void SetPreModelImporter_0(ModelImporter importer)
    {
        string folder = Path.GetDirectoryName(importer.assetPath);
        string modelName = Path.GetFileNameWithoutExtension(importer.assetPath);
        importer.animationType = ModelImporterAnimationType.Human;
        importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;

        importer.materialLocation = ModelImporterMaterialLocation.External;
        importer.ExtractTextures($"{folder}/{modelName}.fbm");
        importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Local);
        importer.autoGenerateAvatarMappingIfUnspecified = true;
        importer.importBlendShapes = true;
        importer.importAnimation = false;
        importer.importConstraints = false;
        importer.importCameras = false;
        importer.importLights = false;
        // Set the scale factor to 1
        importer.globalScale = 1.0f;
        // Set the mesh compression to medium
        importer.meshCompression = ModelImporterMeshCompression.Off; ;
        // Set the normals and tangents to import
        importer.importNormals = ModelImporterNormals.Import;
        importer.importTangents = ModelImporterTangents.CalculateMikk;
    }
    void SetPreModelImporter_1(ModelImporter importer)
    {
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
        importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
        var folder = assetPath;
        if (folder.EndsWith(".fbx"))
        {
            folder = folder.Substring(0, folder.Length - 4) + "_Textures";
        }
        var res = importer.ExtractTextures(folder);
    }


    private void OnPostprocessModel(GameObject go)
    {
        if (!assetPath.EndsWith(".fbx"))
        {
            return;
        }
        var folder = assetPath.Substring(0, assetPath.Length - 4) + "_Textures";
        if (!Directory.Exists(folder))
        {
            return;
        }
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.ImportAsset(folder, ImportAssetOptions.ImportRecursive);

            AssetDatabase.ImportAsset(Path.GetDirectoryName(folder));
        }
        else
        {
            foreach (var child in go.GetComponentsInChildren<Renderer>())
            {
                foreach (var material in child.sharedMaterials)
                {
                    if (material.name.ToLower().Contains("hair"))
                    {
                        //0: Opaque
                        //1: cutout
                        //2: fade
                        //3: 透明transparent
                        material.SetFloat("_Mode", 2);
                    }
                }
            }
        }
    }

    private void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;
        if (importer == null)
        {
            return;
        }
        if (Path.GetFileNameWithoutExtension(assetPath).ToLower().Contains("normal"))
        {
            importer.textureType = TextureImporterType.NormalMap;
        }

    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool needReimport = false;
        string targetPath = "Assets/Res/Fbx/Model";

        Debug.LogError($"文件列表：{importedAssets.Length}\n{string.Join("\n", importedAssets)}");
        if (importedAssets.Length == 1 && importedAssets[0] == targetPath)
        {
            return;
        }

        if (importedAssets.Any(_ => _.Contains(targetPath)) || movedAssets.Any(_ => _.Contains(targetPath)))
        {
            var guids = AssetDatabase.FindAssets("t:GameoObject", new string[] { targetPath });
            foreach (var guid in guids)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));

                if (go != null)
                {
                    foreach (var child in go.GetComponentsInChildren<Renderer>())
                    {
                        foreach (var material in child.sharedMaterials)
                        {
                            if (material.name.ToLower().Contains("hair"))
                            {
                                //0: Opaque
                                //1: cutout
                                //2: fade
                                //3: 透明transparent
                                material.SetFloat("_Mode", 2);
                            }
                        }
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
    }

}
#endif