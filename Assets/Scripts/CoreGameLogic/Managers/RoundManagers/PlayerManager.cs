using UnityEngine;

/// <summary>
/// 玩家管理器
/// </summary>
public class PlayerManager : IRoundManager, IRoundResettable
{
    /// <summary>
    /// 玩家变换组件
    /// </summary>
    private Transform _player;
    public Transform Player => _player;

    public void Init(RoundContext context)
    {
        FindPlayer();
        Debug.Log("[PlayerManager] 初始化完成");
    }

    public void ReInit(RoundContext context)
    {
        FindPlayer();
    }

    public void Dispose()
    {
        _player = null;
        Debug.Log("[PlayerManager] 已释放");
    }

    public void Cleanup()
    {
        _player = null;
    }

    #region 私有方法
    /// <summary>
    /// 查找玩家
    /// </summary>
    private void FindPlayer() => _player = GameObject.Find("Player").transform;
    #endregion
}
