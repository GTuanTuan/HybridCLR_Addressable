using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class AddressablePathSetter
{
    public static void SetCustomPaths(string relativePath)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        // ���ù���·��
        settings.profileSettings.SetValue(
            settings.activeProfileId,
            "Remote.BuildPath",
            $"{Application.dataPath}/{relativePath}/[BuildTarget]");

        // ���ü���·��
        settings.profileSettings.SetValue(
            settings.activeProfileId,
            "Remote.LoadPath",
            $"{relativePath}/ServerData");
    }
}