using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单个角色格子组件
/// </summary>
public class UIComponentRoleSlot : MonoBehaviour
{
    /// <summary>
    /// 角色图像
    /// </summary>
    [SerializeField] private Image _imageRole;

    /// <summary>
    /// 当前角色ID
    /// </summary>
    private int _currentRoleId;

    #region 公共方法
    /// <summary>
    /// 设置角色
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="roleSprite">角色图片</param>
    public void SetRole(int roleId, Sprite roleSprite)
    {
        _currentRoleId = roleId;
        _imageRole.sprite = roleSprite;
    }

    /// <summary>
    /// 获取角色ID
    /// </summary>
    /// <returns>角色ID</returns>
    public int GetRoleId()
    {
        return _currentRoleId;
    }

    /// <summary>
    /// 获取自身位置
    /// </summary>
    /// <returns>位置</returns>
    public Vector3 GetPosition()
    {
        return (transform as RectTransform).anchoredPosition3D;
    }
    #endregion
}
