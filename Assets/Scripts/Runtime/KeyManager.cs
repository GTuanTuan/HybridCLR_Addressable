using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KeyManager
{
    public static string AssetDllDir = "Assets/GameRes/HotUpdate";
    public static string AssetEntityDir = "Assets/GameRes/Entity";
    public static string HybridCLRDataDir = $"{Application.dataPath.Replace("/Assets", "")}/HybridCLRData";
    public static string HotDllSourceDir = $"{HybridCLRDataDir}/HotUpdateDlls";
    public static string TDllSourceDir = $"{HybridCLRDataDir}/AssembliesPostIl2CppStrip";
    public static string LoadDllKey(string dllName)
    {
        return $"{AssetDllDir}/{dllName}.dll.bytes";
    }
    public static string LoadPrefabKey(string prefabName)
    {
        return $"{AssetEntityDir}/{prefabName}.prefab";
    }
}
