using GameFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务奖励组件（奖励图 + 数量）
/// </summary>
public class UIComponentQuestReward : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 奖励图像
    /// </summary>
    [SerializeField] private Image _imageReward;

    /// <summary>
    /// 奖励数量文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textRewardAmount;

    private EventManager _eventManager;

    private void Awake()
    {
        BindServices();
    }

    public void Init()
    {
        _eventManager.AddListener(QuestEvents.QuestDispatched, OnQuestDispatched);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(QuestEvents.QuestDispatched, OnQuestDispatched);
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _eventManager = GameServiceLocator.EventManager;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 任务派发事件回调
    /// </summary>
    private void OnQuestDispatched(QuestDispatchedEventArgs args)
    {
        _textRewardAmount.text = args.RewardValue.ToString();
    }
    #endregion
}
