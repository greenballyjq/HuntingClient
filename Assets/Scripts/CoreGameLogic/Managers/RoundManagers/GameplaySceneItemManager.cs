using UnityEngine;

/// <summary>
/// 游戏游玩场景元素管理器
/// </summary>
public class GameplaySceneItemManager : IRoundManager, IRoundResettable
{
    /// <summary>
    /// 玩家组件
    /// </summary>
    private Transform _player;
    public Transform Player => _player;

    /// <summary>
    /// 可游玩区域
    /// </summary>
    private AreaShape _playableArea;
    public AreaShape PlayableArea => _playableArea;

    public void Init(RoundContext context)
    {
        FindPlayer();
        FindPlayableArea();
        Debug.Log("[PlayerManager] 初始化完成");
    }

    public void ReInit(RoundContext context)
    {
        FindPlayer();
        FindPlayableArea();
    }

    public void Dispose()
    {
        _player = null;
        _playableArea = null;
        Debug.Log("[PlayerManager] 已释放");
    }

    public void Cleanup()
    {
        _player = null;
        _playableArea = null;
    }

    #region 私有方法
    /// <summary>
    /// 查找玩家
    /// </summary>
    private void FindPlayer() => _player = GameObject.Find("Player").transform;

    /// <summary>
    /// 查找可游玩区域
    /// </summary>
    private void FindPlayableArea() => _playableArea = GameObject.Find("PlayableArea").GetComponent<AreaShape>();
    #endregion
}
