using UnityEngine;

/// <summary>
/// 弹道学
/// </summary>
public static class Ballistics
{
    #region 公共方法
    /// <summary>计算 3D 预判瞄准方向</summary>
    /// <param name="origin">发射点</param>
    /// <param name="target">目标当前位置</param>
    /// <param name="velocity">目标速度</param>
    /// <param name="projectileSpeed">飞行物速度</param>
    /// <returns>有解返回单位方向向量，无解返回 null</returns>
    public static Vector3? CalculateLeadDirection(
        Vector3 origin,
        Vector3 target,
        Vector3 velocity,
        float projectileSpeed)
    {
        if (!TrySolveLeadTime(origin, target, velocity, projectileSpeed, out float t))
            return null;

        Vector3 leadPoint = target + velocity * t;
        return (leadPoint - origin).normalized;
    }

    /// <summary>计算 2D 预判瞄准方向</summary>
    /// <param name="origin">发射点</param>
    /// <param name="target">目标当前位置</param>
    /// <param name="velocity">目标速度</param>
    /// <param name="projectileSpeed">飞行物速度</param>
    /// <returns>有解返回单位方向向量，无解返回 null</returns>
    public static Vector2? CalculateLeadDirection2D(
        Vector2 origin,
        Vector2 target,
        Vector2 velocity,
        float projectileSpeed)
    {
        if (!TrySolveLeadTime2D(origin, target, velocity, projectileSpeed, out float t))
            return null;

        Vector2 leadPoint = target + velocity * t;
        return (leadPoint - origin).normalized;
    }

    #endregion

    #region 私有方法
    /// <summary>求解飞行物与目标相遇所需时间（3D）</summary>
    /// <param name="origin">发射点</param>
    /// <param name="target">目标当前位置</param>
    /// <param name="velocity">目标速度</param>
    /// <param name="projectileSpeed">飞行物速度</param>
    /// <param name="t">相遇时间，无解时未定义</param>
    /// <returns>是否有解</returns>
    private static bool TrySolveLeadTime(
        Vector3 origin,
        Vector3 target,
        Vector3 velocity,
        float projectileSpeed,
        out float t)
    {
        t = 0f;
        if (projectileSpeed <= 0f) return false;

        // 目标相对发射点的位移向量
        Vector3 d = target - origin;
        // 将 |origin + velocity*t - target| = projectileSpeed*t 平方后整理为 at²+bt+c=0
        float a = Vector3.Dot(velocity, velocity) - projectileSpeed * projectileSpeed;
        float b = 2f * Vector3.Dot(d, velocity);
        float c = Vector3.Dot(d, d);

        return SolveQuadraticForLead(a, b, c, out t);
    }

    /// <summary>求解飞行物与目标相遇所需时间（2D）</summary>
    /// <param name="origin">发射点</param>
    /// <param name="target">目标当前位置</param>
    /// <param name="velocity">目标速度</param>
    /// <param name="projectileSpeed">飞行物速度</param>
    /// <param name="t">相遇时间，无解时未定义</param>
    /// <returns>是否有解</returns>
    private static bool TrySolveLeadTime2D(
        Vector2 origin,
        Vector2 target,
        Vector2 velocity,
        float projectileSpeed,
        out float t)
    {
        t = 0f;
        if (projectileSpeed <= 0f) return false;

        Vector2 d = target - origin;
        float a = Vector2.Dot(velocity, velocity) - projectileSpeed * projectileSpeed;
        float b = 2f * Vector2.Dot(d, velocity);
        float c = Vector2.Dot(d, d);

        return SolveQuadraticForLead(a, b, c, out t);
    }

    /// <summary>解二次方程at²+bt+c=0，取最小正根</summary>
    /// <param name="a">二次项系数</param>
    /// <param name="b">一次项系数</param>
    /// <param name="c">常数项</param>
    /// <param name="t">最小正根，无解时未定义</param>
    /// <returns>是否存在正根</returns>
    private static bool SolveQuadraticForLead(float a, float b, float c, out float t)
    {
        t = 0f;

        // 判别式 b²-4ac，小于 0 则无实根
        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f) return false;

        float sqrtDisc = Mathf.Sqrt(discriminant);

        // a≈0 时退化为一次方程 bt+c=0
        if (Mathf.Approximately(a, 0f))
        {
            if (Mathf.Approximately(b, 0f)) return false;
            t = -c / b;
        }
        else
        {
            // 求根公式 t = (-b ± √Δ) / (2a)
            float t1 = (-b - sqrtDisc) / (2f * a);
            float t2 = (-b + sqrtDisc) / (2f * a);

            // 取最小正根
            if (t1 > 0f && t2 > 0f)
                t = t1 < t2 ? t1 : t2;
            else if (t1 > 0f)
                t = t1;
            else if (t2 > 0f)
                t = t2;
            else
                return false;
        }

        if (t <= 0f) return false;
        return true;
    }
    #endregion
}
