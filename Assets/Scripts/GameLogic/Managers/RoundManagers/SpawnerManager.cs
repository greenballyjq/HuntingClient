using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 派发器管理器
/// </summary>
public class SpawnerManager : IRoundManager, IRoundResettable
{
    /// <summary>
    /// 派发器列表
    /// </summary>
    private readonly List<Spawner> _spawners = new List<Spawner>();

    public void Init(RoundContext context)
    {
        CollectSpawners();
        SetMap(context.MapData.ID);
        SetActive(true);
        Debug.Log("[SpecieSpawnManager] 初始化完成");
    }

    public void Cleanup()
    {
        _spawners.Clear();
    }

    public void ReInit(RoundContext context)
    {
        CollectSpawners();
        SetMap(context.HiddenMapData.ID);
        SetActive(true);
    }

    public void Dispose()
    {
        SetActive(false);
        _spawners.Clear();
        Debug.Log("[SpecieSpawnManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 设置派发器的启用状态
    /// </summary>
    public void SetActive(bool active)
    {
        for (int i = 0; i < _spawners.Count; i++)
        {
            if (_spawners[i].IsActive != active)
                _spawners[i].SetActive(active);
        }
    }

    /// <summary>
    /// 设置派发器的地图 ID
    /// </summary>
    public void SetMap(int mapId)
    {
        for (int i = 0; i < _spawners.Count; i++)
            _spawners[i].SetMap(mapId);
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 收集场景中的派发器
    /// </summary>
    private void CollectSpawners()
    {
        _spawners.Clear();
        Spawner[] found = Object.FindObjectsOfType<Spawner>(true);
        _spawners.AddRange(found);
    }
    #endregion
}

