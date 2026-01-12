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
    /// 开始投按钮
    /// </summary>
    [SerializeField] private Button _buttonStartRoll;

    /// <summary>
    /// 金币人动画控制器
    /// </summary>
    [SerializeField] private ThreeKPCoinManAnimation _threeKPCoinManAnimation;

    /// <summary>
    /// 角色图片缓存字典
    /// </summary>
    private Dictionary<int, Sprite> _roleSprites = new Dictionary<int, Sprite>();

    /// <summary>
    /// 当前格子索引（-1表示在场外）
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

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    private void Awake()
    {
        _buttonStartRoll.onClick.AddListener(OnDiceButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonStartRoll.onClick.RemoveListener(OnDiceButtonClicked);
    }

    public async void Init()
    {
        // TODO: 之后换到特定的位置预加载
        await LoadAllRoleImagesAsync();

        InitializeRoleSlots();
    }

    public void CleanUp()
    {

    }

    #region 私有方法
    /// <summary>
    /// 异步加载所有角色图片
    /// </summary>
    private async UniTask LoadAllRoleImagesAsync()
    {
        var allRoles = _configManager.RoleTable.DataList;
        foreach (var role in allRoles)
        {
            var sprite = await _resourceManager.LoadAssetAsync<Sprite>(role.RoleImageResourcePath);
            _roleSprites[role.ID] = sprite;
        }
    }

    /// <summary>
    /// 初始化角色格子
    /// </summary>
    private void InitializeRoleSlots()
    {
        // 生成角色ID列表
        List<int> roleIds = new List<int>();
        for (int i = 1; i <= _roleSlots.Length; i++)
            roleIds.Add(i);

        // 打乱顺序
        for (int i = roleIds.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = roleIds[i];
            roleIds[i] = roleIds[j];
            roleIds[j] = temp;
        }

        // 分配给各个Slot
        for (int i = 0; i < _roleSlots.Length; i++)
        {
            int roleId = roleIds[i];
            Sprite roleSprite = _roleSprites[roleId];
            _roleSlots[i].SetRole(roleId, roleSprite);
        }
    }

    /// <summary>
    /// 投骰子
    /// </summary>
    private async UniTask RollDiceAsync()
    {
        _buttonStartRoll.interactable = false;

        // 随机骰子点数
        _diceValue = Random.Range(1, 7);
        int fromSlotIndex = _currentSlotIndex;
        
        // 计算目标格子索引
        _targetSlotIndex = _currentSlotIndex == -1 ? _diceValue - 1 : (_currentSlotIndex + _diceValue) % _roleSlots.Length;

        // 触发角色选择事件
        TriggerRoleSelected(new RoleSelectedEventArgs
        {
            FromSlotIndex = fromSlotIndex,
            TargetSlotIndex = _targetSlotIndex,
            RoleId = _roleSlots[_targetSlotIndex].GetRoleId(),
            DiceValue = _diceValue,
            RoleSlots = _roleSlots
        });

        // 触发骰子动画开始事件
        _eventManager.Trigger(PrepareEvents.DiceAnimationStarted);

        // 播放金币人投骰子动画
        DiceAnimation diceAnimation = await _threeKPCoinManAnimation.PlayThrowDiceAsync();

        // 播放骰子滚动动画
        await diceAnimation.PlayRoll(_diceValue);

        // 等待投骰子动画结束
        await _threeKPCoinManAnimation.WaitForThrowAnimationEndAsync();

        // 触发骰子动画结束事件
        _eventManager.Trigger(PrepareEvents.DiceAnimationEnded);

        // 计算走格子路径
        Vector3[] positionSequence;
        bool[] directions;
        CalculateWalkPath(fromSlotIndex, _diceValue, out positionSequence, out directions);

        // 触发走格子动画开始事件
        _eventManager.Trigger(PrepareEvents.SlotAnimationStarted);

        // 播放金币人行走动画
        await _threeKPCoinManAnimation.PlayWalk(positionSequence, directions);

        // 触发走格子动画结束事件
        _eventManager.Trigger(PrepareEvents.SlotAnimationEnded);

        // 销毁骰子
        diceAnimation.DestroyDice();

        // 更新当前格子索引
        _currentSlotIndex = _targetSlotIndex;

        // 激活按钮
        _buttonStartRoll.interactable = true;
    }

    /// <summary>
    /// 计算走格子路径
    /// </summary>
    /// <param name="fromIndex">起始格子索引（-1表示在场外）</param>
    /// <param name="stepCount">步数</param>
    /// <param name="positions">位置序列</param>
    /// <param name="directions">朝向序列</param>
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

            // 5 0 1 朝右,2 3 4 朝左
            directionList.Add(currentIndex == 5 || currentIndex <= 1);
        }

        positions = positionList.ToArray();
        directions = directionList.ToArray();
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 投骰子按钮点击回调
    /// </summary>
    private void OnDiceButtonClicked()
    {
        RollDiceAsync().Forget();
    }

    /// <summary>
    /// 触发角色选中事件
    /// </summary>
    /// <param name="args">事件参数</param>
    private void TriggerRoleSelected(RoleSelectedEventArgs args)
    {
        _eventManager.Trigger(PrepareEvents.RoleSelected, args);
    }
    #endregion
}
