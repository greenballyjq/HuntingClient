using UnityEngine;

/// <summary>
/// 瞄准辅助手势引导控制器
/// </summary>
public class AimAssistHandGuideController : MonoBehaviour
{
    private const float Z_DEPTH_OFFSET = -0.2f;

    /// <summary>
    /// 设置显隐
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    /// <param name="targetPos">目标世界坐标</param>
    /// <param name="dt">时间增量</param>
    public void UpdatePosition(Vector3 targetPos, float dt)
    {
        targetPos.z += Z_DEPTH_OFFSET;
        transform.position = targetPos;
    }
}
