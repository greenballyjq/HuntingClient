using cfg.HuntingConfig.Prop;
using CoreGameLogic.Managers.AppManagers;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 道具处理器基类
/// </summary>
public abstract class BasePropHandler : IPropHandler
{
    /// <summary>
    /// 道具阶段
    /// </summary>
    public PropPhase PropPhase { get; private set; }

    /// <summary>
    /// 道具配置
    /// </summary>
    protected Prop PropData;

    /// <summary>
    /// 剩余时间
    /// </summary>
    private float _remainingTime;
    public float RemainingTime => _remainingTime;

    /// <summary>
    /// 玩家变换组件
    /// </summary>
    protected Transform Player;

    /// <summary>
    /// 可游玩区域
    /// </summary>
    protected BaseAreaShape PlayableArea;

    protected EffectManager EffectManager;
    protected HuntingConfigManager ConfigManager;
    protected HuntingSoundManager SoundManager;
    protected GameplaySceneItemManager GameplaySceneItemManager;

    public void Init(Prop propData)
    {
        EffectManager = GameServiceLocator.EffectManager;
        ConfigManager = GameServiceLocator.ConfigManager;
        SoundManager = GameServiceLocator.GetAppManager<HuntingSoundManager>();
        GameplaySceneItemManager = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();

        PropData = propData;

        OnInit();
    }

    public async UniTask StartProp()
    {
        PropPhase = PropPhase.Starting;

        Player = GameplaySceneItemManager.Player;
        PlayableArea = GameplaySceneItemManager.PlayableArea;

        _remainingTime = PropData.Duration;

        await OnPropStart();

        PropPhase = PropPhase.Running;
    }

    public void DoUpdate(float dt)
    {
        _remainingTime -= dt;

        if (_remainingTime <= 0f)
            PropPhase = PropPhase.Finished;

        OnPropUpdate(dt);
    }

    public void EndProp()
    {
        OnPropEnd();
        PropPhase = PropPhase.None;
    }

    /// <summary>
    /// 初始化钩子
    /// </summary>
    protected abstract void OnInit();

    /// <summary>
    /// 道具开始钩子
    /// </summary>
    protected abstract UniTask OnPropStart();

    /// <summary>
    /// 道具更新钩子
    /// </summary>
    /// <param name="dt">时间增量</param>
    protected abstract void OnPropUpdate(float dt);

    /// <summary>
    /// 道具结束钩子
    /// </summary>
    protected abstract void OnPropEnd();
}
