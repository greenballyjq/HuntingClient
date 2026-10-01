using GameFramework.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 任务描述组件
/// </summary>
public class UIComponentQuestDescription : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 任务描述文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textDescription;

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
        _textDescription.text = args.Description;
    }
    #endregion
}
