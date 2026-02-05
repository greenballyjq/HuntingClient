using System;
using UnityEngine;

public class VideoManager : IRoundManager
{
    /// <summary>
    /// 平台管理器
    /// </summary>
    private PlatformManager _platformManager => GameServiceLocator.GetFrameworkManager<PlatformManager>();

    /// <summary>
    /// 单局上下文
    /// </summary>
    private RoundContext _roundContext;

    public void Init(RoundContext context)
    {
        _roundContext = context;
    }

    public void Dispose(){}

    /// <summary>
    /// 播放角色CG
    /// </summary>
    public void PlayRoleCG(Action OnEnded)
    {
        (_platformManager.CurrentPlatform as WeChatPlatform).WXCreateVideo(Application.streamingAssetsPath + "/" + _roundContext.RoleData.VideoResourcePath, null,
        (video) =>
        {
            video.OnEnded(() =>
            {
                Debug.Log("视频播放结束");
                video.Destroy();
                video = null;
                Debug.Log("触发结束事件");
                OnEnded.Invoke();
            });
        });
    }
}
