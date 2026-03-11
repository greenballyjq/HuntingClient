using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletRefSo", menuName = "SO/BulletRefSo",order = 3)]
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
        public int Id;

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
        => _bulletRefList.FirstOrDefault(x => x.Id == id)?.BulletPrefab;

    /// <summary>
    /// 根据ID获取子弹命中特效预制体
    /// </summary>
    public GameObject GetBulletEffectPrefab(int id)
        => _bulletRefList.FirstOrDefault(x => x.Id == id)?.HitEffectPrefab;

    /// <summary>
    /// 根据ID获取子弹图标
    /// </summary>
    public Sprite GetBulletIcon(int id)
        => _bulletRefList.FirstOrDefault(x => x.Id == id)?.BulletIcon;
}
