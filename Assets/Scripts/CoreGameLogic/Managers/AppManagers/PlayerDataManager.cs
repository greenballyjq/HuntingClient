using cfg.HuntingConfig.Enum;
using GameFramework.Game;
using GameFramework.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGameLogic.Managers.AppManagers;
using CoreGameLogic.Net;
using Cysharp.Threading.Tasks;
using GameFramework.Network.Models.Vo;
using GameFramework.Network.Proxy;
using UnityEngine;

/// <summary>
/// 玩家数据管理器
/// </summary>
public class PlayerDataManager : IAppManager
{
    /// <summary>
    /// 三千盘金币数量
    /// </summary>
    private int _threeKPCoinAmount;
    public int ThreeKPCoinAmount => _threeKPCoinAmount;

    /// <summary>
    /// 三千盘积分数量
    /// </summary>
    private int _pointAmount;
    public int PointAmount => _pointAmount;
    
    /// <summary>
    /// 平台用户信息
    /// </summary>
    private PlatformUserInfo _platformUserInfo;

    /// <summary>
    /// 各道具数量
    /// </summary>
    private readonly Dictionary<EPropType, int> _propCounts = new Dictionary<EPropType, int>();

    /// <summary>
    /// 各道具远程ID
    /// </summary>
    private readonly Dictionary<EPropType, string> _propRemoteIds = new Dictionary<EPropType, string>();
    
    private EventManager _eventManager;
    private MallManager _mallManager;
    private PlatformManager _platformManager;

    public void Init()
    {
        RegisterServices();
        RegisterEvents();

        // TODO 未来从服务器获取玩家数据
        FetchRemoteData().Forget();

        // Log.Info("[PlayerDataManager] 初始化完成");
        Debug.Log($"[PlayerDataManager] 初始化完成");
    }

    /// <summary>
    /// 从服务器获取玩家数据
    /// </summary>
    private async UniTask FetchRemoteData()
    {
        // 等待登录完成
        await UniTask.WaitUntil(() => HuntingGameServiceProxy.Instance.IsClientLoggedIn);
        
        // 拉取玩家三币和积分
        _threeKPCoinAmount = (int)UserProxy.Instance.Threekp;
        ProgressData progressData = await GameProxy.Instance.GetProgressData();
        _pointAmount = (int)progressData.Point;

        Debug.Log($"[PlayerDataManager] 获取玩家数据完成，三币：{ThreeKPCoinAmount}，积分：{PointAmount}]");
        
        // _threeKPCoinAmount = 50;
        // foreach (EPropType propType in Enum.GetValues(typeof(EPropType)))
        //     _propCounts[propType] = 3;
        
        // 同步玩家道具数据
        await SyncPropsFromServer();
    }
    
    /// <summary>
    /// 设置平台用户信息 - 在用户按下授权按钮之后调用
    /// </summary>
    /// <param name="platformUserInfo"></param>
    public void SetPlatformUserInfo(PlatformUserInfo platformUserInfo)
    {
        _platformUserInfo = platformUserInfo;
    }

    /// <summary>
    /// 同步平台用户信息
    /// </summary>
    public void SyncPlatformUserInfo()
    {
        _platformManager.CurrentPlatform.GetUserInfo(result =>
        {
            Debug.Log($"[PlayerDataManager] 同步平台用户信息成功: {result.AvatarUrl}, {result.NickName}");
            PlatformUserInfo platformUserInfo = new PlatformUserInfo()
            {
                AvatarUrl = result.AvatarUrl,
                NickName = result.NickName,
                City = result.City,
                Province = result.Province,
                Gender = result.Gender,
                Language = result.Language,
            };
            SetPlatformUserInfo(platformUserInfo);
        }, error =>
        {
            Debug.Log($"[PlayerDataManager] 同步平台用户信息失败: {error}");
        });
    }

    public void Dispose()
    {
        UnregisterEvents();
        Log.Info("[PlayerDataManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 获取三千盘金币数量
    /// </summary>
    public int GetThreeKPCoinAmount()
    {
        return _threeKPCoinAmount;
    }

    /// <summary>
    /// 更新三千盘金币数量
    /// </summary>
    public void UpdateThreeKPCoinAmount(int amount)
    {
        int oldAmount = _threeKPCoinAmount;
        _threeKPCoinAmount += amount;
        int deltaAmount = _threeKPCoinAmount - oldAmount;
        TriggerThreeKPCoinAmountChanged(new ThreeKPCoinAmountChangedEventArgs
        {
            CurrentAmount = _threeKPCoinAmount,
            DeltaAmount = deltaAmount
        });
    }

    /// <summary>
    /// 获取道具数量
    /// </summary>
    public int GetPropCount(EPropType propType)
    {
        return _propCounts[propType];
    }

    /// <summary>
    /// 更新道具数量
    /// </summary>
    public void UpdatePropCount(EPropType propType, int amount)
    {
        int oldAmount = GetPropCount(propType);
        _propCounts[propType] = oldAmount + amount;
        int currentAmount = _propCounts[propType];
        TriggerPropCountChanged(new PropCountChangedEventArgs
        {
            PropType = propType,
            CurrentAmount = currentAmount,
            DeltaAmount = amount
        });
    }
    
    /// <summary>
    /// 金币是否足够
    /// </summary>
    /// <param name="priceCoin"></param>
    /// <returns></returns>
    public bool EnoughThreeKp(int priceCoin)
    {
        return _threeKPCoinAmount >= priceCoin;
    }
    
    #endregion

    #region 私有方法
    private void RegisterServices()
    {
        _eventManager = GameServiceLocator.EventManager;
        _mallManager = GameServiceLocator.GetAppManager<MallManager>();
        _platformManager = GameServiceLocator.PlatformManager;
    }

    private void RegisterEvents()
    {
        _eventManager.AddListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }

    private void UnregisterEvents()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalDropReward, OnAnimalDropReward);
    }
    
    /// <summary>
    /// 从远端同步道具数据
    /// </summary>
    private async UniTask SyncPropsFromServer()
    {
        Debug.Log($"[PlayerDataManager] 从服务器获取道具数据");
        var serverItems = await GameProxy.Instance.GetItemData();

        Debug.Log($"[PlayerDataManager] 打印服务器的道具数据");
        foreach (var serverItem in serverItems)
        {
            Debug.Log($"[PlayerDataManager] 服务器道具id: {serverItem.Key}, 道具数量: {serverItem.Value}");
        }
        
        Debug.Log($"[PlayerDataManager] 从服务器获取道具数据完成");

        // 等待商城物品拉取成功，映射 itemId
        await UniTask.WaitUntil(() => _mallManager.IsFetchedMallItemSuccess);
        List<ItemVo> itemVoList = _mallManager.GetPropItemVoList();

        foreach (var itemVo in itemVoList)
        {
            Debug.Log($"[PlayerDataManager] 打印物品, id: {itemVo.ItemID}, name: {itemVo.Name}");
            EPropType propType = GetPropsTypeFromName(itemVo.Name);
            // 同步远端道具ID
            _propRemoteIds[propType] = itemVo.ItemID;
            // 同步远端道具数量
            _propCounts[propType] = (int)serverItems[itemVo.ItemID];
            Debug.Log($"[PlayerDataManager] 道具：{propType}, id: {itemVo.ItemID}");
        }

        Debug.Log($"[PlayerDataManager] 映射道具数据完成------");
    }
    
    private EPropType GetPropsTypeFromName(string propName)
    {
        switch (propName)
        {
            case "炮火轰炸": return EPropType.Bombardment;
            case "指哪打哪": return EPropType.AimAssist;
            case "智能诱捕陷阱": return EPropType.Trap;
        }

        return default;
    }

    private void OnAnimalDropReward(AnimalDropRewardEventArgs args)
    {
        if (args.DropRewards.TryGetValue(EDropType.ThreeKPCoin, out int count) && count > 0)
            UpdateThreeKPCoinAmount(count);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 触发三千盘金币数量改变事件
    /// </summary>
    private void TriggerThreeKPCoinAmountChanged(ThreeKPCoinAmountChangedEventArgs args)
    {
        _eventManager.Trigger(PlayerDataEvents.ThreeKPCoinAmountChanged, args);
    }

    /// <summary>
    /// 触发道具数量改变事件
    /// </summary>
    private void TriggerPropCountChanged(PropCountChangedEventArgs args)
    {
        _eventManager.Trigger(PlayerDataEvents.PropCountChanged, args);
    }
    #endregion

    
}
