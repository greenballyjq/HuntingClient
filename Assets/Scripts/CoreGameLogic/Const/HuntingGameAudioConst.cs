namespace CoreGameLogic.Const
{
    /// <summary>
    /// 打猎项目 音效资源枚举
    /// </summary>
    public enum HuntingGameAudioId
    {
        #region 地图环境音

        /// <summary>地图环境声-皇家猎场森林</summary>
        MapEnv_RoyalForest,

        /// <summary>地图环境声-海滨沙滩度假村</summary>
        MapEnv_Beatch,

        /// <summary>地图环境声-金状元私家花园</summary>
        MapEnv_PersonalGarden,

        /// <summary>地图环境声-西域大草原</summary>
        MapEnv_GrassLand,

        /// <summary>地图环境声-远古雪山</summary>
        MapEnv_SnowMountain,

        #endregion

        #region 枪和子弹音效

        /// <summary>猎枪发射默认声【PU～】 通用</summary>
        GunShoot_Default,

        /// <summary>子弹命中-普通子弹【久～短音版】</summary>
        BulletHit_Default,

        /// <summary>子弹命中-爆炸子弹</summary>
        BulletHit_Explosive,

        /// <summary>子弹命中-高伤子弹</summary>
        BulletHit_HighDamage,

        /// <summary>子弹命中-高速子弹</summary>
        BulletHit_HighSpeed,

        #endregion

        #region 动物音效

        /// <summary>动物受击叫声【嗯】</summary>
        Animal_Hit,

        /// <summary>动物死亡音效-忍者烟【碰】</summary>
        Animal_Death,

        #endregion

        #region 掉落音效

        /// <summary>掉落-特殊子弹</summary>
        Drop_SpecialBullet,

        #endregion

        #region 丰收技音效

        /// <summary>丰收技通用触发音效</summary>
        Skill_Use,

        /// <summary>色块人丰收技音效【充能声】</summary>
        Skill_BlueRed,

        /// <summary>金状元丰收技音效</summary>
        Skill_JinZhuangYuan,

        /// <summary>金状元专属地图丰收技音效</summary>
        Skill_JinZhuangYuanUnique,

        /// <summary>亚克东丰收技音效</summary>
        Skill_YaKeDong,

        /// <summary>亚克东专属地图丰收技音效</summary>
        Skill_YaKeDongUnique,

        /// <summary>紫薇丰收技音效</summary>
        Skill_ZiWei,

        /// <summary>紫薇专属地图丰收技音效（预备）</summary>
        Skill_ZiWeiUniqueMape_Before,

        /// <summary>紫薇专属地图丰收技音效（射击）</summary>
        Skill_ZiWeiUniqueMap_After,

        /// <summary>大美丽丰收技音效</summary>
        Skill_DaMeiLi,

        /// <summary>大美丽专属地图丰收技音效</summary>
        Skill_DaMeiLiUniqueMap,

        #endregion

        #region 道具音效

        /// <summary>付费道具A-炮火轰炸</summary>
        Props_Bombardment,

        /// <summary>付费道具B-指哪打哪</summary>
        Props_AimAssist,

        /// <summary>付费道具C-智能诱捕陷阱</summary>
        Props_Trap,

        /// <summary>动物被陷阱夹死</summary>
        Props_TrapCatch,

        #endregion

        #region CG音效

        /// <summary>玩家使用紫薇专属地图CG</summary>
        CG_ZiWeiUniqueMap,

        /// <summary>玩家使用金状元专属地图CG</summary>
        CG_JinZhuangYuanUniqueMap,

        /// <summary>玩家使用亚克东专属地图CG</summary>
        CG_YaKeDongUniqueMap,

        #endregion

        #region IP人物语音音效

        /// <summary>紫薇个性化台词 - 被选中触发</summary>
        IP_ZiWei_Selected,

        /// <summary>紫薇自我个性语 - 常规地图开场白</summary>
        IP_ZiWei_Opening,

        /// <summary>紫薇自我个性语 - 专属地图开场白</summary>
        IP_ZiWei_UniqueMap_Opening,

        /// <summary>紫薇使用道具A-炮火轰炸</summary>
        IP_ZiWei_UseProps_Bombardment,

        /// <summary>紫薇使用道具B-指哪打哪</summary>
        IP_ZiWei_UseProps_AimAssist,

        /// <summary>紫薇使用道具C-智能诱捕陷阱</summary>
        IP_ZiWei_UseProps_Trap,

        /// <summary>紫薇使用丰收技时，在两句中随机抓取一条</summary>
        IP_ZiWei_UseSkill_1,

        /// <summary>紫薇使用丰收技时，在两句中随机抓取一条</summary>
        IP_ZiWei_UseSkill_2,

        /// <summary>紫薇游戏结算</summary>
        IP_ZiWei_Settlement,

        /// <summary>大美丽个性化台词 - 被选中触发</summary>
        IP_DaMeiLi_Selected,

        /// <summary>大美丽自我个性语 - 常规地图开场白</summary>
        IP_DaMeiLi_Opening,

        /// <summary>大美丽自我个性语 - 专属地图开场白</summary>
        IP_DaMeiLi_UniqueMap_Opening,

        /// <summary>大美丽使用道具A-炮火轰炸</summary>
        IP_DaMeiLi_UseProps_Bombardment,

        /// <summary>大美丽使用道具B-指哪打哪</summary>
        IP_DaMeiLi_UseProps_AimAssist,

        /// <summary>大美丽使用道具C-智能诱捕陷阱</summary>
        IP_DaMeiLi_UseProps_Trap,

        /// <summary>大美丽使用丰收技时，在两句中随机抓取一条</summary>
        IP_DaMeiLi_UseSkill_1,

        /// <summary>大美丽使用丰收技时，在两句中随机抓取一条</summary>
        IP_DaMeiLi_UseSkill_2,

        /// <summary>大美丽游戏结算</summary>
        IP_DaMeiLi_Settlement,

        #endregion
    }

    /// <summary>
    /// 打猎项目 音效资源路径工具
    /// </summary>
    public static class HuntingGameAudioConst
    {
        /// <summary>
        /// 根据音效枚举获取资源路径
        /// </summary>
        public static string GetPath(HuntingGameAudioId id)
        {
            switch (id)
            {
                // 地图环境音
                case HuntingGameAudioId.MapEnv_RoyalForest: return "Assets/Arts/Audio/DRC/DRC01";
                case HuntingGameAudioId.MapEnv_Beatch: return "Assets/Arts/Audio/DRC/DRC02";
                case HuntingGameAudioId.MapEnv_PersonalGarden: return "Assets/Arts/Audio/DRC/DRC03";
                case HuntingGameAudioId.MapEnv_GrassLand: return "Assets/Arts/Audio/DRC/DRC04";
                case HuntingGameAudioId.MapEnv_SnowMountain: return "Assets/Arts/Audio/DRC/DRC05";

                // 枪和子弹音效
                case HuntingGameAudioId.GunShoot_Default: return "Assets/Arts/Audio/DRC/DRC06";
                case HuntingGameAudioId.BulletHit_Default: return "Assets/Arts/Audio/DRC/DRC07";
                case HuntingGameAudioId.BulletHit_Explosive: return "Assets/Arts/Audio/DRC/DRC08";
                case HuntingGameAudioId.BulletHit_HighDamage: return "Assets/Arts/Audio/DRC/DRC09";
                case HuntingGameAudioId.BulletHit_HighSpeed: return "Assets/Arts/Audio/DRC/DRC10";

                // 动物音效
                case HuntingGameAudioId.Animal_Hit: return "Assets/Arts/Audio/DRC/DRC11";
                case HuntingGameAudioId.Animal_Death: return "Assets/Arts/Audio/DRC/DRC12";

                // 掉落音效
                case HuntingGameAudioId.Drop_SpecialBullet: return "Assets/Arts/Audio/DRC/DRC15";

                // 丰收技音效
                case HuntingGameAudioId.Skill_Use: return "Assets/Arts/Audio/DRC/DRC16";
                case HuntingGameAudioId.Skill_BlueRed: return "Assets/Arts/Audio/DRC/DRC17";
                case HuntingGameAudioId.Skill_JinZhuangYuan: return "Assets/Arts/Audio/DRC/DRC18";
                case HuntingGameAudioId.Skill_JinZhuangYuanUnique: return "Assets/Arts/Audio/DRC/DRC19";
                case HuntingGameAudioId.Skill_YaKeDong: return "Assets/Arts/Audio/DRC/DRC20";
                case HuntingGameAudioId.Skill_YaKeDongUnique: return "Assets/Arts/Audio/DRC/DRC21";
                case HuntingGameAudioId.Skill_ZiWei: return "Assets/Arts/Audio/DRC/DRC08"; // 原先引用 BulletHit_Explosive
                case HuntingGameAudioId.Skill_ZiWeiUniqueMape_Before: return "Assets/Arts/Audio/DRC/DRC23_before";
                case HuntingGameAudioId.Skill_ZiWeiUniqueMap_After: return "Assets/Arts/Audio/DRC/DRC23_after";
                case HuntingGameAudioId.Skill_DaMeiLi: return "Assets/Arts/Audio/DRC/DRC24";
                case HuntingGameAudioId.Skill_DaMeiLiUniqueMap: return "Assets/Arts/Audio/DRC/DRC25";

                // 道具音效
                case HuntingGameAudioId.Props_Bombardment: return "Assets/Arts/Audio/DRC/DRC26";
                case HuntingGameAudioId.Props_AimAssist: return "Assets/Arts/Audio/DRC/DRC27";
                case HuntingGameAudioId.Props_Trap: return "Assets/Arts/Audio/DRC/DRC28";
                case HuntingGameAudioId.Props_TrapCatch: return "Assets/Arts/Audio/DRC/DRC29";

                // CG音效
                case HuntingGameAudioId.CG_ZiWeiUniqueMap: return "Assets/Arts/Audio/DRC/DRC30";
                case HuntingGameAudioId.CG_JinZhuangYuanUniqueMap: return "Assets/Arts/Audio/DRC/DRC31";
                case HuntingGameAudioId.CG_YaKeDongUniqueMap: return "Assets/Arts/Audio/DRC/DRC31";

                // IP人物语音音效
                case HuntingGameAudioId.IP_ZiWei_Selected: return "Assets/Arts/Audio/IP/ZW01";
                case HuntingGameAudioId.IP_ZiWei_Opening: return "Assets/Arts/Audio/IP/ZW02";
                case HuntingGameAudioId.IP_ZiWei_UniqueMap_Opening: return "Assets/Arts/Audio/IP/ZW03";
                case HuntingGameAudioId.IP_ZiWei_UseProps_Bombardment: return "Assets/Arts/Audio/IP/ZW05";
                case HuntingGameAudioId.IP_ZiWei_UseProps_AimAssist: return "Assets/Arts/Audio/IP/ZW06";
                case HuntingGameAudioId.IP_ZiWei_UseProps_Trap: return "Assets/Arts/Audio/IP/ZW04";
                case HuntingGameAudioId.IP_ZiWei_UseSkill_1: return "Assets/Arts/Audio/IP/ZW07";
                case HuntingGameAudioId.IP_ZiWei_UseSkill_2: return "Assets/Arts/Audio/IP/ZW08";
                case HuntingGameAudioId.IP_ZiWei_Settlement: return "Assets/Arts/Audio/IP/ZW09";

                case HuntingGameAudioId.IP_DaMeiLi_Selected: return "Assets/Arts/Audio/IP/DML01";
                case HuntingGameAudioId.IP_DaMeiLi_Opening: return "Assets/Arts/Audio/IP/DML02";
                case HuntingGameAudioId.IP_DaMeiLi_UniqueMap_Opening: return "Assets/Arts/Audio/IP/DML03";
                case HuntingGameAudioId.IP_DaMeiLi_UseProps_Bombardment: return "Assets/Arts/Audio/IP/DML05";
                case HuntingGameAudioId.IP_DaMeiLi_UseProps_AimAssist: return "Assets/Arts/Audio/IP/DML06";
                case HuntingGameAudioId.IP_DaMeiLi_UseProps_Trap: return "Assets/Arts/Audio/IP/DML04";
                case HuntingGameAudioId.IP_DaMeiLi_UseSkill_1: return "Assets/Arts/Audio/IP/DML07";
                case HuntingGameAudioId.IP_DaMeiLi_UseSkill_2: return "Assets/Arts/Audio/IP/DML08";
                case HuntingGameAudioId.IP_DaMeiLi_Settlement: return "Assets/Arts/Audio/IP/DML09";

                default: return string.Empty;
            }
        }

        /// <summary>
        /// 便捷扩展：直接在枚举上取路径
        /// </summary>
        public static string ToPath(this HuntingGameAudioId id) => GetPath(id);
    }
}