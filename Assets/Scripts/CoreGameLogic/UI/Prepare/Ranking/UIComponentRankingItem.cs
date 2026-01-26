using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 排行榜单条数据
/// </summary>
public class RankingItemData
{
    /// <summary>
    /// 玩家头像
    /// </summary>
    public Sprite Avatar { get; set; }

    /// <summary>
    /// 玩家姓名
    /// </summary>
    public string PlayerName { get; set; }

    /// <summary>
    /// 速通时间（已格式化的字符串）
    /// </summary>
    public string ClearTime { get; set; }
}

/// <summary>
/// 排行榜单条数据组件
/// </summary>
public class UIComponentRankingItem : MonoBehaviour, IUIComponent<RankingItemData>
{
    /// <summary>
    /// 玩家头像图像
    /// </summary>
    [SerializeField] private Image _imageAvatar;

    /// <summary>
    /// 玩家姓名文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textPlayerName;

    /// <summary>
    /// 速通时间文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textClearTime;

    /// <summary>
    /// 排名文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textRank;

    public void Init(RankingItemData data)
    {
        UpdateDisplay(data);
    }

    public void CleanUp()
    {
        
    }

    /// <summary>
    /// 设置排名
    /// </summary>
    /// <param name="rank">排名</param>
    public void SetRank(int rank)
    {
        _textRank.text = rank.ToString();
    }

    #region 私有方法
    /// <summary>
    /// 更新显示数据
    /// </summary>
    /// <param name="data">排行榜数据</param>
    private void UpdateDisplay(RankingItemData data)
    {
        if (_imageAvatar != null)
        {
            _imageAvatar.sprite = data.Avatar;
        }
        _textPlayerName.text = data.PlayerName;
        _textClearTime.text = data.ClearTime;
    }
    #endregion
}

