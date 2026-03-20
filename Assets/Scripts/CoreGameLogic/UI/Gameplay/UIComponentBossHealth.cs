using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using GameFramework.Manager;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Boss血量组件
/// </summary>
public class UIComponentBossHealth : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// Boss血量增长动画配置
    /// </summary>
    [Serializable]
    private class BossHealthAnimationConfig
    {
        /// <summary>
        /// 动画时长（秒）
        /// </summary>
        public float Duration = 2f;

        /// <summary>
        /// 是否不受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime;
    }

    /// <summary>
    /// Boss血量条图像
    /// </summary>
    [SerializeField] private Image _imageBossHealthBar;

    /// <summary>
    /// Boss血量增长动画配置
    /// </summary>
    [SerializeField] private BossHealthAnimationConfig _animationConfig;

    private EventManager _eventManager;

    private void Awake()
    {
        RegisterServers();
    }

    public void Init()
    {
        _imageBossHealthBar.fillAmount = 0f;
        _eventManager.AddListener(AnimalEvents.BossDamaged, OnBossDamaged);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(AnimalEvents.BossDamaged, OnBossDamaged);
    }

    /// <summary>
    /// 播放Boss血量增长动画
    /// </summary>
    public async UniTask PlayBossHealthIncreaseAsync()
    {
        await _imageBossHealthBar
            .DOFillAmount(1f, _animationConfig.Duration)
            .SetLink(gameObject)
            .SetUpdate(_animationConfig.UseUnscaledTime)
            .ToUniTask();
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServers()
    {
        _eventManager = GameServiceLocator.EventManager;
    }

    /// <summary>
    /// 刷新Boss血量条显示
    /// </summary>
    private void RefreshBossHealthBar(float healthRatio)
    {
        _imageBossHealthBar.fillAmount = healthRatio;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// Boss受伤事件回调
    /// </summary>
    private void OnBossDamaged(BossDamagedEventArgs args)
    {
        RefreshBossHealthBar(args.CurrentHealth / args.MaxHealth);
    }
    #endregion
}
