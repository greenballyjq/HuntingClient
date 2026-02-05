using Cysharp.Threading.Tasks;
using UnityEngine;
using WeChatWASM;

public class VideoManager : IRoundManager
{
    /// <summary>
    /// 平台管理器
    /// </summary>
    private PlatformManager _platformManager => GameServiceLocator.GetFrameworkManager<PlatformManager>();

    /// <summary>
    /// 视频组件
    /// </summary>
    private WXVideo _video;

    public void Init(RoundContext context)
    {
        CreateVideo(context);
    }

    public void Dispose(){}

    public async UniTask PlayRoleCGAsync()
    {
        var completionSource = new UniTaskCompletionSource();

        _video.OnPlay(() =>
        {
            Debug.LogWarning("[VideoManager] 视频开始播放");
        });

        _video.OnEnded(() =>
        {
            _video.Destroy();
            _video = null;
            completionSource.TrySetResult();

            Debug.LogWarning("[VideoManager] 视频播放结束");
        });

        _video.x = 0;
        _video.y = 0;
        _video.Play();

        await completionSource.Task;
    }

    /// <summary>
    /// 创建视频组件
    /// </summary>
    /// <param name="context">单局上下文</param>
    private void CreateVideo(RoundContext context)
    {
        float beginTime = Time.time;
        Debug.LogWarning($"[VideoManager] 创建视频开始时间{beginTime}");
        Debug.LogWarning(Application.streamingAssetsPath + "/" + context.RoleData.VideoResourcePath);

        (_platformManager.CurrentPlatform as WeChatPlatform).WXCreateVideo(
           Application.streamingAssetsPath + "/" + context.RoleData.VideoResourcePath,
           null, 
           (video) =>
           {
               Debug.LogWarning("[VideoManager] 创建视频成功" + " " + video + " " + _video);
               Debug.LogWarning($"[VideoManager] 创建视频结束时间{Time.time}， 耗时{Time.time - beginTime}");
               _video = video;
           }
        );
    }
}
