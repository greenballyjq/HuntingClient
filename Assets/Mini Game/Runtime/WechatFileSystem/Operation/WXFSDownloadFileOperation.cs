#if UNITY_WEBGL && WEIXINMINIGAME
// #if UNITY_WEBGL
using UnityEngine;
using UnityEngine.Networking;
using WeChatWASM;
using YooAsset;

internal class WXFSDownloadFileOperation : FSDownloadFileOperation
{
    protected enum ESteps
    {
        None,
        CreateRequest,
        CheckRequest,
        SaveToCache,
        TryAgain,
        Done,
    }

    private readonly WechatFileSystem _fileSystem;
    private readonly DownloadFileOptions _options;
    private UnityWebRequest _webRequest;
    private UnityWebRequestAsyncOperation _requestOp;
    private int _requestCount = 0;
    private float _tryAgainTimer;
    private int _failedTryAgain = 3;
    private ESteps _steps = ESteps.None;

    internal WXFSDownloadFileOperation(WechatFileSystem fileSystem, PackageBundle bundle, DownloadFileOptions options) : base(bundle)
    {
        _fileSystem = fileSystem;
        _options = options;
    }
    internal override void InternalStart()
    {
        UnityEngine.Debug.Log($"[WXFSDownloadFileOperation] 开始下载 Bundle: {Bundle.FileName}");
        _steps = ESteps.CreateRequest;
    }
    internal override void InternalUpdate()
    {
        // 创建下载器 - 使用普通 UnityWebRequest 真正下载文件内容
        if (_steps == ESteps.CreateRequest)
        {
            string url = GetRequestURL();
            UnityEngine.Debug.Log($"[WXFSDownloadFileOperation] 创建下载请求\nBundle: {Bundle.FileName}\nURL: {url}\n文件大小: {Bundle.FileSize / 1024f:F1}KB");

            _webRequest = UnityWebRequest.Get(url);
            _webRequest.downloadHandler = new DownloadHandlerBuffer();
            _requestOp = _webRequest.SendWebRequest();
            _steps = ESteps.CheckRequest;
        }

        // 检测下载结果
        if (_steps == ESteps.CheckRequest)
        {
            Progress = _requestOp.progress;
            DownloadProgress = _webRequest.downloadProgress;
            DownloadedBytes = (long)_webRequest.downloadedBytes;

            if (_requestOp.isDone == false)
                return;

            if (_webRequest.result == UnityWebRequest.Result.Success)
            {
                byte[] data = _webRequest.downloadHandler.data;
                if (data != null && data.Length > 0)
                {
                    UnityEngine.Debug.Log($"[WXFSDownloadFileOperation] 下载完成，保存到缓存\nBundle: {Bundle.FileName}\n下载大小: {data.Length} 字节");
                    _steps = ESteps.SaveToCache;
                }
                else
                {
                    UnityEngine.Debug.LogError($"[WXFSDownloadFileOperation] ❌ 下载数据为空: {Bundle.FileName}");
                    HandleFailure($"Downloaded data is empty for {Bundle.FileName}");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"[WXFSDownloadFileOperation] ❌ 下载失败: {Bundle.FileName}\nHTTP: {_webRequest.responseCode}\n错误: {_webRequest.error}");
                HandleFailure(_webRequest.error);
            }
        }

        // 保存到微信文件系统缓存
        if (_steps == ESteps.SaveToCache)
        {
            try
            {
                byte[] data = _webRequest.downloadHandler.data;
                string cacheFilePath = _fileSystem.GetCacheFileLoadPath(Bundle);

                // 确保目录存在
                string dir = cacheFilePath.Substring(0, cacheFilePath.LastIndexOf('/'));
                EnsureDirectoryExists(dir);

                // 写入文件
                var fs = _fileSystem.GetFileSystemMgr();
                fs.WriteFileSync(cacheFilePath, data);

                UnityEngine.Debug.Log($"[WXFSDownloadFileOperation] ✅ Bundle 下载并缓存成功: {Bundle.FileName}\n缓存路径: {cacheFilePath}\n大小: {data.Length} 字节");

                DisposeRequest();
                _steps = ESteps.Done;
                Status = EOperationStatus.Succeed;
                DownloadProgress = 1f;
                DownloadedBytes = Bundle.FileSize;
                Progress = 1f;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[WXFSDownloadFileOperation] ❌ 保存缓存失败: {Bundle.FileName}\n错误: {ex.Message}");
                HandleFailure($"Save cache failed: {ex.Message}");
            }
        }

        // 重新尝试下载
        if (_steps == ESteps.TryAgain)
        {
            _tryAgainTimer += Time.unscaledDeltaTime;
            if (_tryAgainTimer > 1f)
            {
                _tryAgainTimer = 0f;
                _failedTryAgain--;
                Progress = 0f;
                DownloadProgress = 0f;
                DownloadedBytes = 0;
                DisposeRequest();
                _steps = ESteps.CreateRequest;
            }
        }
    }

    private void HandleFailure(string error)
    {
        DisposeRequest();
        if (_failedTryAgain > 0)
        {
            _steps = ESteps.TryAgain;
            UnityEngine.Debug.LogWarning($"[WXFSDownloadFileOperation] ⚠️ 准备重试 ({_failedTryAgain}): {Bundle.FileName}");
        }
        else
        {
            _steps = ESteps.Done;
            Status = EOperationStatus.Failed;
            Error = error;
            UnityEngine.Debug.LogError($"[WXFSDownloadFileOperation] ❌ 最终失败: {Bundle.FileName}\n错误: {error}");
        }
    }

    private void EnsureDirectoryExists(string dirPath)
    {
        try
        {
            var fs = _fileSystem.GetFileSystemMgr();
            fs.MkdirSync(dirPath, true);
        }
        catch (System.Exception)
        {
            // 目录可能已存在，忽略
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

    /// <summary>
    /// 获取网络请求地址
    /// </summary>
    private string GetRequestURL()
    {
        // 轮流返回请求地址
        _requestCount++;
        if (_requestCount % 2 == 0)
            return _options.FallbackURL;
        else
            return _options.MainURL;
    }
}
#endif
