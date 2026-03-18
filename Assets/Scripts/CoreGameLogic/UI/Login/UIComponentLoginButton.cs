using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameFramework.Core;
using GameFramework.Utility;
using CoreGameLogic.Net;

/// <summary>
/// 登录按钮组件
/// </summary>
public class UIComponentLoginButton : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 闪烁动画配置
    /// </summary>
    [Serializable]
    private class BlinkConfig
    {
        /// <summary>
        /// 闪烁次数
        /// </summary>
        public int BlinkCount = 3;

        /// <summary>
        /// 单次间隔时长（秒）
        /// </summary>
        public float IntervalDuration = 0.5f;

        /// <summary>
        /// 目标透明度
        /// </summary>
        [Range(0f, 1f)]
        public float TargetOpacity = 0.5f;

        /// <summary>
        /// 是否使用未缩放时间
        /// </summary>
        public bool UseUnscaledTime;
    }

    /// <summary>
    /// 登录按钮
    /// </summary>
    [SerializeField]
    private Button _buttonLogin;

    /// <summary>
    /// UI摄像机
    /// </summary>
    [SerializeField] private Camera _uiCamera;
    
    /// <summary>
    /// 闪烁配置
    /// </summary>
    [SerializeField]
    private BlinkConfig _blinkConfig;
    
    /// <summary>
    /// 原始透明度
    /// </summary>
    private float _originAlpha = 1;

    /// <summary>
    /// 画布组件
    /// </summary>
    private CanvasGroup _canvasGroup;

    private PlatformManager _platformManager;
    private PlayerDataManager _playerDataManager;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init()
    {
        _buttonLogin.onClick.AddListener(OnLoginButtonClicked);

        _platformManager = GameServiceLocator.PlatformManager;
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();

    
    }

    public void CleanUp()
    {
        _buttonLogin.onClick.RemoveListener(OnLoginButtonClicked);
    }

    public async UniTask PlayBlinkAsync()
    {
        float stepDuration = _blinkConfig.IntervalDuration / 2f;
        float targetAlpha = _blinkConfig.TargetOpacity;

        Sequence blinkSequence = DOTween.Sequence();

        for (int i = 0; i < _blinkConfig.BlinkCount; i++)
        {
            blinkSequence.Append(_canvasGroup.DOFade(targetAlpha, stepDuration));
            blinkSequence.Append(_canvasGroup.DOFade(_originAlpha, stepDuration));
        }

        blinkSequence.SetUpdate(_blinkConfig.UseUnscaledTime);

        blinkSequence.OnComplete(() =>
        {
            _canvasGroup.alpha = _originAlpha;
        });

        blinkSequence.Play();

        await blinkSequence.AsyncWaitForCompletion();
    }

    /// <summary>
    /// 登录按钮点击回调
    /// </summary>
    private async void OnLoginButtonClicked()
    {
        // TODO: 在此处实现登录逻辑
        _buttonLogin.targetGraphic.raycastTarget = false;

        

        await PlayBlinkAsync();

        await HuntingAppFlow.Instance.EnterPrepareAsync();

        GameFrameLauncher.Instance.HideLoading();
    }

    /// <summary>
    /// 游戏后端登录
    /// </summary>
    protected void GameLogin()
    {
        HuntingGameServiceProxy.Instance.InitNetwork();
        Debug.Log($"[HuntingAppFlow] 开始游戏后端登录");
        IPlatform platform = _platformManager.CurrentPlatform;
        platform.Login(async code =>
        {
            Debug.Log($"[HuntingAppFlow] 平台登录成功：{code}");
            await HuntingGameServiceProxy.Instance.Login(_platformManager.GetCurrentPlatform(), code);

            Debug.Log($"[UIComponentLoginButton] 开始检测用户是否授权");
            // 检测用户是否授权
            _platformManager.CurrentPlatform.CheckUserAuthorization(result =>
            {
                if (!result)
                {
                    Debug.Log($"[UIComponentLoginButton] 用户未授权");
                    // 没有授权，创建授权按钮
                    RectTransform loginButtonRectTransform = _buttonLogin.GetComponent<RectTransform>();
                    WeChatPlatform.WechatPoint wechatPoint = PlatformRectTransformScreenPointTransformer
                        .RectTransformToWechatCreateUserInfoButtonPoint(loginButtonRectTransform, _uiCamera);
                    _platformManager.CurrentPlatform.CreateUserInfoButton(wechatPoint.x, wechatPoint.y, wechatPoint.width,
                        wechatPoint.height,
                        userInfoResult =>
                        {
                            Debug.Log($"[UIComponentLoginButton] 用户按下授权按钮: {userInfoResult.AvatarUrl}, {userInfoResult.NickName}");
                            PlatformUserInfo platformUserInfo = new PlatformUserInfo
                            {
                                NickName = userInfoResult.NickName,
                                AvatarUrl = userInfoResult.AvatarUrl,
                                Gender = userInfoResult.Gender,
                                Province = userInfoResult.Province,
                                City = userInfoResult.City,
                                Language = userInfoResult.Language
                            };
                            _playerDataManager.SetPlatformUserInfo(platformUserInfo);
                        });
                }
                else
                {
                    Debug.Log($"[UIComponentLoginButton] 用户已授权");
                    // 提醒 PlayerDataManager 进行平台用户信息拉取
                    _playerDataManager.SyncPlatformUserInfo();
                }
            }, error =>
            {

            });

        }, error =>
        {
            Debug.Log($"[HuntingAppFlow] 平台登录失败：{error}");
        });


    }
}
