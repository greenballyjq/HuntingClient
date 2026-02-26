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
    /// 特效预制体列表
    /// </summary>
    private List<GameObject> _effectPrefabs = new List<GameObject>();

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
    /// 肉度值
    /// </summary>
    private float _meatAmount;

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager;

    /// <summary>
    /// 肉条管理器
    /// </summary>
    private MeatProgressManager _meatProgressManager;

    /// <summary>
    /// 游戏游玩场景元素管理器
    /// </summary>
    private GameplaySceneItemManager _gameplaySceneItemManager;

    public SkillJinZhuangYuanHandler()
    {
        _resourceManager = GameServiceLocator.ResourceManager;
        _effectManager = GameServiceLocator.EffectManager;
        _meatProgressManager = GameServiceLocator.GetRoundManager<MeatProgressManager>();
        _gameplaySceneItemManager = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();
    }

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        var skillParam = _configManager.GetSkillJinZhuangYuan(context.SkillData.ParamTableID);

        _spawnTimer = 0f;
        _spawnedCount = 0;

        // 缓存配置
        _meatAmount = skillParam.MeatAmount;

        // 缓存特效预制体
        _effectPrefabs.Clear();
        foreach (var effectPath in skillParam.EffectPrefabResourcePaths)
        {
            var prefab = await _resourceManager.LoadAssetAsync<GameObject>(effectPath);
            _effectPrefabs.Add(prefab);
        }

        // 随机生成数量
        _spawnCount = skillParam.SpawnCount[Random.Range(0, skillParam.SpawnCount.Length)];

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
            SpawnEffect();
            _spawnedCount++;
        }
    }

    protected override void OnSkillEnd()
    {
        _effectPrefabs.Clear();
    }

    #region 私有方法
    /// <summary>
    /// 生成特效并增加肉度值
    /// </summary>
    private void SpawnEffect()
    {
        // 游玩区域随机位置
        var playableArea = _gameplaySceneItemManager.PlayableArea;
        Vector3 spawnPos = playableArea.GetRandomPoint();

        // 随机播放特效
        var randomEffectPrefab = _effectPrefabs[Random.Range(0, _effectPrefabs.Count)];
        _effectManager.PlayOneShot(randomEffectPrefab, spawnPos, Quaternion.identity);

        // 增加肉度值
        _meatProgressManager.AddMeatValue(_meatAmount);
    }
    #endregion
}
