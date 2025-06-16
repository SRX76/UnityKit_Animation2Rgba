using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

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
                var clip = LoadAssetAtPath(file);
                if (clip != null)
                {
                    res.Add(clip);
                }
            }
            return res;
        }

        public static AnimationClip LoadAssetAtPath(string file)
        {
            AnimationClip clip = null;
#if UNITY_EDITOR
            clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);

#endif
            return clip;
        }
    }
}