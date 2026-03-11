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
        /// 1.肉
        /// </summary>
        Rou,

        /// <summary>
        /// 2.鸡腿
        /// </summary>
        JiTui,

        /// <summary>
        /// 3.烤乳猪
        /// </summary>
        KaoRuZhu,

        /// <summary>
        /// 4.东坡肉
        /// </summary>
        DongPoRou,

        /// <summary>
        /// 5.烤鱿鱼
        /// </summary>
        KaoYouYu,

        /// <summary>
        /// 6.牛排
        /// </summary>
        NiuPai,

        /// <summary>
        /// 7.兰州拉面
        /// </summary>
        LanZhouLaMian,

        /// <summary>
        /// 8.佛跳墙
        /// </summary>
        FoTiaoQiang,

        /// <summary>
        /// 9.冰淇淋
        /// </summary>
        BingQiLin,

        /// <summary>
        /// 10.冻肉
        /// </summary>
        DongRou,

        /// <summary>
        /// 11.龙虾
        /// </summary>
        LongXia,

        /// <summary>
        /// 12.三文鱼
        /// </summary>
        SanWenYu,

        /// <summary>
        /// 13.披萨
        /// </summary>
        Pizza,

        /// <summary>
        /// 14.汉堡
        /// </summary>
        HanBao,

        /// <summary>
        /// 15.薯条
        /// </summary>
        ShuTiao,

        /// <summary>
        /// 16.蛋糕
        /// </summary>
        DanGao,

        /// <summary>
        /// 101.三千盘金币
        /// </summary>
        ThreeKPCoin,

        /// <summary>
        /// 201.子弹
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
        public int Id;

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
        var matched = _dropRewardRefList.Where(x => ids.Contains(x.Id)).ToList();
        return matched[Random.Range(0, matched.Count)].EffectPrefab;
    }
}
