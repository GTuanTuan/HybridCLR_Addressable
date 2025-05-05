using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public class CopyHotDll
{
    [MenuItem("Tools/生成热更Dll二进制文件")]
    public static void CreateDllByte()
    {
        HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
        BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
        foreach (var dll in Boot.Inst.dllMap)
        {
            string sourcePath = dll.Value == 0 ?
                $"{KeyManager.TDllSourceDir}/{buildTarget}/{dll.Key}.dll" :
                $"{KeyManager.HotDllSourceDir}/{buildTarget}/{dll.Key}.dll";
            string destPath = $"{KeyManager.AssetDllDir}/{dll.Key}.dll.bytes"; 
            if (File.Exists(destPath))
                File.Delete(destPath);
            File.Copy(sourcePath, destPath);
            AssetDatabase.Refresh();
            Debug.Log($"copy {sourcePath} to {destPath}");
        }
        Debug.Log($"生成热更Dll二进制文件结束");
    }
  
}
