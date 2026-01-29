#if UNITY_WEBGL && WEIXINMINIGAME
// #if UNITY_WEBGL
using UnityEngine.Networking;
using UnityEngine;
using WeChatWASM;

namespace YooAsset
{
    internal class UnityWechatAssetBundleRequestOperation : UnityWebRequestOperation
    {
        protected enum ESteps
        {
            None,
            CreateRequest,
            Download,
            Done,
        }

        private readonly PackageBundle _packageBundle;
        private UnityWebRequestAsyncOperation _requestOperation;
        private ESteps _steps = ESteps.None;

        /// <summary>
        /// 请求结果
        /// </summary>
        public AssetBundle Result { private set; get; }

        internal UnityWechatAssetBundleRequestOperation(PackageBundle bundle, string url) : base(url)
        {
            _packageBundle = bundle;
        }
        internal override void InternalStart()
        {
            _steps = ESteps.CreateRequest;
        }
        internal override void InternalUpdate()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.CreateRequest)
            {
                UnityEngine.Debug.Log($"[UnityWechatAssetBundleRequestOperation] 🌐 开始从 CDN 下载 Bundle\nBundle: {_packageBundle.FileName}\nURL: {_requestURL}\n预期大小: {_packageBundle.FileSize} 字节");
                CreateWebRequest();
                _steps = ESteps.Download;
            }

            if (_steps == ESteps.Download)
            {
                DownloadProgress = _webRequest.downloadProgress;
                DownloadedBytes = (long)_webRequest.downloadedBytes;
                Progress = _requestOperation.progress;
                
                // 定期打印下载进度
                if (_requestOperation.isDone == false)
                {
                    if (DownloadedBytes > 0 && (int)(DownloadProgress * 100) % 25 == 0)
                    {
                        UnityEngine.Debug.Log($"[UnityWechatAssetBundleRequestOperation] 📥 下载进度: {_packageBundle.FileName} - {DownloadProgress:P0} ({DownloadedBytes} / {_packageBundle.FileSize} 字节)");
                    }
                    return;
                }

                UnityEngine.Debug.Log($"[UnityWechatAssetBundleRequestOperation] 下载请求完成\nBundle: {_packageBundle.FileName}\n已下载: {DownloadedBytes} 字节\nHTTP 状态: {_webRequest.responseCode}\n是否错误: {(_webRequest.result != UnityWebRequest.Result.Success)}");

                if (CheckRequestResult())
                {
                    var downloadHanlder = (DownloadHandlerWXAssetBundle)_webRequest.downloadHandler;
                    AssetBundle assetBundle = downloadHanlder.assetBundle;
                    if (assetBundle == null)
                    {
                        _steps = ESteps.Done;
                        Status = EOperationStatus.Failed;
                        Error = $"URL : {_requestURL} Download handler asset bundle object is null !";
                        UnityEngine.Debug.LogError($"[UnityWechatAssetBundleRequestOperation] ❌ AssetBundle 为 null\nBundle: {_packageBundle.FileName}\nURL: {_requestURL}\nHTTP 状态: {_webRequest.responseCode}\n已下载字节: {DownloadedBytes}\n可能原因:\n1. CDN 上文件不存在 (404)\n2. 文件格式不是有效的 AssetBundle\n3. 文件已损坏\n4. CORS 配置问题");
                    }
                    else
                    {
                        _steps = ESteps.Done;
                        Result = assetBundle;
                        Status = EOperationStatus.Succeed;

                        UnityEngine.Debug.Log($"[UnityWechatAssetBundleRequestOperation] ✅ Bundle 下载并加载成功\nBundle: {_packageBundle.FileName}\n大小: {DownloadedBytes} 字节\nAssetBundle 名称: {assetBundle.name}");

                        //TODO 解决微信小游戏插件问题
                        // Issue : https://github.com/wechat-miniprogram/minigame-unity-webgl-transform/issues/108#
                        DownloadProgress = 1f;
                        DownloadedBytes = _packageBundle.FileSize;
                        Progress = 1f;
                    }
                }
                else
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    UnityEngine.Debug.LogError($"[UnityWechatAssetBundleRequestOperation] ❌ Web 请求失败\nBundle: {_packageBundle.FileName}\nURL: {_requestURL}\nHTTP 状态: {_webRequest.responseCode}\n错误: {_webRequest.error}\n结果: {_webRequest.result}");
                }

                // 注意：最终释放请求器
                DisposeRequest();
            }
        }

        private void CreateWebRequest()
        {
            _webRequest = WXAssetBundle.GetAssetBundle(_requestURL);
            _webRequest.disposeDownloadHandlerOnDispose = true;
            _requestOperation = _webRequest.SendWebRequest();
        }
    }
}
#endif