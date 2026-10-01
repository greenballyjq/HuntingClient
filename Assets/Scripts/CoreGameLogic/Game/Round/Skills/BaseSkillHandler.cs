using Cysharp.Threading.Tasks;
using cfg.HuntingConfig.Enum;
using GameFramework.Audio;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 技能处理器基类
/// </summary>
public abstract class BaseSkillHandler : ISkillHandler
{
    /// <summary>
    /// 技能阶段
    /// </summary>
    public SkillPhase SkillPhase { get; private set; }

    /// <summary>
    /// 技能上下文
    /// </summary>
    protected SkillContext SkillContext;

    /// <summary>
    /// 剩余时间
    /// </summary>
    private float _remainingTime;
    
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
    protected GameplaySceneItemManager GameplaySceneItemManager;
    protected RoundNumericLayer NumericLayer;
    protected AudioManager AudioManager;

    /// <summary>
    /// 技能专属音效播放期间是否暂停 BGM，播完再恢复
    /// </summary>
    protected virtual bool ShouldYieldMusicForUniqueSkillAudio => false;

    public void Init(SkillContext context)
    {
        EffectManager = GameServiceLocator.EffectManager;
        ConfigManager = GameServiceLocator.ConfigManager;
        AudioManager = GameServiceLocator.AudioManager;
        GameplaySceneItemManager = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();
        NumericLayer = GameServiceLocator.GetRoundManager<RoundNumericLayer>();

        SkillContext = context;

        OnInit();
    }

    public async UniTask StartSkill()
    {
        SkillPhase = SkillPhase.Starting;

        Player = GameplaySceneItemManager.Player;
        PlayableArea = GameplaySceneItemManager.PlayableArea;

        _remainingTime = SkillContext.SkillData.Duration;

        var skillRefSo = ConfigManager.SkillRefSo;
        AudioManager.Play(skillRefSo.Use);
        PlayUniqueSkillAudio(skillRefSo.Get(SkillContext.SkillData.ID)?.Unique);

        var roleData = HuntingAppFlow.Instance.RoundFlow.RoundContext.RoleData;
        if (roleData.RoleType != ERoleType.Bule && roleData.RoleType != ERoleType.Red)
        {
            var roleRef = ConfigManager.RoleRefSo.Get(roleData.ID);
            AudioManager.Play(roleRef?.SkillVoice);
        }

        await OnSkillStart();

        SkillPhase = SkillPhase.Running;
    }

    public void DoUpdate(float dt)
    {
        _remainingTime -= dt;

        if (_remainingTime <= 0)
            SkillPhase = SkillPhase.Finished;

        OnSkillUpdate(dt);
    }

    public void EndSkill()
    {
        OnSkillEnd();
        SkillPhase = SkillPhase.None;
    }

    /// <summary>
    /// 初始化钩子
    /// </summary>
    protected abstract void OnInit();

    /// <summary>
    /// 技能开始钩子
    /// </summary>
    protected abstract UniTask OnSkillStart();

    /// <summary>
    /// 技能更新钩子
    /// </summary>
    /// <param name="dt">时间增量</param>
    protected abstract void OnSkillUpdate(float dt);

    /// <summary>
    /// 技能结束钩子
    /// </summary>
    protected abstract void OnSkillEnd();

    private void PlayUniqueSkillAudio(SfxCue cue)
    {
        if (cue == null)
            return;

        if (!ShouldYieldMusicForUniqueSkillAudio)
        {
            AudioManager.Play(cue);
            return;
        }

        AudioManager.PauseMusic();
        PlayResult result = AudioManager.Play(cue, _ => AudioManager.ResumeMusic());
        if (!result.Succeeded)
            AudioManager.ResumeMusic();
    }
}
