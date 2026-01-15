using System.Collections.Generic;
using GameFramework.Core.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 排行榜类型
/// </summary>
public enum RankingType
{
    Daily,
    Weekly
}

/// <summary>
/// 排行榜面板初始化数据
/// </summary>
public class RankingPanelData
{
    /// <summary>
    /// 日榜数据列表
    /// </summary>
    public List<RankingItemData> DailyRankingList { get; set; }

    /// <summary>
    /// 周榜数据列表
    /// </summary>
    public List<RankingItemData> WeeklyRankingList { get; set; }
}

/// <summary>
/// 排行榜面板
/// </summary>
public class UIRanking : UIBase
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    [SerializeField] private Button _buttonClose;

    /// <summary>
    /// 日榜按钮
    /// </summary>
    [SerializeField] private Button _buttonDaily;

    /// <summary>
    /// 周榜按钮
    /// </summary>
    [SerializeField] private Button _buttonWeekly;

    /// <summary>
    /// 滚动视图Content
    /// </summary>
    [SerializeField] private Transform _content;

    /// <summary>
    /// 排行榜项预制体
    /// </summary>
    [SerializeField] private GameObject _itemPrefab;

    /// <summary>
    /// 动态创建的组件列表
    /// </summary>
    private List<UIComponentRankingItem> _itemComponents = new List<UIComponentRankingItem>();

    /// <summary>
    /// 当前榜单类型
    /// </summary>
    private RankingType _currentType = RankingType.Daily;

    /// <summary>
    /// 面板数据
    /// </summary>
    private RankingPanelData _panelData;

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        _panelData = userData as RankingPanelData;

        _buttonDaily.onClick.AddListener(OnDailyButtonClicked);
        _buttonWeekly.onClick.AddListener(OnWeeklyButtonClicked);
        _buttonClose.onClick.AddListener(OnCloseButtonClicked);

        ShowRanking(RankingType.Daily);
    }

    public override void OnClose()
    {
        _buttonDaily.onClick.RemoveListener(OnDailyButtonClicked);
        _buttonWeekly.onClick.RemoveListener(OnWeeklyButtonClicked);
        _buttonClose.onClick.RemoveListener(OnCloseButtonClicked);

        ClearItems();

        base.OnClose();
    }

    #region 私有方法
    /// <summary>
    /// 显示指定类型的榜单
    /// </summary>
    /// <param name="type">榜单类型</param>
    private void ShowRanking(RankingType type)
    {
        _currentType = type;

        ClearItems();

        List<RankingItemData> dataList = type == RankingType.Daily
            ? _panelData.DailyRankingList
            : _panelData.WeeklyRankingList;

        for (int i = 0; i < dataList.Count; i++)
        {
            CreateRankingItem(dataList[i], i + 1);
        }
    }

    /// <summary>
    /// 创建排行榜项
    /// </summary>
    /// <param name="data">排行榜数据</param>
    /// <param name="rank">排名</param>
    private void CreateRankingItem(RankingItemData data, int rank)
    {
        var itemObj = Instantiate(_itemPrefab, _content);
        var component = itemObj.GetComponent<UIComponentRankingItem>();
        component.Init(data);
        component.SetRank(rank);
        _itemComponents.Add(component);
    }

    /// <summary>
    /// 清理所有项
    /// </summary>
    private void ClearItems()
    {
        foreach (var component in _itemComponents)
        {
            component.CleanUp();
            Destroy(component.gameObject);
        }
        _itemComponents.Clear();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 日榜按钮点击回调
    /// </summary>
    private void OnDailyButtonClicked()
    {
        ShowRanking(RankingType.Daily);
    }

    /// <summary>
    /// 周榜按钮点击回调
    /// </summary>
    private void OnWeeklyButtonClicked()
    {
        ShowRanking(RankingType.Weekly);
    }

    /// <summary>
    /// 关闭按钮点击回调
    /// </summary>
    private void OnCloseButtonClicked()
    {
        Close();
    }
    #endregion
}

