using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace SrxUnity.Recorder
{
    public class AssetKit
    {
        //在指定路径下查找所有指定类型的资源文件
        public static List<string> FindAssets<T>(string rootPath)
        {
            List<string> res = new();
#if UNITY_EDITOR
            Debug.Log($"查找资源类型: {typeof(T).Name} 在路径: {rootPath}");
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[] { rootPath });
            res = new List<string>(guids.Length);
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(path))
                {
                    res.Add(path);
                }
            }
#endif
            return res;
        }

        public static List<string> FindFile_AnimationClip(string rootPath)
        {
            return FindAssets<AnimationClip>(rootPath);
        }

        public static List<AnimationClip> FindAnimationClips(string rootPath)
        {
            var files = FindAssets<AnimationClip>(rootPath);
            List<AnimationClip> res = new List<AnimationClip>(files.Count);
            foreach (var file in files)
            {
                var clip = LoadAnimationClip(file);
                if (clip != null)
                {
                    res.Add(clip);
                }
            }
            return res;
        }

        public static AnimationClip LoadAnimationClip(string file)
        {
            AnimationClip clip = null;
#if UNITY_EDITOR
            clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);

#endif
            return clip;
        }

        public static RuntimeAnimatorController LoadAnimatorController(string file)
        {
            return LoadAssetAtPath<AnimatorController>(file);
        }

        public static T LoadAssetAtPath<T>(string file) where T : Object
        {
            T asset = null;
#if UNITY_EDITOR
            asset = AssetDatabase.LoadAssetAtPath<T>(file);
#endif
            if (asset == null)
            {
                Debug.LogError($"加载资源失败：类型{typeof(T).Name},path:{file}");
            }
            return asset;
        }

        [MenuItem("Tools/格式转换为png")]
        static void ConvertToPng()
        {
            //string file = "Recordings/Image Sequence_007_0000.png";
            string file = "_SavePath.bin";
            var bytes = File.ReadAllBytes(file);
            Texture2D tex = new Texture2D(1920, 1080, TextureFormat.RGBA32, false);
            tex.LoadRawTextureData(bytes);
            tex.Apply();
            var bytesSave = tex.EncodeToPNG();
            File.WriteAllBytes("_tex.png", bytesSave);
            UnityEngine.Object.DestroyImmediate(tex);
            tex = null;
        }
    }
}