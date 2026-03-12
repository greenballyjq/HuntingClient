using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 智能诱捕陷阱道具处理器
/// </summary>
public class PropTrapHandler : BasePropHandler
{
    /// <summary>
    /// 玩家管理器
    /// </summary>
    private GameplaySceneItemManager _playerManager;

    /// <summary>
    /// 陷阱管理器
    /// </summary>
    private TrapManager _trapManager;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PropTrapHandler()
    {
        _playerManager = GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();
        _trapManager = GameServiceLocator.GetRoundManager<TrapManager>();
    }

    /// <summary>
    /// 道具开始钩子
    /// </summary>
    protected override UniTask OnPropStart(Prop propData)
    {
        Transform player = _playerManager.Player;

        PropTrap parameter = _configManager.GetPropTrap(propData.ParamTableID);
        GameObject trapPrefab = _configManager.PropRefSo.GetPropEffectPrefab(propData.ID);

        List<Vector3> existingTrapPositions = _trapManager.GetAllTrapPositions();

        List<Vector3> trapPositions = GenerateTrapPositions(
            player,
            parameter.TrapCount,
            parameter.SpawnMinDistance,
            parameter.SpawnMaxDistance,
            parameter.SpawnSectorAngle,
            parameter.SpawnAnimalCheckRadius,
            parameter.SpawnTrapMinDistance,
            existingTrapPositions
        );

        foreach (Vector3 position in trapPositions)
        {
            _trapManager.CreateTrap(
                position,
                parameter.AttractRadius,
                parameter.TriggerRadius,
                parameter.AttractRadiusRangeByVolume,
                trapPrefab
            );
        }

        return UniTask.CompletedTask;
    }

    /// <summary>
    /// 道具更新钩子
    /// </summary>
    protected override void OnPropUpdate(float dt) { }

    /// <summary>
    /// 道具结束钩子
    /// </summary>
    protected override void OnPropEnd() { }

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

            Vector3 candidate = GetRandomSectorPoint(playerTransform, spawnMinDistance, spawnMaxDistance, sectorAngle);

            if (CanPlaceTrap(candidate, animalCheckRadius, trapMinDistance, positions, existingTrapPositions))
            {
                positions.Add(candidate);
                attempts = 0;
            }
        }

        return positions;
    }

    /// <summary>
    /// 获取扇形范围内随机点
    /// </summary>
    private Vector3 GetRandomSectorPoint(Transform playerTransform, float spawnMinDistance, float spawnMaxDistance, float sectorAngle)
    {
        float randomAngle = Random.Range(-sectorAngle / 2f, sectorAngle / 2f);
        Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
        float randomDistance = Random.Range(spawnMinDistance, spawnMaxDistance);
        Vector3 direction = rotation * playerTransform.forward;
        Vector3 position = playerTransform.position + direction * randomDistance;

        return position;
    }

    /// <summary>
    /// 检查是否可以在此位置放置陷阱
    /// </summary>
    private bool CanPlaceTrap(Vector3 position, float animalCheckRadius, float trapMinDistance, List<Vector3> newPositions, List<Vector3> existingTrapPositions)
    {
        Collider[] animalColliders = Physics.OverlapSphere(position, animalCheckRadius, LayerMask.GetMask("Animal"));

        if (animalColliders.Length > 0)
            return false;

        foreach (Vector3 newPosition in newPositions)
        {
            if (Vector3.Distance(position, newPosition) < trapMinDistance)
                return false;
        }
        foreach (Vector3 existingPosition in existingTrapPositions)
        {
            if (Vector3.Distance(position, existingPosition) < trapMinDistance)
                return false;
        }

        return true;
    }

    #endregion
}
