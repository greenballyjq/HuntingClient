using System.Collections.Generic;
using cfg.HuntingConfig.Enum;
using UnityEngine;

/// <summary>
/// 子弹关联资源配置
/// </summary>
[CreateAssetMenu(fileName = "BulletRefSo", menuName = "SO/BulletRefSo", order = 3)]
public class BulletRefSo : ScriptableObject
{
    /// <summary>
    /// 子弹关联资源类
    /// </summary>
    [System.Serializable]
    public class BulletRef
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 子弹类型
        /// </summary>
        public EBulletType BulletType;

        /// <summary>
        /// 子弹预制体
        /// </summary>
        public GameObject BulletPrefab;

        /// <summary>
        /// 子弹图标
        /// </summary>
        public Sprite BulletIcon;

        /// <summary>
        /// 命中特效预制体
        /// </summary>
        public GameObject HitEffectPrefab;
    }

    /// <summary>
    /// 子弹关联资源列表
    /// </summary>
    [SerializeField] private List<BulletRef> _bulletRefList;

    /// <summary>
    /// 根据ID获取子弹预制体
    /// </summary>
    public GameObject GetBulletPrefab(int id)
    {
        for (int i = 0; i < _bulletRefList.Count; i++)
        {
            if (_bulletRefList[i].ID == id)
                return _bulletRefList[i].BulletPrefab;
        }
        return null;
    }

    /// <summary>
    /// 根据ID获取子弹命中特效预制体
    /// </summary>
    public GameObject GetBulletEffectPrefab(int id)
    {
        for (int i = 0; i < _bulletRefList.Count; i++)
        {
            if (_bulletRefList[i].ID == id)
                return _bulletRefList[i].HitEffectPrefab;
        }
        return null;
    }

    /// <summary>
    /// 根据ID获取子弹图标
    /// </summary>
    public Sprite GetBulletIcon(int id)
    {
        for (int i = 0; i < _bulletRefList.Count; i++)
        {
            if (_bulletRefList[i].ID == id)
                return _bulletRefList[i].BulletIcon;
        }
        return null;
    }
}
