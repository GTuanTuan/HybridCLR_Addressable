using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class AddressablePathSetter
{
    public static void SetCustomPaths(string relativePath)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        // 设置构建路径
        settings.profileSettings.SetValue(
            settings.activeProfileId,
            "Remote.BuildPath",
            $"{Application.dataPath}/{relativePath}/[BuildTarget]");

        // 设置加载路径
        settings.profileSettings.SetValue(
            settings.activeProfileId,
            "Remote.LoadPath",
            $"{relativePath}/ServerData");
    }
}