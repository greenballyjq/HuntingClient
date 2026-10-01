using Hunting.Game.Animal;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹效果接口
/// </summary>
public interface IBulletEffect
{
    /// <summary>
    /// 命中处理
    /// </summary>
    /// <param name="context">子弹运行时上下文</param>
    /// <param name="hitInfo">命中信息</param>
    /// <returns>命中的所有动物列表</returns>
    List<IDamageable> OnHit(BulletRuntimeContext context, BulletHitInfo hitInfo);
}

/// <summary>
/// 子弹运行时上下文
/// </summary>
public class BulletRuntimeContext
{
    /// <summary>
    /// 最终伤害
    /// </summary>
    public float FinalDamage { get; set; }

    /// <summary>
    /// 数值修正层
    /// </summary>
    public RoundNumericLayer Numeric { get; set; }
}

/// <summary>
/// 子弹命中信息
/// </summary>
public class BulletHitInfo
{
    /// <summary>
    /// 命中位置
    /// </summary>
    public Vector3 HitPoint { get; set; }

    /// <summary>
    /// 主要命中目标
    /// </summary>
    public IDamageable PrimaryTarget { get; set; }
}
