using cfg.HuntingConfig.Prop;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 智能诱捕陷阱道具处理器
/// </summary>
public class PropTrapHandler : IPropHandler
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 陷阱管理器
    /// </summary>
    private TrapManager _trapManager => GameServiceLocator.GetRoundManager<TrapManager>();

    /// <summary>
    /// 道具效果开始
    /// </summary>
    public async void OnPropStart(PropContext context)
    {
        Transform playerTransform = FindPlayerTransform();

        // 读取配置参数
        PropTrap parameter = _configManager.GetPropTrap(context.PropData.ParamTableID);

        // 获取已有陷阱位置
        List<Vector3> existingTrapPositions = _trapManager.GetAllTrapPositions();

        // 生成陷阱位置列表
        List<Vector3> trapPositions = GenerateTrapPositions(
            playerTransform,
            parameter.TrapCount,
            parameter.SpawnMinDistance,
            parameter.SpawnMaxDistance,
            parameter.SpawnSectorAngle,
            parameter.SpawnAnimalCheckRadius,
            parameter.SpawnTrapMinDistance,
            existingTrapPositions
        );

        // 创建陷阱实例
        foreach (Vector3 position in trapPositions)
        {
            await _trapManager.CreateTrapAsync(
                position,
                parameter.AttractRadius,
                parameter.TriggerRadius,
                parameter.AttractRadiusRangeByVolume,
                parameter.TrapPrefabResourcePath
            );
        }
    }

    /// <summary>
    /// 道具效果更新
    /// </summary>
    public void OnPropUpdate(PropContext context, float deltaTime)
    {

    }

    /// <summary>
    /// 道具效果结束
    /// </summary>
    public void OnPropEnd(PropContext context)
    {

    }

    #region 私有方法
    /// <summary>
    /// 生成陷阱位置列表
    /// </summary>
    private List<Vector3> GenerateTrapPositions(
        Transform playerTransform,
        int trapCount,
        float spawnMinDistance,
        float spawnMaxDistance,
        float sectorAngle,
        float animalCheckRadius,
        float trapMinDistance,
        List<Vector3> existingTrapPositions
    )
    {
        List<Vector3> positions = new List<Vector3>();
        int maxAttempts = 100;
        int attempts = 0;

        while (positions.Count < trapCount && attempts < maxAttempts)
        {
            attempts++;

            // 生成候选位置
            Vector3 candidate = GetRandomSectorPoint(
                playerTransform,
                spawnMinDistance,
                spawnMaxDistance,
                sectorAngle
            );

            // 检查位置是否可以放置陷阱
            if (CanPlaceTrap(candidate, animalCheckRadius, trapMinDistance, positions, existingTrapPositions))
            {
                positions.Add(candidate);
                attempts = 0; // 重置尝试次数
            }
        }

        return positions;
    }

    /// <summary>
    /// 获取扇形范围内随机点
    /// </summary>
    private Vector3 GetRandomSectorPoint(
        Transform playerTransform,
        float spawnMinDistance,
        float spawnMaxDistance,
        float sectorAngle
    )
    {
        // 随机角度
        float randomAngle = Random.Range(-sectorAngle / 2f, sectorAngle / 2f);
        Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);

        // 随机距离
        float randomDistance = Random.Range(spawnMinDistance, spawnMaxDistance);

        // 计算位置
        Vector3 direction = rotation * playerTransform.forward;
        Vector3 position = playerTransform.position + direction * randomDistance;

        return position;
    }

    /// <summary>
    /// 检查是否可以在此位置放置陷阱
    /// </summary>
    private bool CanPlaceTrap(
        Vector3 position,
        float animalCheckRadius,
        float trapMinDistance,
        List<Vector3> newPositions,
        List<Vector3> existingTrapPositions
    )
    {
        // 检测是否有动物
        Collider[] animalColliders = Physics.OverlapSphere(
            position,
            animalCheckRadius,
            LayerMask.GetMask("Animal")
        );

        if (animalColliders.Length > 0)
            return false;

        // 检测是否与生成的其他陷阱太近
        foreach (Vector3 newPosition in newPositions)
        {
            float distance = Vector3.Distance(position, newPosition);
            if (distance < trapMinDistance)
                return false;
        }
        foreach (Vector3 existingPosition in existingTrapPositions)
        {
            float distance = Vector3.Distance(position, existingPosition);
            if (distance < trapMinDistance)
                return false;
        }

        return true;
    }
    #endregion

    #region TODO：未来可配置化
    /// <summary>
    /// 玩家Transform
    /// </summary>
    private Transform _playerTransform;

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
