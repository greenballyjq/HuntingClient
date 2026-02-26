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
    [SerializeField] private UIComponentButtonReturnOrSettlement _uiComponentReturnOrSettlement;
    public UIComponentButtonReturnOrSettlement UIComponentReturnOrSettlement => _uiComponentReturnOrSettlement;

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
    [SerializeField] private UIComponentPropUseTip _uiComponentPropUseTip;

    /// <summary>
    /// 掉落奖励组件
    /// </summary>
    [SerializeField] private UIComponentDropReward _uiComponentDropReward;

    /// <summary>
    /// 雪山胜利光效组件
    /// </summary>
    [SerializeField] private UIComponentSnowMountainVictory _uiComponentSnowMountainVictory;

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
        _uiComponentReturnOrSettlement.Init();
        _uiComponentBossHealth.Init();
        _uiComponentBossHealth.gameObject.SetActive(false);
        _uiComponentAnimalCounter.Init();
        _uiComponentTime.Init();
        _uiComponentMeatProgress.Init();
        _uiComponentSkill.Init();
        _uiComponentBullet.Init();
        _uiComponentPropGroup.Init();
        _uiComponentPropUseTip.Init();
        _uiComponentPropUseTip.gameObject.SetActive(false);
        _uiComponentDropReward.Init(this);
        _uiComponentSnowMountainVictory.Init();
        _uiComponentSnowMountainVictory.gameObject.SetActive(false);

        //uiComponentQuest.Init();
    }

    public override void OnClose()
    {
        _uiComponentReturnOrSettlement.CleanUp();
        _uiComponentAnimalCounter.CleanUp();
        _uiComponentBossHealth.CleanUp();
        _uiComponentTime.CleanUp();
        _uiComponentMeatProgress.CleanUp();
        _uiComponentSkill.CleanUp();
        _uiComponentBullet.CleanUp();
        _uiComponentPropGroup.CleanUp();
        _uiComponentPropUseTip.CleanUp();
        _uiComponentDropReward.CleanUp();
        _uiComponentSnowMountainVictory.CleanUp();

        //uiComponentQuest.CleanUp();

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

        _uiComponentReturnOrSettlement.gameObject.SetActive(false);

        _uiComponentBossHealth.Init();
        _uiComponentBossHealth.gameObject.SetActive(true);
    }

    /// <summary>
    /// 播放Boss血量增长动画
    /// </summary>
    public async UniTask PlayBossHealthIncreaseAnimationAsync()
    {
        await _uiComponentBossHealth.PlayBossHealthIncreaseAnimationAsync();
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
    /// 播放道具使用提示动画
    /// </summary>
    public async UniTask PlayPropUseTipAnimationAsync()
    {
        _uiComponentPropUseTip.Init();
        _uiComponentPropUseTip.gameObject.SetActive(true);

        await _uiComponentPropUseTip.PlayBlinkAsync();
        
        _uiComponentPropUseTip.CleanUp();
        _uiComponentPropUseTip.gameObject.SetActive(false);
    }

    /// <summary>
    /// 播放雪山胜利光效动画
    /// </summary>
    /// <param name="worldPosition">起点世界坐标</param>
    public async UniTask PlaySnowMountainVictoryAsync(Vector3 worldPosition)
    {
        _uiComponentSnowMountainVictory.gameObject.SetActive(true);
        await _uiComponentSnowMountainVictory.PlayLightEffectAsync(worldPosition);
        await _uiComponentSnowMountainVictory.PlayFamilyPortraitFadeInAsync();
    }
    #endregion
}
