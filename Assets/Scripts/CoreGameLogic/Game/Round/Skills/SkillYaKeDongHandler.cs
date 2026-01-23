using Cysharp.Threading.Tasks;
using cfg.HuntingConfig.Skill;
using UnityEngine;

/// <summary>
/// 亚克东技能处理器
/// </summary>
public class SkillYaKeDongHandler : ISkillHandler
{
    #region TODO：未来可配置化
    /// <summary>
    /// 生成前方距离
    /// </summary>
    private const float SpawnForwardDistance = 5f;

    /// <summary>
    /// 左右偏移距离
    /// </summary>
    private const float SpawnSideOffset = 8f;

    /// <summary>
    /// 玩家Transform
    /// </summary>
    private Transform _playerTransform;
    #endregion

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 动物管理器
    /// </summary>
    private AnimalManager _animalManager => GameServiceLocator.GetRoundManager<AnimalManager>();

    /// <summary>
    /// 技能开始
    /// </summary>
    public void OnSkillStart(SkillContext context)
    {
        SpawnCoinAnimalsAsync(context).Forget();
    }

    /// <summary>
    /// 技能更新
    /// </summary>
    public void OnSkillUpdate(SkillContext context, float deltaTime)
    {

    }

    /// <summary>
    /// 技能结束
    /// </summary>
    public void OnSkillEnd(SkillContext context)
    {

    }

    #region 私有方法
    /// <summary>
    /// 生成金币怪
    /// </summary>
    private async UniTask SpawnCoinAnimalsAsync(SkillContext context)
    {
        var parameter = _configManager.GetSkillYaKeDong(context.SkillData.ParamTableID);
        var specie = _configManager.GetSpecie(parameter.SpawnAnimalID);
        var player = FindPlayerTransform();

        int spawnCount = GetSpawnCount(parameter);

        Vector3 forward = player.forward;
        Vector3 right = player.right;
        Vector3 basePosition = player.position + forward * SpawnForwardDistance;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition(basePosition, right, i);
            await _animalManager.GenerateAnimalAsync(specie, spawnPosition, forward);
        }
    }

    /// <summary>
    /// 计算单个生成位置
    /// </summary>
    private Vector3 CalculateSpawnPosition(Vector3 basePosition, Vector3 right, int index)
    {
        if (index == 0)
            return basePosition;

        int offsetLayer = (index + 1) / 2;
        int directionSign = (index % 2 == 1) ? -1 : 1;
        Vector3 offset = right * directionSign * offsetLayer * SpawnSideOffset;
        return basePosition + offset;
    }

    /// <summary>
    /// 获取生成数量
    /// </summary>
    private int GetSpawnCount(SkillYaKeDong parameter)
    {
        return Random.Range(parameter.SpawnCount[0], parameter.SpawnCount[1] + 1);
    }

    /// <summary>
    /// 获取玩家Transform
    /// </summary>
    private Transform FindPlayerTransform()
    {
        if (_playerTransform == null)
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        return _playerTransform;
    }
    #endregion
}
