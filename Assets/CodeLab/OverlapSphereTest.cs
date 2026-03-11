using GameFramework.Utility;
using UnityEngine;

/// <summary>
/// Physics.OverlapSphere 性能测试
/// </summary>
public class OverlapSphereTest : MonoBehaviour
{
    /// <summary>
    /// 检测半径
    /// </summary>
    [SerializeField] private float _radius = 10f;

    /// <summary>
    /// 层遮罩
    /// </summary>
    [SerializeField] private LayerMask _layerMask = -1;

    /// <summary>
    /// 执行次数
    /// </summary>
    [SerializeField] private int _iterations = 10000;

    /// <summary>
    /// 触发按键
    /// </summary>
    [SerializeField] private KeyCode _triggerKey = KeyCode.O;

    #region Unity 生命周期

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
#endif

    private void Update()
    {
        if (!Input.GetKeyDown(_triggerKey)) return;

        using (CodeTimer.Start("OverlapSphere", _iterations))
        {
            for (int i = 0; i < _iterations; i++)
            {
                Physics.OverlapSphere(transform.position, _radius, _layerMask);
            }
        }
    }

    #endregion
}
