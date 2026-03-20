using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using GameFramework.Manager;
using GameFramework.Utility;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 掉落奖励光效组件
/// </summary>
public class UIComponentDropRewardLightEffect : MonoBehaviour, IUIComponent<UIGameplay>
{
    /// <summary>
    /// 掉落奖励光效预制体
    /// </summary>
    [SerializeField] private GameObject _dropRewardEffectPrefab;

    /// <summary>
    /// 游玩界面
    /// </summary>
    private UIGameplay _uiGameplay;

    /// <summary>
    /// 掉落奖励光效组件矩形变换
    /// </summary>
    private RectTransform _dropRewardLightEffectRectTransform;

    /// <summary>
    /// 掉落目标位置缓存字典
    /// </summary>
    private Dictionary<EDropType, Vector3> _dropTargetPositionCache;

    private EventManager _eventManager;
    private EffectManager _effectManager;
    private CameraManager _cameraManager;

    private void Awake()
    {
        RegisterServers();
        _dropRewardLightEffectRectTransform = GetComponent<RectTransform>();
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

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void RegisterServers()
    {
        _eventManager = GameServiceLocator.EventManager;
        _effectManager = GameServiceLocator.EffectManager;
        _cameraManager = GameServiceLocator.GetAppManager<CameraManager>();
    }

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
            { EDropType.ThreeKPCoin, _uiGameplay.UIComponentThreeKPCoin.RectTransformThreeKPCoin.position }
        };
    }

    /// <summary>
    /// 播放掉落奖励动画
    /// </summary>
    private async UniTask PlayDropReward(EDropType dropType, int dropCount, Vector3 worldPosition)
    {
        Vector3 endPosition = _dropTargetPositionCache[dropType];
        Vector3 startPosition = PointConverter.WorldPointToUiPoint(_dropRewardLightEffectRectTransform, worldPosition, _cameraManager.MainCamera, _cameraManager.UICamera);

        GameObject effectObj = _effectManager.PlayLoop(_dropRewardEffectPrefab);
        effectObj.transform.SetParent(_dropRewardLightEffectRectTransform, true);
        effectObj.transform.position = startPosition;
       
        await effectObj.transform
            .DOMove(endPosition, 2f)
            .SetLink(effectObj)
            .SetAutoKill()
            .ToUniTask();

        _effectManager.Stop(effectObj);

        TriggerDropRewardEffect(new DropRewardArrivedEventArgs 
        { 
            DropType = dropType, 
            DropCount = dropCount 
        });
    }

    /// <summary>
    /// 触发掉落奖励生效事件
    /// </summary>
    private void TriggerDropRewardEffect(DropRewardArrivedEventArgs args)
    {
        _eventManager.Trigger(AnimalEvents.DropRewardArrived, args);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 动物掉落奖励事件回调
    /// </summary>
    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        foreach (var dropReward in args.DropRewards)
        {
            if (dropReward.Value > 0)
                PlayDropReward(dropReward.Key, dropReward.Value, args.Animal.transform.position).Forget();
        }
    }
    #endregion
}
