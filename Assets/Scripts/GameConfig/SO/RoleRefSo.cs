using System.Collections.Generic;
using GameFramework.Audio;
using UnityEngine;

/// <summary>
/// 角色关联资源配置（表现与语音）
/// </summary>
[CreateAssetMenu(fileName = "RoleRefSo", menuName = "SO/RoleRefSo")]
public class RoleRefSo : ScriptableObject
{
    /// <summary>
    /// 角色关联资源类
    /// </summary>
    [System.Serializable]
    public class RoleRef
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 被选中语音
        /// </summary>
        public SfxCue Selected;

        /// <summary>
        /// 常规开场白
        /// </summary>
        public SfxCue Opening;

        /// <summary>
        /// 结算语音
        /// </summary>
        public SfxCue Settlement;

        /// <summary>
        /// 技能语音（多 clip 随机）
        /// </summary>
        public SfxCue SkillVoice;

        /// <summary>
        /// 使用轰炸道具语音
        /// </summary>
        public SfxCue BombardmentVoice;

        /// <summary>
        /// 使用瞄准道具语音
        /// </summary>
        public SfxCue AimAssistVoice;

        /// <summary>
        /// 使用陷阱道具语音
        /// </summary>
        public SfxCue TrapVoice;
    }

    /// <summary>
    /// 角色关联资源列表
    /// </summary>
    [SerializeField] private List<RoleRef> _roleRefList;

    /// <summary>
    /// 根据ID获取角色关联资源
    /// </summary>
    public RoleRef Get(int id)
    {
        for (int i = 0; i < _roleRefList.Count; i++)
        {
            if (_roleRefList[i].ID == id)
                return _roleRefList[i];
        }
        return null;
    }
}
