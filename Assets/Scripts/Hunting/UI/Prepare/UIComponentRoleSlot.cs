using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
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
        /// 角色ID
        /// </summary>
        [SerializeField] private int _roleId;

        #region 公共方法
        /// <summary>
        /// 获取角色ID
        /// </summary>
        /// <returns>角色ID</returns>
        public int GetRoleId()
        {
            return _roleId;
        }
        #endregion
    }
}
