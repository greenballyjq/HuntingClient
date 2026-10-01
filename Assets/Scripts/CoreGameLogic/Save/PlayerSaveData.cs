using System;
using cfg.HuntingConfig.Enum;

/// <summary>
/// 玩家存档 DTO
/// </summary>
[Serializable]
public class PlayerSaveData
{
    /// <summary>
    /// 存档版本
    /// </summary>
    public int Version = 2;

    /// <summary>
    /// 三千盘金币数量
    /// </summary>
    public int ThreeKPCoinAmount;

    /// <summary>
    /// 积分数量
    /// </summary>
    public int PointAmount;

    /// <summary>
    /// 完成局数
    /// </summary>
    public int CompletedRoundCount;

    /// <summary>
    /// 击败 Boss 总数
    /// </summary>
    public int TotalBossKillCount;

    /// <summary>
    /// 各道具数量
    /// </summary>
    public PropCountEntry[] PropCounts = Array.Empty<PropCountEntry>();

    /// <summary>
    /// 各 Boss 击杀数
    /// </summary>
    public BossKillEntry[] BossKills = Array.Empty<BossKillEntry>();

    /// <summary>
    /// 金币人所在格子索引，-1 为场外
    /// </summary>
    public int CoinManSlotIndex = -1;
}

/// <summary>
/// 道具数量条目
/// </summary>
[Serializable]
public class PropCountEntry
{
    /// <summary>
    /// 道具类型
    /// </summary>
    public EPropType PropType;

    /// <summary>
    /// 数量
    /// </summary>
    public int Count;
}

/// <summary>
/// Boss 击杀条目
/// </summary>
[Serializable]
public class BossKillEntry
{
    /// <summary>
    /// Boss 类型
    /// </summary>
    public EBossType BossType;

    /// <summary>
    /// 击杀数
    /// </summary>
    public int Count;
}
