using cfg.HuntingConfig;

namespace Hunting.Round
{
    /// <summary>
    /// 单局上下文
    /// </summary>
    public sealed class RoundContext
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 地图ID
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// 技能ID
        /// </summary>
        public int SkillId { get; set; }

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
