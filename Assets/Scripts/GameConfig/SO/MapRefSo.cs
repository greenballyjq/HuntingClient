using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图关联资源配置
/// </summary>
[CreateAssetMenu(fileName = "MapRefSo", menuName = "SO/MapRefSo", order = 6)]
public class MapRefSo : ScriptableObject
{
    /// <summary>
    /// 地图关联资源类
    /// </summary>
    [System.Serializable]
    public class MapRef
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 地图图标
        /// </summary>
        public Sprite MapIcon;
    }

    /// <summary>
    /// 地图关联资源列表
    /// </summary>
    [SerializeField] private List<MapRef> _mapRefList;

    /// <summary>
    /// 根据ID获取地图图标
    /// </summary>
    public Sprite GetMapIcon(int id)
    {
        for (int i = 0; i < _mapRefList.Count; i++)
        {
            if (_mapRefList[i].ID == id)
                return _mapRefList[i].MapIcon;
        }
        return null;
    }
}
