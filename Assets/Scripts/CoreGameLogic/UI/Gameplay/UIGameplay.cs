using System;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游玩界面主面板
/// </summary>
public class UIGameplay : UIBase
{
    /// <summary>
    /// 返回/结算按钮组件
    /// </summary>
    [SerializeField] private UIComponentReturnButton _uiComponentReturnButton;
    public UIComponentReturnButton UIComponentReturnButton => _uiComponentReturnButton;

    /// <summary>
    /// 动物计数器组件
    /// </summary>
    [SerializeField] private UIComponentAnimalCounter _uiComponentAnimalCounter;

    /// <summary>
    /// Boss血量组件
    /// </summary>
    [SerializeField] private UIComponentBossHealth _uiComponentBossHealth;

    /// <summary>
    /// 时间显示组件
    /// </summary>
    [SerializeField] private UIComponentTime _uiComponentTime;

    /// <summary>
    /// 肉度条组件
    /// </summary>
    [SerializeField] private UIComponentMeatProgress _uiComponentMeatProgress;
    public UIComponentMeatProgress UIComponentMeatProgress => _uiComponentMeatProgress;

    /// <summary>
    /// 技能组件
    /// </summary>
    [SerializeField] private UIComponentSkill _uiComponentSkill;
    public UIComponentSkill UIComponentSkill => _uiComponentSkill;

    /// <summary>
    /// 子弹组件
    /// </summary>
    [SerializeField] private UIComponentBullet _uiComponentBullet;
    public UIComponentBullet UIComponentBullet => _uiComponentBullet;

    /// <summary>
    /// 道具组组件
    /// </summary>
    [SerializeField] private UIComponentPropGroup _uiComponentPropGroup;

    /// <summary>
    /// 道具使用提示组件
    /// </summary>
    [SerializeField] private UIComponentTip _uiComponentTip;

    /// <summary>
    /// 掉落奖励组件
    /// </summary>
    [SerializeField] private UIComponentDropReward _uiComponentDropReward;

    /// <summary>
    /// 背景图像
    /// </summary>
    [SerializeField] private Image _imageBackground;

    /// <summary>
    /// 雪山背景精灵图
    /// </summary>
    [SerializeField] private Sprite _snowBackgroundSprite;

    /// <summary>
    /// 画布组
    /// </summary>
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _uiComponentReturnButton.Init();
        _uiComponentBossHealth.Init();
        _uiComponentAnimalCounter.Init();
        _uiComponentTime.Init();
        _uiComponentMeatProgress.Init();
        _uiComponentSkill.Init();
        _uiComponentBullet.Init();
        _uiComponentPropGroup.Init();
        _uiComponentTip.Init();
        _uiComponentDropReward.Init(this);

        _uiComponentTip.gameObject.SetActive(false);
        _uiComponentBossHealth.gameObject.SetActive(false);
    }

    public override void OnClose()
    {
        _uiComponentReturnButton.CleanUp();
        _uiComponentAnimalCounter.CleanUp();
        _uiComponentBossHealth.CleanUp();
        _uiComponentTime.CleanUp();
        _uiComponentMeatProgress.CleanUp();
        _uiComponentSkill.CleanUp();
        _uiComponentBullet.CleanUp();
        _uiComponentPropGroup.CleanUp();
        _uiComponentTip.CleanUp();
        _uiComponentDropReward.CleanUp();
        base.OnClose();
    }

    #region 公共方法
    /// <summary>
    /// 切换到雪山背景
    /// </summary>
    public void SwitchSnow()
    {
        _imageBackground.sprite = _snowBackgroundSprite;

        _uiComponentAnimalCounter.CleanUp();

        _uiComponentAnimalCounter.gameObject.SetActive(false);
        _uiComponentReturnButton.gameObject.SetActive(false);
        _uiComponentBossHealth.gameObject.SetActive(true);
    }

    /// <summary>
    /// 设置是否可点击
    /// </summary>
    /// <param name="enable"></param>
    public void SetClickable(bool enable)
    {
        _canvasGroup.blocksRaycasts = enable;
    }

    /// <summary>
    /// 播放Boss血量增长动画
    /// </summary>
    public async UniTask PlayBossHealthIncreaseAnimationAsync()
    {
        await _uiComponentBossHealth.PlayBossHealthIncreaseAnimationAsync();
    }

    /// <summary>
    /// 播放提示动画
    /// </summary>
    /// <param name="duration">播放总时长（秒）</param>
    public async UniTask PlayTipAnimationAsync(string text, Color textColor, float duration)
    {
        _uiComponentTip.gameObject.SetActive(true);
        try
        {
            await _uiComponentTip.PlayTipAnimationAsync(text, textColor, duration);
            if (this != null && _uiComponentTip != null)
                _uiComponentTip.gameObject.SetActive(false);
        }
        catch (OperationCanceledException) { }
        catch (MissingReferenceException) { }
    }

    #endregion
}
