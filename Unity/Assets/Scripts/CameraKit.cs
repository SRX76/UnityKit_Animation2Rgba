using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SrxUnity.Recorder
{
    public class CameraKit
    {
        public static void SaveCamTexture(Camera cam, string filePath, Action actSuccess = null, Action actFailed = null)
        {
            if (cam == null)
            {
                actFailed?.Invoke();
                return;
            }
            int Width = 1920;
            int Height = 1080;
            var rtTemp = RenderTexture.GetTemporary(Width, Height, 0, RenderTextureFormat.ARGB32);
            RenderTexture preRt = cam.targetTexture;
            cam.targetTexture = rtTemp;
            cam.Render();
            cam.targetTexture = preRt;
            preRt = RenderTexture.active;
            RenderTexture.active = rtTemp;
            Texture2D tex = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            tex.Apply();
            RenderTexture.active = preRt;
            RenderTexture.ReleaseTemporary(rtTemp);
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);

            actSuccess?.Invoke();
        }

    }
}