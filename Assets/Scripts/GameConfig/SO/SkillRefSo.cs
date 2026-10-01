using cfg.HuntingConfig.Enum;
using System.Collections.Generic;
using GameFramework.Audio;
using UnityEngine;

/// <summary>
/// 技能关联资源配置
/// </summary>
[CreateAssetMenu(fileName = "SkillRefSo", menuName = "SO/SkillRefSo")]
public class SkillRefSo : ScriptableObject
{
    /// <summary>
    /// 技能关联资源类
    /// </summary>
    [System.Serializable]
    public class SkillRef
    {
        /// <summary>
        /// ID
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

        /// <summary>
        /// 技能专属音效。空则只播通用触发音
        /// </summary>
        public SfxCue Unique;
    }

    /// <summary>
    /// 所有技能共用的触发音
    /// </summary>
    public SfxCue Use;

    /// <summary>
    /// 技能关联资源列表
    /// </summary>
    [SerializeField] private List<SkillRef> _skillRefList;

    #region 公共方法
    /// <summary>
    /// 根据ID获取技能关联资源
    /// </summary>
    public SkillRef Get(int id)
    {
        for (int i = 0; i < _skillRefList.Count; i++)
        {
            if (_skillRefList[i].ID == id)
                return _skillRefList[i];
        }
        return null;
    }

    /// <summary>
    /// 根据ID获取技能图标
    /// </summary>
    public Sprite GetSkillIcon(int id)
    {
        return Get(id)?.SkillIcon;
    }

    /// <summary>
    /// 根据ID获取技能预制体
    /// </summary>
    public GameObject GetSkillPrefab(int id)
    {
        return Get(id)?.SkillPrefab;
    }
    #endregion
}
