using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DropRewardRefSo", menuName = "SO/DropRewardRefSo", order = 2)]
public class DropRewardRefSo : ScriptableObject
{
    /// <summary>
    /// 掉落奖励类型
    /// </summary>
    public enum EDropRewardType
    {
        /// <summary>
        /// 肉
        /// </summary>
        Rou,

        /// <summary>
        /// 鸡腿
        /// </summary>
        JiTui,

        /// <summary>
        /// 烤乳猪
        /// </summary>
        KaoRuZhu,

        /// <summary>
        /// 东坡肉
        /// </summary>
        DongPoRou,

        /// <summary>
        /// 烤鱿鱼
        /// </summary>
        KaoYouYu,

        /// <summary>
        /// 牛排
        /// </summary>
        NiuPai,

        /// <summary>
        /// 兰州拉面
        /// </summary>
        LanZhouLaMian,

        /// <summary>
        /// 佛跳墙
        /// </summary>
        FoTiaoQiang,

        /// <summary>
        /// 冰淇淋
        /// </summary>
        BingQiLin,

        /// <summary>
        /// 冻肉
        /// </summary>
        DongRou,

        /// <summary>
        /// 龙虾
        /// </summary>
        LongXia,

        /// <summary>
        /// 三文鱼
        /// </summary>
        SanWenYu,

        /// <summary>
        /// 披萨
        /// </summary>
        Pizza,

        /// <summary>
        /// 汉堡
        /// </summary>
        HanBao,

        /// <summary>
        /// 薯条
        /// </summary>
        ShuTiao,

        /// <summary>
        /// 蛋糕
        /// </summary>
        DanGao,

        /// <summary>
        /// 三千盘金币
        /// </summary>
        ThreeKPCoin,

        /// <summary>
        /// 子弹
        /// </summary>
        Bullet,
    }

    [System.Serializable]
    /// <summary>
    /// 掉落奖励关联资源类
    /// </summary>
    public class DropRewardRef
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 掉落奖励类型
        /// </summary>
        public EDropRewardType Type;

        /// <summary>
        /// 特效预制体
        /// </summary>
        public GameObject EffectPrefab;
    }

    /// <summary>
    /// 掉落奖励关联资源配置列表
    /// </summary>

    [SerializeField] private List<DropRewardRef> _dropRewardRefList;

    /// <summary>
    /// 根据ID数组随机返回一个特效预制体资源
    /// </summary>
    public GameObject GetRandomEffectPrefabByIds(int[] ids)
    {
        var matched = _dropRewardRefList.Where(x => ids.Contains(x.ID)).ToList();
        return matched[Random.Range(0, matched.Count)].EffectPrefab;
    }
}
