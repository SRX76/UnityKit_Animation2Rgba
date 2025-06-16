using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SrxUnity.Recorder;
using System.IO;
using System;

public class RecorderMgr : MonoBehaviour
{
    public List<string> clipFiles;
    public int clipIndex = -1;
    public AnimationClip clip;

    public List<string> goPfbFiles;
    public GameObject goPfb;
    public int modelIndex = -1;


    private Transform modelRoot;
    public RuntimeAnimatorController animatorController;

    public Camera recorderCamera;
    public Animator animator;
    public float clipTime = 0;
    public float curClipTime = 0;
    public float dtTime = 0.04f;//fps:25
    public int texId = 0;

    public string modelName;
    public string clipName;


    public void Awake()
    {
        Application.targetFrameRate = 60;
        CollectionAsset();

        modelRoot = transform.Find("Model");
        recorderCamera = transform.Find("CameraRoot/Camera").GetComponent<Camera>();
        recorderCamera?.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        yield return null;
        string saveRootFolder = "../output";
        modelIndex = -1;
        while (LoadNextModel())
        {
            if (!Directory.Exists($"{saveRootFolder}/{modelName}"))
            {
                Directory.CreateDirectory($"{saveRootFolder}/{modelName}");
            }
            clipIndex = -1;
            while (LoadNextAnimationClip())
            {
                OverrideController();
                yield return null;
                texId = 0;
                int maxCount = Mathf.FloorToInt(clip.length / dtTime);
                for (int i = 0; i < maxCount; i++)
                {
                    yield return null;
                    texId++;
                    animator.Update(dtTime);
                    yield return new WaitForEndOfFrame();
                    string savePath = $"{saveRootFolder}/{modelName}/{clipName}_{texId}.png";
                    Recorder(savePath, () =>
                    {
                        Debug.Log($"保存成功:{savePath}");
                    });
                }
            }
            ClearOldModel();
        }
        ClearOldModel();
    }

    [ContextMenu("调试")]
    void Test()
    {

    }

    //清理旧的人物模型
    void ClearOldModel()
    {
        if (goPfb != null)
        {
            DestroyImmediate(goPfb);
            goPfb = null;
        }
        Resources.UnloadUnusedAssets();
    }
    //加载新的人物模型
    bool LoadNextModel()
    {
        modelIndex++;
        if (modelIndex >= goPfbFiles.Count)
        {
            return false;
        }
        var pfb = AssetKit.LoadAssetAtPath<GameObject>(goPfbFiles[modelIndex]);
        pfb.gameObject.SetActive(false);
        goPfb = GameObject.Instantiate(pfb, modelRoot);
        animator = goPfb.GetComponent<Animator>();
        animator.enabled = false;
        animator.runtimeAnimatorController = animatorController;
        goPfb.SetActive(true);
        modelName = pfb.name;
        return true;
    }

    bool LoadNextAnimationClip()
    {
        clipIndex++;
        if (clipIndex >= clipFiles.Count)
        {
            return false;
        }
        clip = AssetKit.LoadAnimationClip(clipFiles[clipIndex]);
        clipTime = clip.length;
        curClipTime = 0;
        clipName = Path.GetFileNameWithoutExtension(clipFiles[clipIndex]);
        return clip != null;
    }

    //覆盖动画控制
    void OverrideController()
    {
        animator.runtimeAnimatorController = null;
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animatorController);
        animatorOverrideController["PlayBase"] = clip;
        animator.runtimeAnimatorController = animatorOverrideController;
        animator.playableGraph.SetTimeUpdateMode(UnityEngine.Playables.DirectorUpdateMode.Manual);
        animator.Play("Empty");
        animator.updateMode = AnimatorUpdateMode.Normal;
    }

    //开始录制数据

    void Recorder(string savePath, Action act_Success)
    {
        CameraKit.SaveCamTexture(recorderCamera, savePath, act_Success);

    }


    //在指定的路径查找人物模型和动画片段
    void CollectionAsset()
    {
        animatorController = AssetKit.LoadAnimatorController("Assets/Res/Animator/HumanController.controller");
        string animationRootFolder = "Assets/Res/Fbx/Animation";

        clipFiles = AssetKit.FindFile_AnimationClip(animationRootFolder);
        clipIndex = 0;

        string pfbRootFolder = "Assets/Res/Prefab";
        goPfbFiles = AssetKit.FindAssets<GameObject>(pfbRootFolder);
    }

}
