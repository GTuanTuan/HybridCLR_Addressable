using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class BuildTool
{
    [MenuItem("Tools/增量构建 %g")]  // Ctrl+G (Windows) or Command+G (Mac)
    public static void BuildIncremental()
    {
        CopyHotDll.CreateDllByte();
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("Addressables 增量构建完成 (Ctrl+G)");
    }

    [MenuItem("Tools/全部重新打包 %#g")]  // Ctrl+Shift+G (Windows) or Command+Shift+G (Mac)
    public static void BuildClearCache()
    {
        CopyHotDll.CreateDllByte();
        AddressableAssetSettings.CleanPlayerContent();
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("Addressables 全部重新打包完成 (Ctrl+Shift+G)");
    }
}