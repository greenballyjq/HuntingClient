using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 单个角色格子组件
/// </summary>
public class UIComponentRoleSlot : MonoBehaviour
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SerializeField] private int _roleID;
    public int RoleID => _roleID;

    /// <summary>
    /// 角色图像
    /// </summary>
    [SerializeField] private Image _imageRole;

    #region 公共方法
    /// <summary>
    /// 获取格子位置
    /// </summary>
    public Vector3 GetPosition()
    {
        return (transform as RectTransform).anchoredPosition3D;
    }
    #endregion
}
