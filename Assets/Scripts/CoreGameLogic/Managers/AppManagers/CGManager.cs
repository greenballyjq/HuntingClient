using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GameFramework.Manager;
using UnityEngine;
using WeChatWASM;

/// <summary>
/// CG管理器
/// </summary>
public class CGManager : IAppManager
{
    /// <summary>
    /// CG播放完毕异步任务源
    /// </summary>
    private UniTaskCompletionSource CGEnded;

    /// <summary>
    /// CG
    /// </summary>
    private WXVideo _cg;

    private PlatformManager _platformManager;

    public void Init()
    {
        RegisterServices();
    }

    public void Dispose() { }

    #region 私有方法
    private void RegisterServices()
    {
        _platformManager = GameServiceLocator.GetFrameworkManager<PlatformManager>();
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 播放CG
    /// </summary>
    public async UniTask PlayCGAsync()
    {
        _cg.x = 0;
        _cg.y = 0;

        _cg.Play();

        await CGEnded.Task;
    }

    /// <summary>
    /// 创建CG
    /// </summary>
    /// <param name="src">CG路径</param>
    public UniTask CreateCGAsync(string src)
    {
        var cgCreated = new UniTaskCompletionSource<WXVideo>();

        (_platformManager.CurrentPlatform as WeChatPlatform).WXCreateVideo(Application.streamingAssetsPath + "/" + src, null, (cg) =>
        {
            _cg = cg;

            cg.OnEnded(() =>
            {
                cg.Destroy();
                cg.OffEnded();
                CGEnded.TrySetResult();
            });

            cgCreated.TrySetResult(cg);
        });

        CGEnded = null;
        CGEnded = new UniTaskCompletionSource();

        return cgCreated.Task;
    }
    #endregion
}
