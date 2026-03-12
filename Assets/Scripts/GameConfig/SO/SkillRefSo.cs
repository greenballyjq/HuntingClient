using cfg.HuntingConfig.Enum;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能关联资源配置
/// </summary>
[CreateAssetMenu(fileName = "SkillRefSo", menuName = "SO/SkillRefSo", order = 5)]
public class SkillRefSo : ScriptableObject
{
    /// <summary>
    /// 技能关联资源类
    /// </summary>
    [System.Serializable]
    public class SkillRef
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 技能类型
        /// </summary>
        public ESkillType SkillType;

        /// <summary>
        /// 技能图标
        /// </summary>
        public Sprite SkillIcon;

        /// <summary>
        /// 技能预制体
        /// </summary>
        public GameObject SkillPrefab;
    }

    /// <summary>
    /// 技能关联资源列表
    /// </summary>
    [SerializeField] private List<SkillRef> _skillRefList;

    #region 公共方法
    /// <summary>
    /// 根据ID获取技能图标
    /// </summary>
    public Sprite GetSkillIcon(int id)
    {
        for (int i = 0; i < _skillRefList.Count; i++)
        {
            if (_skillRefList[i].ID == id)
                return _skillRefList[i].SkillIcon;
        }
        return null;
    }

    /// <summary>
    /// 根据ID获取技能预制体
    /// </summary>
    public GameObject GetSkillPrefab(int id)
    {
        for (int i = 0; i < _skillRefList.Count; i++)
        {
            if (_skillRefList[i].ID == id)
                return _skillRefList[i].SkillPrefab;
        }
        return null;
    }
    #endregion
}
