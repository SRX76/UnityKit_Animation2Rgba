using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
public static class PngSaveKit
{
    public static void SaveToPng(byte[] imageData, string filePath, GraphicsFormat format, int width, int height)
    {
        try
        {
            var bytes = ImageConversion.EncodeArrayToPNG(imageData, format, (uint)width, (uint)height);
            File.WriteAllBytes(filePath, bytes);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public static void SaveToPngInThreadPool(byte[] imageData, string filePath, GraphicsFormat format, int width, int height, Action cb)
    {
        ThreadPool.QueueUserWorkItem((_) =>
        {
            SaveToPng(imageData, filePath, format, width, height);
            cb?.Invoke();
        });
    }
}
