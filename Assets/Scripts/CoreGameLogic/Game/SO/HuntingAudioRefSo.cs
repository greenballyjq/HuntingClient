using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "HuntingAudioRefSo", menuName = "SO/HuntingAudioRefSo", order = 1)]
public class HuntingAudioRefSo : ScriptableObject
{
    public List<HuntingAudioRef> audioRefList;

    /// <summary>
    /// 打猎项目 音效资源枚举
    /// </summary>
    public enum HuntingGameAudioType
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
        Skill_JinZhuangYuan_UniqueMap,

        /// <summary>亚克东丰收技音效</summary>
        Skill_YaKeDong,

        /// <summary>亚克东专属地图丰收技音效</summary>
        Skill_YaKeDong_UniqueMap,

        /// <summary>紫薇丰收技音效</summary>
        Skill_ZiWei,

        /// <summary>紫薇专属地图丰收技音效（预备）</summary>
        Skill_ZiWeiUniqueMap_Before,

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
        
        /// <summary>金状元个性化台词 - 被选中触发</summary>
        IP_JinZhuangYuan_Selected,

        /// <summary>金状元自我个性语 - 常规地图开场白</summary>
        IP_JinZhuangYuan_Opening,

        /// <summary>金状元自我个性语 - 专属地图开场白</summary>
        IP_JinZhuangYuan_UniqueMap_Opening,

        /// <summary>金状元使用道具A-炮火轰炸</summary>
        IP_JinZhuangYuan_UseProps_Bombardment,

        /// <summary>金状元使用道具B-指哪打哪</summary>
        IP_JinZhuangYuan_UseProps_AimAssist,

        /// <summary>金状元使用道具C-智能诱捕陷阱</summary>
        IP_JinZhuangYuan_UseProps_Trap,

        /// <summary>金状元使用丰收技时，在两句中随机抓取一条</summary>
        IP_JinZhuangYuan_UseSkill_1,

        /// <summary>金状元使用丰收技时，在两句中随机抓取一条</summary>
        IP_JinZhuangYuan_UseSkill_2,

        /// <summary>金状元游戏结算</summary>
        IP_JinZhuangYuan_Settlement,

        
        /// <summary>亚克东个性化台词 - 被选中触发</summary>
        IP_YaKeDong_Selected,

        /// <summary>亚克东自我个性语 - 常规地图开场白</summary>
        IP_YaKeDong_Opening,

        /// <summary>亚克东自我个性语 - 专属地图开场白</summary>
        IP_YaKeDong_UniqueMap_Opening,

        /// <summary>亚克东使用道具A-炮火轰炸</summary>
        IP_YaKeDong_UseProps_Bombardment,

        /// <summary>亚克东使用道具B-指哪打哪</summary>
        IP_YaKeDong_UseProps_AimAssist,

        /// <summary>亚克东使用道具C-智能诱捕陷阱</summary>
        IP_YaKeDong_UseProps_Trap,

        /// <summary>亚克东使用丰收技时，在两句中随机抓取一条</summary>
        IP_YaKeDong_UseSkill_1,

        /// <summary>亚克东使用丰收技时，在两句中随机抓取一条</summary>
        IP_YaKeDong_UseSkill_2,

        /// <summary>亚克东游戏结算</summary>
        IP_YaKeDong_Settlement,
        #endregion
    }

    [Serializable]
    public class HuntingAudioRef 
    {
        public HuntingGameAudioType type;
        public AudioClip clip;
    }

    public AudioClip GetAudioFromType(HuntingGameAudioType type)
    {
        return audioRefList.FirstOrDefault(x => x.type == type)?.clip;
    }
}
