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
    // DLL���ã�0=����Ԫ����, 1=�ȸ��³���
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
        // ��ʼ��Addressables
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        // ���ظ��´���
        var windowHandle = patchWindowRef.LoadAssetAsync<GameObject>();
        yield return windowHandle;

        var windowObj = Instantiate(windowHandle.Result, GameManager.Inst.MainCanvasUI.transform);
        var patchWindow = windowObj.GetComponent<PatchWindow>();

        // ��ʼ�����£���ɺ�ص�LoadDll
        patchWindow.StartCheckUpdate(() => StartCoroutine(LoadDllAndStartGame()));
    }

    IEnumerator LoadDllAndStartGame()
    {
        // ��������DLL
        foreach (var dll in dllMap)
        {
            yield return LoadSingleDll(dll.Key, dll.Value);
        }

        // ���ز�������Ϸ
        yield return LoadGameStart();
    }

    IEnumerator LoadSingleDll(string dllName, int dllType)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(KeyManager.LoadDllKey(dllName));
        yield return handle;

        if (dllType == 0)
        {
            // ���ز���Ԫ����
            RuntimeApi.LoadMetadataForAOTAssembly(handle.Result.bytes, HomologousImageMode.SuperSet);
        }
        else
        {
#if UNITY_EDITOR
            // �༭��ģʽ��ֱ�ӻ�ȡ�Ѽ��صĳ���
            var hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies()
                .First(a => a.GetName().Name == "UnityScripts.HotUpdate");
#else
            // ����ʱ�����ȸ��³���
            var hotUpdateAss = Assembly.Load(handle.Result.bytes);
#endif
            Debug.Log($"�ɹ����� {dllName}");
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