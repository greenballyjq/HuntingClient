using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 派发器管理器
/// </summary>
public class SpawnerManager : IRoundManager, IRoundUpdatable, IRoundResettable
{
    /// <summary>
    /// 派发器列表
    /// </summary>
    private readonly List<Spawner> _spawners = new List<Spawner>();



    public void Init(RoundContext context)
    {
        CollectSpawners();
        for (int i = 0; i < _spawners.Count; i++)
            _spawners[i].Init(context.HiddenMapData);
          
        Debug.Log("[SpawnerManager] 初始化完成");
    }

    public void Cleanup()
    {
        _spawners.Clear();
    }

    public void ReInit(RoundContext context)
    {
        CollectSpawners();
        for (int i = 0; i < _spawners.Count; i++)
            _spawners[i].Init(context.HiddenMapData);
    }

    public void Dispose()
    {
        _spawners.Clear();
        Debug.Log("[SpawnerManager] 已释放");
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    public void DoUpdate(float dt)
    {
        for (int i = 0; i < _spawners.Count; i++)
            _spawners[i].DoUpdate(dt);
    }

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


