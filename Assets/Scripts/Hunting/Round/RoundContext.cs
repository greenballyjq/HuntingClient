using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;

namespace Hunting.Round
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public sealed class RoundContext
    {
           /// <summary>
        /// 角色数据
        /// </summary>
        public Role RoleData { get; set; }

        /// <summary>
        /// 地图数据
        /// </summary>
        public Map MapData { get; set; }

        /// <summary>
        /// 技能数据
        /// </summary>
        public Skill SkillData { get; set; }

        /// <summary>
        /// 幸运仪式增益数据
        /// </summary>
        public LuckyBuff LuckyBuffData { get; set; }

        /// <summary>
        /// 是否存在地图联动
        /// </summary>
        public bool HasMapAffinity { get; set; }
    }
}
