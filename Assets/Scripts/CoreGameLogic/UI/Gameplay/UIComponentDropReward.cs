using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core;
using GameFramework.Core.UI;
using GameFramework.Manager;
using GameFramework.Utility;
using Hunting.Events;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 掉落奖励组件
/// </summary>
public class UIComponentDropReward : MonoBehaviour, IUIComponent<UIGameplay>
{
    /// <summary>
    /// 掉落奖励特效预制体
    /// </summary>
    [SerializeField] private GameObject _dropRewardEffectPrefab;

    /// <summary>
    /// 游玩界面
    /// </summary>
    private UIGameplay _uiGameplay;

    /// <summary>
    /// 掉落奖励组件矩形变换
    /// </summary>
    private RectTransform _dropRewardRectTransform;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager => GameServiceLocator.EffectManager;

    /// <summary>
    /// 相机管理器
    /// </summary>
    private CameraManager _cameraManager => GameServiceLocator.GetAppManager<CameraManager>();

    /// <summary>
    /// 掉落目标位置缓存字典
    /// </summary>
    private Dictionary<EDropType, Vector3> _dropTargetPositionCache;

    private void Awake()
    {
        _dropRewardRectTransform = GetComponent<RectTransform>();
    }

    public void Init(UIGameplay uiGameplay)
    {
        _uiGameplay = uiGameplay;
        CacheDropTargetPositions();
        _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }

    public void CleanUp()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }

    #region 事件处理
    /// <summary>
    /// 动物掉落奖励事件回调
    /// </summary>
    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        foreach (var dropReward in args.DropRewards)
        {
            if (dropReward.Value > 0)
                PlayDropRewardAnimation(dropReward.Key, dropReward.Value, args.Animal.transform.position).Forget();
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 缓存掉落目标位置
    /// </summary>
    private void CacheDropTargetPositions()
    {
        _dropTargetPositionCache = new Dictionary<EDropType, Vector3>
        {
            { EDropType.Bullet, _uiGameplay.UIComponentBullet.RectTransformBulletImage.position },
            { EDropType.Energy, _uiGameplay.UIComponentSkill.RectTransformSkillImage.position },
            { EDropType.Meat, _uiGameplay.UIComponentMeatProgress.RectTransformMeatImage.position },
            { EDropType.ThreeKPCoin, _uiGameplay.UIComponentReturnButton.RectTransformReturnButton.position }
            //{ EDropType.ThreeKPCoin, _uiGameplay.UIComponentReturnOrSettlement.RectTransformReturnOrSettlementButton.position }
        };
    }

    /// <summary>
    /// 播放掉落奖励动画
    /// </summary>
    /// <param name="dropType">掉落类型</param>
    /// <param name="dropCount">掉落数量</param>
    /// <param name="worldPosition">世界位置</param>
    private async UniTask PlayDropRewardAnimation(EDropType dropType, int dropCount, Vector3 worldPosition)
    {
        Vector3 endPosition = _dropTargetPositionCache[dropType];
        Vector3 startPosition = PointConverter.WorldPointToUiPoint(_dropRewardRectTransform, worldPosition, _cameraManager.MainCamera, _cameraManager.UICamera);

        GameObject effectObj = _effectManager.PlayLoop(_dropRewardEffectPrefab);
        effectObj.transform.position = startPosition;
        effectObj.transform.SetParent(_dropRewardRectTransform,true);

        await effectObj.transform
            .DOMove(endPosition, 2f)
            .SetLink(effectObj)
            .SetAutoKill()
            .ToUniTask();

        _effectManager.Stop(effectObj);

        TriggerDropRewardEffect(new RewardArrivedEventArgs 
        { 
            DropType = dropType, 
            DropCount = dropCount 
        });
    }

    /// <summary>
    /// 触发掉落奖励生效事件
    /// </summary>
    private void TriggerDropRewardEffect(RewardArrivedEventArgs args)
    {
        _eventManager.Trigger(AnimalEvents.DropRewardArrived, args);
    }
    #endregion
}
