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
    /// 三千盘金币组件
    /// </summary>
    [SerializeField] private UIComponentThreeKPCoin _uiComponentThreeKPCoin;
    public UIComponentThreeKPCoin UIComponentThreeKPCoin => _uiComponentThreeKPCoin;

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
    [SerializeField] private UIComponentTimer _uiComponentTime;

    /// <summary>
    /// 肉度条组件
    /// </summary>
    [SerializeField] private UIComponentMeatProgress _uiComponentMeatProgress;
    public UIComponentMeatProgress UIComponentMeatProgress => _uiComponentMeatProgress;

    /// <summary>
    /// 道具组组件
    /// </summary>
    [SerializeField] private UIComponentPropGroup _uiComponentPropGroup;

    /// <summary>
    /// 子弹组件
    /// </summary>
    [SerializeField] private UIComponentBullet _uiComponentBullet;
    public UIComponentBullet UIComponentBullet => _uiComponentBullet;

    /// <summary>
    /// 摇杆组件
    /// </summary>
    [SerializeField] private UIComponentJoystick _uiComponentJoystick;
    public UIComponentJoystick UIComponentJoystick => _uiComponentJoystick;

    /// <summary>
    /// 技能组件
    /// </summary>
    [SerializeField] private UIComponentSkill _uiComponentSkill;
    public UIComponentSkill UIComponentSkill => _uiComponentSkill;

    /// <summary>
    /// 掉落奖励光效组件
    /// </summary>
    [SerializeField] private UIComponentDropRewardLightEffect _uiComponentDropRewardLightEffect;

    /// <summary>
    /// 提示组件
    /// </summary>
    [SerializeField] private UIComponentTip _uiComponentTip;

    /// <summary>
    /// 背景图像控件
    /// </summary>
    [SerializeField] private Image _imageBackground;

    /// <summary>
    /// 主地图背景图
    /// </summary>
    [SerializeField] private Sprite _mainMapBackgroundSprite;

    /// <summary>
    /// 隐藏地图背景图
    /// </summary>
    [SerializeField] private Sprite _hiddenMapBackgroundSprite;

    /// <summary>
    /// 画布组组件
    /// </summary>
    private CanvasGroup _canvasGroup;

    /// <summary>
    /// 单局流程
    /// </summary>
    private RoundFlow _roundFlow;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _roundFlow = RoundFlow.Instance;
    }

    public override void OnClose()
    {
        CleanUpAll();
        SetAllComponentsActive(false);
        base.OnClose();
    }

    #region 公共方法
    /// <summary>
    /// 切换到指定玩法模式
    /// </summary>
    /// <param name="mode">玩法模式</param>
    public void SwitchToMode(EGameplayMode mode)
    {
        CleanUpAll();
        SetAllComponentsActive(false);

        var rule = _roundFlow.GetPlayRule<IUIGameplayComponentVisibilityRule>();
        InitAndShowByRule(rule);
    }

    /// <summary>
    /// 清理所有组件
    /// </summary>
    private void CleanUpAll()
    {
        _uiComponentReturnButton.CleanUp();
        _uiComponentThreeKPCoin.CleanUp();
        _uiComponentAnimalCounter.CleanUp();
        _uiComponentBossHealth.CleanUp();
        _uiComponentTime.CleanUp();
        _uiComponentMeatProgress.CleanUp();
        _uiComponentPropGroup.CleanUp();
        _uiComponentBullet.CleanUp();
        _uiComponentJoystick.CleanUp();
        _uiComponentSkill.CleanUp();
        _uiComponentDropRewardLightEffect.CleanUp();
        _uiComponentTip.CleanUp();
    }

    /// <summary>
    /// 设置所有组件的显隐状态
    /// </summary>
    /// <param name="active">是否激活</param>
    private void SetAllComponentsActive(bool active)
    {
        _uiComponentReturnButton.gameObject.SetActive(active);
        _uiComponentThreeKPCoin.gameObject.SetActive(active);
        _uiComponentAnimalCounter.gameObject.SetActive(active);
        _uiComponentBossHealth.gameObject.SetActive(active);
        _uiComponentTime.gameObject.SetActive(active);
        _uiComponentMeatProgress.gameObject.SetActive(active);
        _uiComponentPropGroup.gameObject.SetActive(active);
        _uiComponentBullet.gameObject.SetActive(active);
        _uiComponentJoystick.gameObject.SetActive(active);
        _uiComponentSkill.gameObject.SetActive(active);
        _uiComponentDropRewardLightEffect.gameObject.SetActive(active);
        _uiComponentTip.gameObject.SetActive(active);
    }

    /// <summary>
    /// 根据玩法规则初始化并显示组件
    /// </summary>
    /// <param name="rule">组件显隐规则</param>
    private void InitAndShowByRule(IUIGameplayComponentVisibilityRule rule)
    {
        _imageBackground.sprite = rule.UseHiddenMapBackground ? _hiddenMapBackgroundSprite : _mainMapBackgroundSprite;

        if (rule.ShowTime) { _uiComponentTime.Init(); _uiComponentTime.gameObject.SetActive(true); }
        if (rule.ShowMeatBar) { _uiComponentMeatProgress.Init(); _uiComponentMeatProgress.gameObject.SetActive(true); }
        if (rule.ShowAnimalCounter) { _uiComponentAnimalCounter.Init(); _uiComponentAnimalCounter.gameObject.SetActive(true); }
        if (rule.ShowBossHealth) { _uiComponentBossHealth.Init(); _uiComponentBossHealth.gameObject.SetActive(true); }
        if (rule.ShowReturnButton) { _uiComponentReturnButton.Init(); _uiComponentReturnButton.gameObject.SetActive(true); }
        if (rule.ShowThreeKPCoin) { _uiComponentThreeKPCoin.Init(); _uiComponentThreeKPCoin.gameObject.SetActive(true); }
        if (rule.ShowPropGroup) { _uiComponentPropGroup.Init(); _uiComponentPropGroup.gameObject.SetActive(true); }
        if (rule.ShowBullet) { _uiComponentBullet.Init(); _uiComponentBullet.gameObject.SetActive(true); }
        if (rule.ShowJoystick) { _uiComponentJoystick.Init(); _uiComponentJoystick.gameObject.SetActive(true); }
        if (rule.ShowSkill) { _uiComponentSkill.Init(); _uiComponentSkill.gameObject.SetActive(true); }
        if (rule.ShowDropRewardLightEffect) { _uiComponentDropRewardLightEffect.Init(this); _uiComponentDropRewardLightEffect.gameObject.SetActive(true); }
        if (rule.ShowTip) { _uiComponentTip.Init(); _uiComponentTip.gameObject.SetActive(true); }
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
