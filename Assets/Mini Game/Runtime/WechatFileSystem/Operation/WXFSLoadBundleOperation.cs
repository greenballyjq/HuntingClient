#if UNITY_WEBGL && WEIXINMINIGAME
// #if UNITY_WEBGL
using UnityEngine;
using UnityEngine.Networking;
using YooAsset;
using WeChatWASM;

internal class WXFSLoadBundleOperation : FSLoadBundleOperation
{
    private enum ESteps
    {
        None,
        DownloadBundle,
        CheckDownload,
        SaveToCache,
        LoadFromCache,
        CheckLoad,
        Done,
    }

    private readonly WechatFileSystem _fileSystem;
    private readonly PackageBundle _bundle;
    private UnityWebRequest _webRequest;
    private UnityWebRequestAsyncOperation _requestOp;
    private AssetBundleCreateRequest _loadRequest;
    private ESteps _steps = ESteps.None;

    internal WXFSLoadBundleOperation(WechatFileSystem fileSystem, PackageBundle bundle)
    {
        _fileSystem = fileSystem;
        _bundle = bundle;
    }
    internal override void InternalStart()
    {
        // UnityEngine.Debug.Log($"[WXFSLoadBundleOperation] 开始加载 Bundle: {_bundle.FileName}");

        // 检查本地缓存是否存在
        string cacheFilePath = _fileSystem.GetCacheFileLoadPath(_bundle);
        bool cached = _fileSystem.CheckCacheFileExist(cacheFilePath);

        if (cached)
        {
            // UnityEngine.Debug.Log($"[WXFSLoadBundleOperation] 本地缓存已存在，直接加载: {_bundle.FileName}");
            _steps = ESteps.LoadFromCache;
        }
        else
        {
            // UnityEngine.Debug.Log($"[WXFSLoadBundleOperation] 本地缓存不存在，先下载: {_bundle.FileName}");
            _steps = ESteps.DownloadBundle;
        }
    }
    internal override void InternalUpdate()
    {
        if (_steps == ESteps.None || _steps == ESteps.Done)
            return;

        // 步骤1：使用 UnityWebRequest 真正下载文件
        if (_steps == ESteps.DownloadBundle)
        {
            string mainURL = _fileSystem.RemoteServices.GetRemoteMainURL(_bundle.FileName);
            // UnityEngine.Debug.Log($"[WXFSLoadBundleOperation] 开始下载 Bundle\nBundle: {_bundle.FileName}\nURL: {mainURL}");

            _webRequest = UnityWebRequest.Get(mainURL);
            _webRequest.downloadHandler = new DownloadHandlerBuffer();
            _requestOp = _webRequest.SendWebRequest();
            _steps = ESteps.CheckDownload;
        }

        // 步骤2：检查下载结果
        if (_steps == ESteps.CheckDownload)
        {
            Progress = _requestOp.progress * 0.4f;
            DownloadProgress = _webRequest.downloadProgress;
            DownloadedBytes = (long)_webRequest.downloadedBytes;

            if (_requestOp.isDone == false)
                return;

            if (_webRequest.result == UnityWebRequest.Result.Success)
            {
                byte[] data = _webRequest.downloadHandler.data;
                if (data != null && data.Length > 0)
                {
                    // UnityEngine.Debug.Log($"[WXFSLoadBundleOperation] 下载完成: {_bundle.FileName} ({data.Length} 字节)");
                    _steps = ESteps.SaveToCache;
                }
                else
                {
                    // UnityEngine.Debug.LogError($"[WXFSLoadBundleOperation] 下载数据为空: {_bundle.FileName}");
                    DisposeRequest();
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = $"Downloaded data is empty for {_bundle.FileName}";
                }
            }
            else
            {
                // UnityEngine.Debug.LogError($"[WXFSLoadBundleOperation] 下载失败: {_bundle.FileName}\nHTTP: {_webRequest.responseCode}\n错误: {_webRequest.error}");
                DisposeRequest();
                _steps = ESteps.Done;
                Status = EOperationStatus.Failed;
                Error = _webRequest.error;
            }
        }

        // 步骤3：保存到微信文件系统缓存
        if (_steps == ESteps.SaveToCache)
        {
            try
            {
                byte[] data = _webRequest.downloadHandler.data;
                string cacheFilePath = _fileSystem.GetCacheFileLoadPath(_bundle);

                // 确保目录存在
                string dir = cacheFilePath.Substring(0, cacheFilePath.LastIndexOf('/'));
                try
                {
                    var fs = _fileSystem.GetFileSystemMgr();
                    fs.MkdirSync(dir, true);
                }
                catch (System.Exception)
                {
                    // 目录可能已存在
                }

                // 写入文件
                _fileSystem.GetFileSystemMgr().WriteFileSync(cacheFilePath, data);
                // UnityEngine.Debug.Log($"[WXFSLoadBundleOperation] 缓存保存成功: {_bundle.FileName} -> {cacheFilePath}");

                DisposeRequest();
                _steps = ESteps.LoadFromCache;
            }
            catch (System.Exception ex)
            {
                // UnityEngine.Debug.LogError($"[WXFSLoadBundleOperation] 保存缓存失败: {_bundle.FileName}\n错误: {ex.Message}");
                DisposeRequest();
                _steps = ESteps.Done;
                Status = EOperationStatus.Failed;
                Error = $"Save cache failed: {ex.Message}";
            }
        }

        // 步骤4：从本地缓存读取字节并加载 AssetBundle
        if (_steps == ESteps.LoadFromCache)
        {
            try
            {
                string cacheFilePath = _fileSystem.GetCacheFileLoadPath(_bundle);
                byte[] bundleData = _fileSystem.GetFileSystemMgr().ReadFileSync(cacheFilePath);

                if (bundleData == null || bundleData.Length == 0)
                {
                    // UnityEngine.Debug.LogError($"[WXFSLoadBundleOperation] 缓存文件读取为空: {_bundle.FileName}");
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = $"Cache file is empty: {_bundle.FileName}";
                    return;
                }

                // UnityEngine.Debug.Log($"[WXFSLoadBundleOperation] 从缓存加载 AssetBundle: {_bundle.FileName} ({bundleData.Length} 字节)");
                _loadRequest = AssetBundle.LoadFromMemoryAsync(bundleData);
                _steps = ESteps.CheckLoad;
            }
            catch (System.Exception ex)
            {
                // UnityEngine.Debug.LogError($"[WXFSLoadBundleOperation] 读取缓存失败: {_bundle.FileName}\n错误: {ex.Message}");
                _steps = ESteps.Done;
                Status = EOperationStatus.Failed;
                Error = $"Read cache failed: {ex.Message}";
            }
        }

        // 步骤5：检查 AssetBundle 加载结果
        if (_steps == ESteps.CheckLoad)
        {
            Progress = 0.5f + _loadRequest.progress * 0.5f;

            if (_loadRequest.isDone == false)
                return;

            AssetBundle assetBundle = _loadRequest.assetBundle;
            if (assetBundle != null)
            {
                // UnityEngine.Debug.Log($"[WXFSLoadBundleOperation] Bundle 加载成功: {_bundle.FileName}");
                _steps = ESteps.Done;
                Result = new WXAssetBundleResult(_fileSystem, _bundle, assetBundle);
                Status = EOperationStatus.Succeed;
            }
            else
            {
                // UnityEngine.Debug.LogError($"[WXFSLoadBundleOperation] AssetBundle.LoadFromMemory 失败: {_bundle.FileName}");
                _steps = ESteps.Done;
                Status = EOperationStatus.Failed;
                Error = $"Failed to load AssetBundle from memory: {_bundle.FileName}";
            }
        }
    }

    private void DisposeRequest()
    {
        if (_webRequest != null)
        {
            _webRequest.Dispose();
            _webRequest = null;
        }
    }

    internal override void InternalWaitForAsyncComplete()
    {
        if (_steps != ESteps.Done)
        {
            _steps = ESteps.Done;
            Status = EOperationStatus.Failed;
            Error = "WebGL platform not support sync load method !";
            UnityEngine.Debug.LogError(Error);
        }
    }
}
#endif
