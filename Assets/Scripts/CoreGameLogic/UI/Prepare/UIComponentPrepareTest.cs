using GameFramework.Core.UI;
using UnityEngine;
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

        (_platformManager.CurrentPlatform as WeChatPlatform).WXCreateVideo(Application.streamingAssetsPath + "/YaKeDong.mp4", null, (video) =>
        {
            _wxVideo = video;
            video.OnEnded(() =>
            {
                video.Destroy();
            });
        });
        
    }

    public void CleanUp()
    {
        _buttonCreateVideoTest.onClick.RemoveListener(OnCreateVideoTestButtonClicked);
    }  
    
    public void OnCreateVideoTestButtonClicked()
    {
        _wxVideo.x = 0;
        _wxVideo.y = 0;

        _wxVideo.Play();
    }
}
