using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace SrxUnity.Recorder
{
    public class CameraKit
    {
        static Stack<NativeArray<byte>> stack = new Stack<NativeArray<byte>>();


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

        public static void SaveCamTexture2(Camera cam, string filePath)
        {
            //SystemInfo.supportsAsyncGPUReadback
            cam.Render();
            var rt = cam.targetTexture;
            int Width = rt.width;
            int Height = rt.height;


            var buff = new NativeArray<byte>(Width * Height * 4, Allocator.Persistent);
            var format = rt.graphicsFormat;
            var req = AsyncGPUReadback.RequestIntoNativeArray(ref buff, rt, 0, format, (_) =>
            {
                if (_.hasError)
                {
                    Debug.LogError("AsyncGPUReadback error");
                    buff.Dispose();
                    return;
                }
                var bytesSRC = _.GetData<byte>().ToArray();
                buff.Dispose();
                SaveWithThread(() =>
                    {
                        var bytes = ImageConversion.EncodeArrayToPNG(bytesSRC, format, (uint)Width, (uint)Height);
                        File.WriteAllBytes(filePath, bytes);
                    });
            });
        }




        static void SaveWithThread(Action act)
        {
            var subThread = new Thread((_) => act?.Invoke());
            subThread.Start();
        }
    }
}