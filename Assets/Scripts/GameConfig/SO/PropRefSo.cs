using cfg.HuntingConfig.Enum;
using System.Collections.Generic;
using GameFramework.Audio;
using UnityEngine;

/// <summary>
/// 道具关联资源配置
/// </summary>
[CreateAssetMenu(fileName = "PropRefSo", menuName = "SO/PropRefSo")]
public class PropRefSo : ScriptableObject
{
    /// <summary>
    /// 道具关联资源类
    /// </summary>
    [System.Serializable]
    public class PropRef
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 道具类型
        /// </summary>
        public EPropType PropType;

        /// <summary>
        /// 道具图标
        /// </summary>
        public Sprite PropIcon;

        /// <summary>
        /// 道具预制体
        /// </summary>
        public GameObject PropPrefab;

        /// <summary>
        /// 道具特效预制体
        /// </summary>
        public GameObject PropEffectPrefab;

        /// <summary>
        /// 使用音效
        /// </summary>
        public SfxCue Use;

        /// <summary>
        /// 陷阱捕获音效
        /// </summary>
        public SfxCue Catch;
    }

    /// <summary>
    /// 道具关联资源列表
    /// </summary>
    [SerializeField] private List<PropRef> _propRefList;

    #region 公共方法
    /// <summary>
    /// 根据ID获取道具关联资源
    /// </summary>
    public PropRef Get(int id)
    {
        for (int i = 0; i < _propRefList.Count; i++)
        {
            if (_propRefList[i].ID == id)
                return _propRefList[i];
        }
        return null;
    }

    /// <summary>
    /// 根据ID获取道具图标
    /// </summary>
    public Sprite GetPropIcon(int id)
    {
        return Get(id)?.PropIcon;
    }

    /// <summary>
    /// 根据ID获取道具预制体
    /// </summary>
    public GameObject GetPropPrefab(int id)
    {
        return Get(id)?.PropPrefab;
    }

    /// <summary>
    /// 根据ID获取道具特效预制体
    /// </summary>
    public GameObject GetPropEffectPrefab(int id) 
    {
        return Get(id)?.PropEffectPrefab;
    }
    #endregion
}
