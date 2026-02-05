using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WeChatWASM;

public class UIComponentPrepareTest : MonoBehaviour, IUIComponent
{
    [SerializeField] private Button _buttonCreateVideoTest;

    private PlatformManager _platformManager => GameServiceLocator.GetFrameworkManager<PlatformManager>();


    private WXVideo _wxVideo;

    public void Init()
    {
       _buttonCreateVideoTest.onClick.AddListener(OnCreateVideoTestButtonClicked);

        float beginTime = Time.time;
        Debug.LogWarning($"[UIComponentPrepareTest] 创建视频开始时间{beginTime}");

        Debug.LogWarning(Application.streamingAssetsPath + "/DaMeiLi.mp4");

        (_platformManager.CurrentPlatform as WeChatPlatform).WXCreateVideo(Application.streamingAssetsPath + "/YaKeDong.mp4", null, (video) =>
        {
            _wxVideo = video;

            Debug.LogWarning("[UIComponentPrepareTest] 创建视频成功" + " " + video + " " + _wxVideo);

            Debug.LogWarning($"[UIComponentPrepareTest] 创建视频结束时间{Time.time}， 耗时{Time.time - beginTime}");

            video.OnPlay(() =>
            {
                Debug.LogWarning("[UIComponentPrepareTest] 视频开始播放");
            });

            

            video.OnEnded(() =>
            {
                Debug.LogWarning("[UIComponentPrepareTest] 视频播放结束");
                video.Destroy();
                Debug.LogWarning("[UIComponentPrepareTest] 视频销毁");
            });
        });
        
    }

    public void CleanUp()
    {
        _buttonCreateVideoTest.onClick.RemoveListener(OnCreateVideoTestButtonClicked);
    }  
    
    public async void OnCreateVideoTestButtonClicked()
    {
        Debug.LogWarning("[UIComponentPrepareTest] 调用视频播放");

        await UniTask.Yield();

        _wxVideo.x = 0;
        _wxVideo.y = 0;

        _wxVideo.Play();

        
    }
}
