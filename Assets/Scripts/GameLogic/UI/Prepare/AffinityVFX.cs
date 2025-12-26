using GameFramework.Core;
using Hunting.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    /// <summary>
    /// 地图联动视觉组件
    /// </summary>
    public class AffinityVFX : MonoBehaviour
    {
        /// <summary>
        /// 角色图像
        /// </summary>
        [SerializeField] private Image _imageRole;

        /// <summary>
        /// 地图图像
        /// </summary>
        [SerializeField] private Image _imageMap;

        /// <summary>
        /// 联动颜色
        /// </summary>
        [SerializeField] private Color _affinityColor = new Color(1f, 0.84f, 0f, 1f);

        /// <summary>
        /// 原始角色颜色
        /// </summary>
        private Color _originalRoleColor = Color.white;

        /// <summary>
        /// 原始地图颜色
        /// </summary>
        private Color _originalMapColor = Color.white;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager _eventManager => GameServiceLocator.EventManager;

        private void Awake()
        {
            _originalRoleColor = _imageRole.color;
            _originalMapColor = _imageMap.color;

            _eventManager.AddListener(PrepareEvents.MapAffinityChecked, OnMapAffinityChecked);
        }

        private void OnDestroy()
        {
            _eventManager.RemoveListener(PrepareEvents.MapAffinityChecked, OnMapAffinityChecked);
        }

        #region 事件相关
        /// <summary>
        /// 地图联动检查完成事件回调
        /// </summary>
        private void OnMapAffinityChecked(MapAffinityCheckedEventArgs args)
        {
            if (args.HasAffinity)
            {
                _imageRole.color = _affinityColor;
                _imageMap.color = _affinityColor;
            }
            else
            {
                _imageRole.color = _originalRoleColor;
                _imageMap.color = _originalMapColor;
            }
        }
        #endregion
    }
}

