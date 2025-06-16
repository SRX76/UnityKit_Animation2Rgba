using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SrxUnity.Recorder;
using System.IO;

public class RecorderMgr : MonoBehaviour
{
    public List<string> clipFiles;
    public int clipIndex = 0;
    public AnimationClip clip;

    public List<string> goPfbFiles;
    public GameObject goPfb;
    public int modelIndex = 0;


    public void Awake()
    {
        CollectionAsset();
    }

    //在指定的路径查找人物模型和动画片段
    void CollectionAsset()
    {
        string animationRootFolder = "Assets/Res/Fbx/Animation";

        clipFiles = AssetKit.FindFile_AnimationClip(animationRootFolder);
        clipIndex = 0;

        string pfbRootFolder = "Assets/Res/Prefab";
        goPfbFiles = AssetKit.FindAssets<GameObject>(pfbRootFolder);
    }

}
