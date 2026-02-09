using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 随机角色组件
/// </summary>
public class UIComponentRollRole : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 角色格子列表
    /// </summary>
    [SerializeField] private UIComponentRoleSlot[] _roleSlots;

    /// <summary>
    /// 金币人动画控制器
    /// </summary>
    [SerializeField] private ThreeKPCoinManThrowDiceAnimator _threeKPCoinManThrowDiceAnimator;

    /// <summary>
    /// 当前格子索引
    /// </summary>
    private int _currentSlotIndex = -1;

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

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    public void Init(){}

    public void CleanUp(){}


    #region 公共方法
    /// <summary>
    /// 投骰子
    /// </summary>
    public async UniTask PlayRollDiceAsync()
    {
        // 随机骰子点数
        _diceValue = Random.Range(1, 7);
        // _diceValue = 1;
        
        int fromSlotIndex = _currentSlotIndex;

        _targetSlotIndex = (_currentSlotIndex + _diceValue) % _roleSlots.Length;
        if (_targetSlotIndex < 0)
        {
            _targetSlotIndex += _roleSlots.Length;
        }

        var roleId = _roleSlots[_targetSlotIndex].RoleID;

        // 触发角色选择事件
        TriggerRoleSelected(new RoleSelectedEventArgs
        {
            FromSlotIndex = fromSlotIndex,
            TargetSlotIndex = _targetSlotIndex,
            RoleId = roleId,
            DiceValue = _diceValue,
            RoleSlots = _roleSlots
        });

        // 触发骰子动画开始事件
        _eventManager.Trigger(PrepareEvents.DiceAnimationStarted);

        // 播放金币人投骰子动画
        DiceRollAnimator diceAnimation = await _threeKPCoinManThrowDiceAnimator.PlayThrowDiceAsync();

        // 播放骰子滚动动画
        await diceAnimation.PlayRoll(_diceValue);

        // 等待投骰子动画结束
        await _threeKPCoinManThrowDiceAnimator.WaitForThrowAnimationEndAsync();

        // 触发骰子动画结束事件
        _eventManager.Trigger(PrepareEvents.DiceAnimationEnded);

        // 计算走格子路径
        Vector3[] positionSequence;
        bool[] directions;
        CalculateWalkPath(fromSlotIndex, _diceValue, out positionSequence, out directions);

        // 触发走格子动画开始事件
        _eventManager.Trigger(PrepareEvents.SlotAnimationStarted);

        // 播放金币人行走动画
        await _threeKPCoinManThrowDiceAnimator.PlayWalkSlot(positionSequence, directions);

        // 触发走格子动画结束事件
        _eventManager.Trigger(PrepareEvents.SlotAnimationEnded);

        // 销毁骰子
        diceAnimation.DestroyDice();

        // 更新当前格子索引
        _currentSlotIndex = _targetSlotIndex;
    }

    /// <summary>
    /// 计算走格子路径
    /// </summary>
    private void CalculateWalkPath(int fromIndex, int stepCount, out Vector3[] positions, out bool[] directions)
    {
        List<Vector3> positionList = new List<Vector3>();
        List<bool> directionList = new List<bool>();

        int currentIndex = fromIndex;
        int totalSlots = _roleSlots.Length;

        for (int i = 0; i < stepCount; i++)
        {
            currentIndex = currentIndex == -1 ? 0 : (currentIndex + 1) % totalSlots;

            positionList.Add(_roleSlots[currentIndex].GetPosition());

            // 0 1 2朝右，3 4 5朝左
            directionList.Add(currentIndex <= 2);
        }

        positions = positionList.ToArray();
        directions = directionList.ToArray();
    }
    #endregion

    #region 事件相关
    private void TriggerRoleSelected(RoleSelectedEventArgs args)
    {
        _eventManager.Trigger(PrepareEvents.RoleSelected, args);
    }
    #endregion
}
