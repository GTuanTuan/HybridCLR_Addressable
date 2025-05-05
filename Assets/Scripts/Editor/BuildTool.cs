using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class BuildTool
{
    [MenuItem("Tools/�������� %g")]  // Ctrl+G (Windows) or Command+G (Mac)
    public static void BuildIncremental()
    {
        CopyHotDll.CreateDllByte();
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("Addressables ����������� (Ctrl+G)");
    }

    [MenuItem("Tools/ȫ�����´�� %#g")]  // Ctrl+Shift+G (Windows) or Command+Shift+G (Mac)
    public static void BuildClearCache()
    {
        CopyHotDll.CreateDllByte();
        AddressableAssetSettings.CleanPlayerContent();
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("Addressables ȫ�����´����� (Ctrl+Shift+G)");
    }
}