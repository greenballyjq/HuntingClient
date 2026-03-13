using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 陷阱道具处理器
/// </summary>
public class PropTrapHandler : BasePropHandler
{
    /// <summary>
    /// 道具参数
    /// </summary>
    private PropTrap _propParam;

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
    /// 场上陷阱位置缓存
    /// </summary>
    private readonly List<Vector3> _existingTrapPositions = new List<Vector3>();

    /// <summary>
    /// 本局已生成陷阱位置
    /// </summary>
    private readonly List<Vector3> _newPositions = new List<Vector3>();

    private TrapManager _trapManager;

    private const float TRAP_MIN_DISTANCE = 3f;
    private const float AREA_BOUNDARY_OFFSET = -16f;
    private const float ANIMAL_CHECK_RADIUS = 2f;
    private const int MAX_ATTEMPTS = 20;

    protected override void OnInit()
    {
        _trapManager = GameServiceLocator.GetRoundManager<TrapManager>();
        _propParam = ConfigManager.GetPropTrap(PropData.ParamTableID);
    }

    protected override UniTask OnPropStart()
    {
        _spawnTimer = 0f;
        _spawnedCount = 0;
        _newPositions.Clear();

        int min = _propParam.SpawnCount?.Length > 0 ? _propParam.SpawnCount[0] : 0;
        int max = _propParam.SpawnCount?.Length > 1 ? _propParam.SpawnCount[1] : min;
        _spawnCount = Mathf.Clamp(Random.Range(min, max + 1), 1, int.MaxValue);

        _spawnInterval = PropData.Duration / _spawnCount;

        return UniTask.CompletedTask;
    }

    protected override void OnPropUpdate(float dt)
    {
        if (_spawnedCount >= _spawnCount)
            return;

        _spawnTimer += dt;

        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0f;
            SpawnTrap();
            _spawnedCount++;
        }
    }

    protected override void OnPropEnd() { }

    #region 私有方法

    /// <summary>
    /// 生成单个陷阱
    /// </summary>
    private void SpawnTrap()
    {
        _trapManager.GetTrapPositions(_existingTrapPositions);

        Vector3 position = default;
        for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
        {
            position = PlayableArea.GetRandomPoint(100, AREA_BOUNDARY_OFFSET);
            if (IsValidTrapPosition(position))
                break;
        }

        _newPositions.Add(position);
        _trapManager.SpawnTrap(position);
    }

    /// <summary>
    /// 是否是有效陷阱位置
    /// </summary>
    private bool IsValidTrapPosition(Vector3 position)
    {
        if (Physics.OverlapSphere(position, ANIMAL_CHECK_RADIUS, LayerMask.GetMask("Animal")).Length > 0)
            return false;

        foreach (var p in _newPositions)
        {
            if (Vector3.Distance(position, p) < TRAP_MIN_DISTANCE)
                return false;
        }
        foreach (var p in _existingTrapPositions)
        {
            if (Vector3.Distance(position, p) < TRAP_MIN_DISTANCE)
                return false;
        }
        return true;
    }

    #endregion
}
