using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WeChatWASM;

public class UIComponentPrepareTest : MonoBehaviour, IUIComponent
{
    [SerializeField] private Button _buttonCreateVideoTest;

    private WXVideo _wxVideo;

    private UIManager _uiManager => GameServiceLocator.UIManager;

    public void Init()
    {
       _buttonCreateVideoTest.onClick.AddListener(OnCreateVideoTestButtonClicked);

        var systemInfo = GameServiceLocator.GetFrameworkManager<PlatformManager>().CurrentPlatform.GetSystemInfo();

        _wxVideo = WXBase.CreateVideo(new WXCreateVideoParam()
        {
            width = (int)systemInfo.ScreenWidth,
            height = (int)systemInfo.ScreenHeight,

            src = Application.streamingAssetsPath + "/YaKeDongA.mp4",
            poster = null,

            autoplay = false,
            muted = false,

            objectFit = "cover",
            underGameView = true,

            controls = false,
            showProgress = false,
            showProgressInControlMode = false,
            enableProgressGesture = false,
            enablePlayGesture = false,
            showCenterPlayBtn = false,
        });

        _wxVideo.OnPlay(() =>
        {
            Debug.Log("视频开始播放");
        });

        _wxVideo.OnEnded(() =>
        {
            Debug.Log("视频播放结束");
            _wxVideo.Destroy();
        });

    }

    public void CleanUp()
    {
        _buttonCreateVideoTest.onClick.RemoveListener(OnCreateVideoTestButtonClicked);
    }  
    
    public async void OnCreateVideoTestButtonClicked()
    {
        await UniTask.DelayFrame(1);

        _uiManager.CloseUI("UIPrepare");

        SceneManager.LoadSceneAsync("VideoScene").completed += ((res) =>
        {
            Debug.Log("场景加载完成，开始播放视频");
            _wxVideo.Play();
        });

        
    }
}
