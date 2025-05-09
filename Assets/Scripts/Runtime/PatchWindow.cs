using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
public class PatchWindow : MonoBehaviour
{
    public Text statusText;
    public Slider progressBar;
    public Text downloadSizeText;
    public VideoPlayer video;
    public Button actionButton;
    public Text buttonText;

    private bool hasUpdate;
    private bool updateCompleted;
    private System.Action onUpdateComplete;

    private void Awake()
    {
        video.targetCamera = GameManager.Inst.UICamera;
        actionButton.onClick.AddListener(OnActionButtonClick);
    }
    public void StartCheckUpdate(System.Action callback)
    {
        this.onUpdateComplete = callback;
        if (GameManager.Inst.HasNetwork())
        {
            StartCoroutine(CheckForCatalogUpdates());
        }
        else
        {
            StartCoroutine(ReadyStart("无网络，使用本地资源"));
        }
    }
    IEnumerator CheckForCatalogUpdates()
    {
        statusText.text = "检测清单更新中...";
        var catalogHandle = Addressables.CheckForCatalogUpdates(false);
        yield return catalogHandle;
        if (catalogHandle.Status != AsyncOperationStatus.Succeeded)
        {
            StartCoroutine(ReadyStart("检测清单更新失败，使用本地资源"));
            yield break;
        }
        else
        {
            var catalogs = catalogHandle.Result;
            if (catalogs.Count > 0)
            {
                StartCoroutine(UpdateCatalogs(catalogs));
            }
            else
            {
                StartCoroutine(CheckNeedDownLoad());
            }
        }
    }
    IEnumerator UpdateCatalogs(List<string> catalogs)
    {
        statusText.text = "清单更新中...";
        var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
        yield return updateHandle;
        if (updateHandle.Status != AsyncOperationStatus.Succeeded)
        {
            StartCoroutine(ReadyStart("清单更新失败，使用本地资源"));
            Addressables.Release(updateHandle);
            yield break;
        }
        else
        {
            StartCoroutine(CheckNeedDownLoad());
        }
        Addressables.Release(updateHandle);
    }
    IEnumerator CheckNeedDownLoad()
    {
        statusText.text = "检测资源更新中";
        var sizeHandle = Addressables.GetDownloadSizeAsync("preload");
        yield return sizeHandle;
        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            StartCoroutine(ReadyStart("资源更新检查失败，使用本地资源"));
            yield break;
        }
        else
        {
            long downloadSize = sizeHandle.Result;
            if(downloadSize > 0)
            {
                hasUpdate = true;
                statusText.text = "有可用更新";
                downloadSizeText.text = $"需要下载: {downloadSize / 1024f / 1024f:F1}MB";
                buttonText.text = "下载更新";
                actionButton.gameObject.SetActive(true);
            }
            else
            {
                StartCoroutine(ReadyStart());
            }
        }
    }
    private IEnumerator Download()
    {
        statusText.text = "下载更新中...";
        progressBar.gameObject.SetActive(true);
        var downloadHandle = Addressables.DownloadDependenciesAsync("preload");
        while (!downloadHandle.IsDone)
        {
            float smoothProgress = Mathf.Lerp(progressBar.value, downloadHandle.PercentComplete, Time.deltaTime * 5f);
            progressBar.value = smoothProgress;
            var status = downloadHandle.GetDownloadStatus();
            downloadSizeText.text = $"{status.DownloadedBytes / 1024f / 1024f:F1}MB / {status.TotalBytes / 1024f / 1024f:F1}MB";
            yield return null;
        }
        if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            StartCoroutine(ReadyStart("下载更新失败，使用本地资源"));
            yield break;
        }
        else
        {
            StartCoroutine(ReadyStart());
        }
        Addressables.Release(downloadHandle);
    }
    IEnumerator ReadyStart(string text = "更新完成!")
    {
        yield return null;
        statusText.text = text;
        progressBar.value = 1f;
        updateCompleted = true;
        buttonText.text = "进入游戏";
        actionButton.gameObject.SetActive(true);
    }

    private void OnActionButtonClick()
    {
        if (hasUpdate && !updateCompleted)
        {
            StartCoroutine(Download());
        }
        else
        {
            onUpdateComplete?.Invoke();
            gameObject.SetActive(false);
        }
        actionButton.gameObject.SetActive(false);
    }
}