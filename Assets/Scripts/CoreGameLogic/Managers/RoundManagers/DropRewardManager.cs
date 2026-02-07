using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Manager;
using GameFramework.Utility;
using UnityEngine;

public class DropRewardManager : IRoundManager
{
    private EffectManager _effectManager =>
        GameServiceLocator.GetFrameworkManager<EffectManager>();

    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    private EventManager _eventManager => GameServiceLocator.EventManager;
    private UIManager _uiManager => GameServiceLocator.UIManager;
    
    private CameraManager _cameraManager => GameServiceLocator.GetAppManager<CameraManager>();

    private RectTransform _skillTargetTransform;
    private RectTransform _meatProgressTargetTransform;
    private RectTransform _bulletTargetTransform;
    private RectTransform _settlementTargetTransform;

    private Camera _mainCamera => _cameraManager.MainCamera;
    private Camera _uiCamera => _cameraManager.UICamera;
    
    public void Init(RoundContext context)
    {
        // todo 监听DropReward 事件
        _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
        // await _effectManager.PlayOneShotAsync()
        
        // var uiGameplay = _uiManager.GetUI<UIGameplay>("UIGamePlay");
        // _skillTargetTransform = uiGameplay.UIComponentSkill.SkillTransform;
        // _meatProgressTargetTransform = uiGameplay.UIComponentMeatProgress.MeatBarFillTransform;
        // _bulletTargetTransform = uiGameplay.UIComponentBullet.BulletTransform;
        // _settlementTargetTransform = uiGameplay.UIComponentReturnOrSettlement.SettlementTransform;
    }

    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        HandleDropReward(args);
    }

    private void HandleDropReward(AnimalDropRewardEventArgs args)
    {
        // 根据掉落的个数生产相同个数的light特效，并根据掉落类型飞往不同的UI
        var worldPosition = args.Animal.transform.position;
        var dropRewardDict = args.DropRewards;
        foreach (var item in dropRewardDict)
        {
            var count = item.Value;
            var type = item.Key;
            if (count > 0)
                SpawnDropEffectAndMove(type, count, worldPosition).Forget();
        }
    }

    private async UniTask SpawnDropEffectAndMove(EDropType dropType, int dropCount, Vector3 worldPosition)
    {
        var uiGameplay = _uiManager.GetUI<UIGameplay>("UIGameplay");
        _skillTargetTransform = uiGameplay.UIComponentSkill.SkillTransform;
        _meatProgressTargetTransform = uiGameplay.UIComponentMeatProgress.MeatTransform;
        _bulletTargetTransform = uiGameplay.UIComponentBullet.BulletTransform;
        _settlementTargetTransform = uiGameplay.UIComponentReturnOrSettlement.SettlementTransform;
        
        RectTransform parent = null;
        Vector3 startPositon = Vector3.zero;
        Vector3 endPosition = Vector3.zero;

        switch (dropType)
        {
            case EDropType.Bullet:
                parent = _bulletTargetTransform;
                startPositon =
                    PointConverter.WorldPointToUiPoint(_bulletTargetTransform, worldPosition, _mainCamera, _uiCamera);
                endPosition = _bulletTargetTransform.position;
                break;
            case EDropType.Energy:
                parent = _skillTargetTransform;
                startPositon =
                    PointConverter.WorldPointToUiPoint(_skillTargetTransform, worldPosition, _mainCamera, _uiCamera);
                endPosition = _skillTargetTransform.position;
                break;
            case EDropType.Meat:
                parent = _meatProgressTargetTransform;
                startPositon =
                    PointConverter.WorldPointToUiPoint(_meatProgressTargetTransform, worldPosition, _mainCamera, _uiCamera);
                endPosition = _meatProgressTargetTransform.position;
                break;
            case EDropType.ThreeKPCoin:
                parent = _settlementTargetTransform;
                startPositon =
                    PointConverter.WorldPointToUiPoint(_settlementTargetTransform, worldPosition, _mainCamera, _uiCamera);
                endPosition = _settlementTargetTransform.position;
                break;
        }
        
        var effectPrefab = await _resourceManager.LoadAssetAsync<GameObject>("Assets/Arts/Prefabs/Particles/FX_DGC_SJSL_fly_UI");
        var effectObj = Object.Instantiate(effectPrefab, parent);
        effectObj.transform.position = startPositon;
        await effectObj.transform.DOMove(endPosition, 2f).ToUniTask();
        Object.Destroy(effectObj);
        
        TriggerDropRewardArrived(dropType, dropCount);
    }

    private void TriggerDropRewardArrived(EDropType dropType, int dropCount)
    {
        _eventManager.Trigger(DropRewardEvents.DropRewardArrived, new RewardArrivedEventArgs
        {
            DropType = dropType,
            DropCount = dropCount
        });
    }

    public void Dispose()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }
}
