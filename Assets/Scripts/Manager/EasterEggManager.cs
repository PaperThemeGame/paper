using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class EasterEggManager : NormalSingleton<EasterEggManager>
{
    public void CreateEasterEgg()
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, "Egg");
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string eggPath = Path.Combine(desktopPath, "EasterEgg");

        if (Directory.Exists(eggPath))//防止多次复制
        {
            Debug.Log("彩蛋文件已生成过，无需多次生成");
            return;
        }

        try
        {
            Directory.CreateDirectory(eggPath);
            foreach (var file in Directory.GetFiles(sourcePath))
            {
                File.Copy(file, Path.Combine(eggPath, Path.GetFileName(file)));
            }
            Debug.Log("彩蛋生成到桌面");
        }
        catch (Exception e)
        {
            Debug.LogError($"生成失败:{ e.Message}");
        }
    }
}
