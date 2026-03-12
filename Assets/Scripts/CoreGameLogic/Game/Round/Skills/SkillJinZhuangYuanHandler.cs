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
    /// 技能参数缓存
    /// </summary>
    private SkillJinZhuangYuan _skillParamCache;

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

    private MeatProgressManager _meatProgressManager;

    public SkillJinZhuangYuanHandler() : base()
    {
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
    }

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        if(_skillParamCache == null)
            _skillParamCache = _configManager.GetSkillJinZhuangYuan(context.SkillData.ParamTableID);

        _spawnTimer = 0f;
        _spawnedCount = 0;

        _spawnCount = _skillParamCache.SpawnCount[Random.Range(0, _skillParamCache.SpawnCount.Length)];

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

            SpawnMeat();

            _spawnedCount++;
        }
    }

    /// <summary>
    /// 生成肉
    /// </summary>
    private void SpawnMeat()
    {
        _meatProgressManager.AddMeatValue(_skillParamCache.MeatAmount);

        _effectManager.PlayOneShot(
            _configManager.DropRewardRefSo.GetRandomEffectPrefabByIds(_skillParamCache.DropMeatEffectID),
            _playableArea.GetRandomPoint()
        );
    }

    protected override void OnSkillEnd()
    {

    }
}
