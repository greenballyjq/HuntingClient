using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 金状元技能处理器
/// </summary>
public class SkillJinZhuangYuanHandler : BaseSkillHandler
{
    /// <summary>
    /// 生成数量
    /// </summary>
    private int _spawnCount;

    /// <summary>
    /// 生成间隔
    /// </summary>
    private float _spawnInterval;

    /// <summary>
    /// 生成计时器
    /// </summary>
    private float _spawnTimer;

    /// <summary>
    /// 已生成数量
    /// </summary>
    private int _spawnedCount;

    /// <summary>
    /// 可游玩区域
    /// </summary>
    private AreaShape _playableArea;

    /// <summary>
    /// 技能参数
    /// </summary>
    private SkillJinZhuangYuan  _skillParam;

    /// <summary>
    /// 掉落肉特效配置
    /// </summary>
    private DropMeatSo _dropMeatSo;

    private EffectManager _effectManager;

    private MeatProgressManager _meatProgressManager;
    public SkillJinZhuangYuanHandler()
    {
        _effectManager = GameServiceLocator.EffectManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();

        _playableArea = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>().PlayableArea;

        _dropMeatSo = _configManager.DropMeatSo;
    }

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        _spawnTimer = 0f;
        _spawnedCount = 0;

        _skillParam = _configManager.GetSkillJinZhuangYuan(context.SkillData.ParamTableID);

        // 随机生成数量
        _spawnCount = _skillParam.SpawnCount[Random.Range(0, _skillParam.SpawnCount.Length)];

        // 根据技能持续时间计算生成间隔
        _spawnInterval = context.SkillData.Duration / _spawnCount;

        // 模拟播放动画
        await UniTask.Delay(1000);
    }

    protected override void OnSkillUpdate(float dt)
    {
        if (_spawnedCount >= _spawnCount)
            return;

        _spawnTimer += dt;

        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0f;

            // 增加肉度值并生成特效
            _meatProgressManager.AddMeatValue(_skillParam.MeatAmount);
            _effectManager.PlayOneShot(_dropMeatSo.GetRandomDropMeatByIds(_skillParam.DropMeatEffectID), _playableArea.GetRandomPoint());
            
            _spawnedCount++;
        }
    }

    protected override void OnSkillEnd()
    {

    }
}
