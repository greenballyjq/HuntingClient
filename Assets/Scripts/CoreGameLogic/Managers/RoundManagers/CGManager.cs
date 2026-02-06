using Cysharp.Threading.Tasks;
using GameFramework.Game;
using UnityEngine;
using WeChatWASM;

/// <summary>
/// CG管理器
/// </summary>
public class CGManager : IAppManager
{
    /// <summary>
    /// 平台管理器
    /// </summary>
    private PlatformManager _platformManager => GameServiceLocator.GetFrameworkManager<PlatformManager>();

    /// <summary>
    /// CG播放完毕异步任务源
    /// </summary>
    private UniTaskCompletionSource CGEnded = new UniTaskCompletionSource();

    /// <summary>
    /// CG
    /// </summary>
    private WXVideo _cg;

    public void Init() {}

    public void Dispose(){}

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

        return cgCreated.Task;
    }
}
