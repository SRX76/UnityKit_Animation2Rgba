using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        if (!importer.assetPath.StartsWith("Assets/Res/Fbx/Model"))
        {
            return;
        }

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



}
#endif