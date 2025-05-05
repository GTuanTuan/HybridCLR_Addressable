using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

public class Boot : SingletonBehaviour<Boot>
{
    public AssetReferenceGameObject patchWindowRef;
    // DLL配置：0=补充元数据, 1=热更新程序集
    public Dictionary<string, int> dllMap = new Dictionary<string, int>()
    {
        { "mscorlib", 0 },
        { "System", 0 },
        { "System.Core", 0 },
        { "UnityScripts.HotUpdate", 1 },
    };

    void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        // 初始化Addressables
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        // 加载更新窗口
        var windowHandle = patchWindowRef.LoadAssetAsync<GameObject>();
        yield return windowHandle;

        var windowObj = Instantiate(windowHandle.Result, GameManager.Inst.MainCanvasUI.transform);
        var patchWindow = windowObj.GetComponent<PatchWindow>();

        // 开始检查更新，完成后回调LoadDll
        patchWindow.StartCheckUpdate(() => StartCoroutine(LoadDllAndStartGame()));
    }

    IEnumerator LoadDllAndStartGame()
    {
        // 加载所有DLL
        foreach (var dll in dllMap)
        {
            yield return LoadSingleDll(dll.Key, dll.Value);
        }

        // 加载并启动游戏
        yield return LoadGameStart();
    }

    IEnumerator LoadSingleDll(string dllName, int dllType)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(KeyManager.LoadDllKey(dllName));
        yield return handle;

        if (dllType == 0)
        {
            // 加载补充元数据
            RuntimeApi.LoadMetadataForAOTAssembly(handle.Result.bytes, HomologousImageMode.SuperSet);
        }
        else
        {
#if UNITY_EDITOR
            // 编辑器模式下直接获取已加载的程序集
            var hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies()
                .First(a => a.GetName().Name == "UnityScripts.HotUpdate");
#else
            // 运行时加载热更新程序集
            var hotUpdateAss = Assembly.Load(handle.Result.bytes);
#endif
            Debug.Log($"成功加载 {dllName}");
        }

        Addressables.Release(handle);
    }

    IEnumerator LoadGameStart()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(KeyManager.LoadPrefabKey("GameStart"));
        yield return handle;

        Instantiate(handle.Result);
        Addressables.Release(handle);
    }
}