using cfg.HuntingConfig.Prop;
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
    /// 剩余时间
    /// </summary>
    private float _remainingTime;
    public float RemainingTime => _remainingTime;

    /// <summary>
    /// 玩家变换组件
    /// </summary>
    protected Transform _player;

    /// <summary>
    /// 可游玩区域
    /// </summary>
    protected BaseAreaShape _playableArea;

    protected ResourceManager _resourceManager;
    protected HuntingConfigManager _configManager;
    protected GameplaySceneItemManager _gameplaySceneItemManager;

    public BasePropHandler()
    {
        _resourceManager = GameServiceLocator.ResourceManager;
        _configManager = GameServiceLocator.ConfigManager;
        _gameplaySceneItemManager = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();
    }

    public async UniTask StartProp(Prop propData)
    {
        PropPhase = PropPhase.Starting;

        _player = _gameplaySceneItemManager.Player;
        _playableArea = _gameplaySceneItemManager.PlayableArea;

        _remainingTime = propData.Duration;

        await OnPropStart(propData);

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
    /// 道具开始钩子
    /// </summary>
    /// <param name="propData">道具配置</param>
    protected abstract UniTask OnPropStart(Prop propData);

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
