using GameFramework.Core;
using GameFramework.Core.UI;
using Hunting.Events;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色选择组件
/// </summary>
public class UIComponentRoleChoice : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 角色格子列表
    /// </summary>
    [SerializeField] private UIComponentRoleSlot[] _roleSlots;

    /// <summary>
    /// 骰子按钮
    /// </summary>
    [SerializeField] private Button _buttonDice;

    /// <summary>
    /// 当前格子索引
    /// </summary>
    private int _currentSlotIndex;

    /// <summary>
    /// 目标格子索引
    /// </summary>
    private int _targetSlotIndex;

    /// <summary>
    /// 骰子点数
    /// </summary>
    private int _diceValue;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    private void Awake()
    {
        _buttonDice.onClick.AddListener(OnDiceButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonDice.onClick.RemoveListener(OnDiceButtonClicked);
    }

    public void Init()
    {
        _eventManager.AddListener(PrepareEvents.RoleSelectionAnimationEnded, OnRoleSelectionAnimationEnded);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(PrepareEvents.RoleSelectionAnimationEnded, OnRoleSelectionAnimationEnded);
    }

    #region 私有方法
    /// <summary>
    /// 投骰子
    /// </summary>
    private void RollDice()
    {
        // 失活按钮
        _buttonDice.interactable = false;

        // 随机骰子点数
        _diceValue = Random.Range(1, 7);
        int fromSlotIndex = _currentSlotIndex;
        _targetSlotIndex = (_currentSlotIndex + _diceValue) % _roleSlots.Length;

        // 触发角色选中事件
        TriggerRoleSelected(new RoleSelectedEventArgs
        {
            FromSlotIndex = fromSlotIndex,
            TargetSlotIndex = _targetSlotIndex,
            RoleId = _roleSlots[_targetSlotIndex].GetRoleId(),
            DiceValue = _diceValue,
            RoleSlots = _roleSlots
        });

            // 触发完整选角动画开始事件
        _eventManager.Trigger(PrepareEvents.RoleSelectionAnimationStarted);

        // 更新当前格子索引
        _currentSlotIndex = _targetSlotIndex;
    }  
    #endregion

    #region 事件相关
    /// <summary>
    /// 投骰子按钮点击回调
    /// </summary>
    private void OnDiceButtonClicked()
    {
        RollDice();
    }

    /// <summary>
    /// 触发角色选中事件
    /// </summary>
    /// <param name="args">事件参数</param>
    private void TriggerRoleSelected(RoleSelectedEventArgs args)
    {
        _eventManager.Trigger(PrepareEvents.RoleSelected, args);
    }

    /// <summary>
    /// 完整选角动画结束事件回调
    /// </summary>
    private void OnRoleSelectionAnimationEnded()
    {
        // 激活按钮
        _buttonDice.interactable = true;
    }
    #endregion
}
