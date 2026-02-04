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
        _wxVideo = WXBase.CreateVideo(new WXCreateVideoParam()
        {
            x = 0,
            y = 0,

            width = 1920,
            height = 1080,

            src = Application.streamingAssetsPath + "/YaKeDong.mp4",
            poster = Application.streamingAssetsPath + "/YaKeDong.jpg",

            initialTime = 0,
            playbackRate = 1f,
            live = false,
            controls = false,
            showProgress = false,
            showProgressInControlMode = false,

            autoplay = false,
            loop = false,
            muted = false,
            enablePlayGesture = false,
            enableProgressGesture = false,
            showCenterPlayBtn = false,

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

        _wxVideo.Play();
    }
}
