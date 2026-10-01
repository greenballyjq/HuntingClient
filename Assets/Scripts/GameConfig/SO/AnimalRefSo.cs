using cfg.HuntingConfig.Enum;
using System.Collections.Generic;
using GameFramework.Audio;
using UnityEngine;

/// <summary>
/// 动物关联资源配置
/// </summary>
[CreateAssetMenu(fileName = "AnimalRefSo", menuName = "SO/AnimalRefSo", order = 5)]
public class AnimalRefSo : ScriptableObject
{
    /// <summary>
    /// 动物关联资源类
    /// </summary>
    [System.Serializable]
    public class AnimalRef
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 动物图标
        /// </summary>
        public Sprite AnimalIcon;

        /// <summary>
        /// 动物预制体
        /// </summary>
        public GameObject AnimalPrefab;

        /// <summary>
        /// Boss 咆哮
        /// </summary>
        public SfxCue Roar;
    }

    /// <summary>
    /// 动物关联资源列表
    /// </summary>
    [SerializeField] private List<AnimalRef> _animalRefList;

    #region 公共方法
    /// <summary>
    /// 根据ID获取动物关联资源
    /// </summary>
    public AnimalRef Get(int id)
    {
        for (int i = 0; i < _animalRefList.Count; i++)
        {
            if (_animalRefList[i].ID == id)
                return _animalRefList[i];
        }
        return null;
    }

    /// <summary>
    /// 根据ID获取动物图标
    /// </summary>
    public Sprite GetAnimalIcon(int id)
    {
        return Get(id)?.AnimalIcon;
    }

    /// <summary>
    /// 根据ID获取动物预制体
    /// </summary>
    public GameObject GetAnimalPrefab(int id)
    {
        return Get(id)?.AnimalPrefab;
    }
    #endregion
}
