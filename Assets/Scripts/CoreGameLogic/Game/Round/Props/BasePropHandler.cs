using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Audio;
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
    protected AudioManager AudioManager;
    protected GameplaySceneItemManager GameplaySceneItemManager;
    protected RoundNumericLayer NumericLayer;

    public void Init(Prop propData)
    {
        EffectManager = GameServiceLocator.EffectManager;
        ConfigManager = GameServiceLocator.ConfigManager;
        AudioManager = GameServiceLocator.AudioManager;
        GameplaySceneItemManager = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();
        NumericLayer = GameServiceLocator.GetRoundManager<RoundNumericLayer>();

        PropData = propData;

        OnInit();
    }

    public async UniTask StartProp()
    {
        PropPhase = PropPhase.Starting;

        Player = GameplaySceneItemManager.Player;
        PlayableArea = GameplaySceneItemManager.PlayableArea;

        _remainingTime = PropData.Duration;

        var propRef = ConfigManager.PropRefSo.Get(PropData.ID);
        AudioManager.Play(propRef?.Use);
        var roleRef = ConfigManager.RoleRefSo.Get(HuntingAppFlow.Instance.RoundFlow.RoundContext.RoleData.ID);
        if (roleRef != null)
        {
            switch (PropData.PropType)
            {
                case cfg.HuntingConfig.Enum.EPropType.Bombardment:
                    AudioManager.Play(roleRef.BombardmentVoice);
                    break;
                case cfg.HuntingConfig.Enum.EPropType.AimAssist:
                    AudioManager.Play(roleRef.AimAssistVoice);
                    break;
                case cfg.HuntingConfig.Enum.EPropType.Trap:
                    AudioManager.Play(roleRef.TrapVoice);
                    break;
            }
        }

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
