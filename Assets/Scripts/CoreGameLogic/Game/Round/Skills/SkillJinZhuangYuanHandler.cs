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
    /// 技能参数
    /// </summary>
    private SkillJinZhuangYuan _skillParam;

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

    protected override void OnInit() 
    {
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
        _skillParam = ConfigManager.GetSkillJinZhuangYuan(SkillContext.SkillData.ParamTableID);
    }

    protected override UniTask OnSkillStart()
    {
        _spawnTimer = 0f;
        _spawnedCount = 0;

        _spawnCount = _skillParam.SpawnCount[Random.Range(0, _skillParam.SpawnCount.Length)];

        _spawnInterval = SkillContext.SkillData.Duration / _spawnCount;

        return UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt)
    {
        if (_spawnedCount >= _spawnCount)
            return;

        _spawnTimer += dt;

        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0f;

            SpawnDropMeat();

            _spawnedCount++;
        }
    }

    /// <summary>
    /// 生成掉落肉
    /// </summary>
    private void SpawnDropMeat()
    {
        _meatProgressManager.AddMeatValue(_skillParam.MeatAmount);

        EffectManager.PlayOneShot(
            ConfigManager.DropRewardRefSo.GetRandomEffectPrefabByIds(_skillParam.DropMeatEffectID),
            PlayableArea.GetRandomPoint()
        );
    }

    protected override void OnSkillEnd(){}
}
