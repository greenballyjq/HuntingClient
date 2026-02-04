using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class UIComponentPrepareTest : MonoBehaviour, IUIComponent
{
    [SerializeField] private Button _buttonCreateVideoTest;

    private WXVideo _wxVideo;

    public void Init()
    {
       _buttonCreateVideoTest.onClick.AddListener(OnCreateVideoTestButtonClicked);
    }

    public void CleanUp()
    {
        _buttonCreateVideoTest.onClick.RemoveListener(OnCreateVideoTestButtonClicked);
    }  
    
    public void OnCreateVideoTestButtonClicked()
    {

        var systemInfo = GameServiceLocator.GetFrameworkManager<PlatformManager>().CurrentPlatform.GetSystemInfo();

        _wxVideo = WXBase.CreateVideo(new WXCreateVideoParam()
        {
            x = 0,
            y = 0,
            width = (int)systemInfo.ScreenWidth,
            height = (int)systemInfo.ScreenHeight,

            src = Application.streamingAssetsPath + "/YaKeDong.mp4",
            poster = null,

            initialTime = 0,
            playbackRate = 1f,

            controls = false,
            showProgress = false,
            showProgressInControlMode = false,

            autoplay = false,
            loop = false,
            muted = false,

            enableProgressGesture = false,
            enablePlayGesture = false,
            showCenterPlayBtn = false,

            objectFit = "cover",
            underGameView = false
        });

        _wxVideo.OnPlay(() =>
        {
            Debug.Log("开始播放");
        });

        _wxVideo.OnEnded(() =>
        {
            Debug.Log("播放结束");
        });

        _wxVideo.OnError(() =>
        {
            Debug.Log("错误");
            _wxVideo.Destroy();
        });

        _wxVideo.Play();
    }
}
