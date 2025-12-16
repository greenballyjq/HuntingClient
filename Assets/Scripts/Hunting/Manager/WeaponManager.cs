using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using GameFramework.Core;
using GameFramework.Game;
using Hunting.Game.Weapons;
using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 武器管理器
    /// </summary>
    public class WeaponManager : BaseGameManager
    {
        /// <summary>
        /// 主武器实例
        /// </summary>
        private MainWeapon _mainWeapon;

        /// <summary>
        /// 武器预制体资源路径
        /// </summary>
        private const string WeaponPrefabPath = "Arts/Prefabs/Weapons/MainWeapon";

        /// <summary>
        /// 武器预制体缓存
        /// </summary>
        private GameObject _weaponPrefabCache;

        /// <summary>
        /// 射速修正倍率字典
        /// key: 来源ID
        /// value: 倍率值
        /// </summary>
        private Dictionary<string, float> _fireRateModifiers = new Dictionary<string, float>();

        /// <summary>
        /// 伤害修正倍率字典
        /// key: 来源ID
        /// value: 倍率值
        /// </summary>
        private Dictionary<string, float> _damageModifiers = new Dictionary<string, float>();

        /// <summary>
        /// 当前射速修正倍率
        /// </summary>
        private float _fireRateMultiplier = 1f;

        /// <summary>
        /// 当前伤害修正倍率
        /// </summary>
        private float _damageMultiplier = 1f;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager Resource => GameServiceLocator.Resource;

        public override void Init()
        {
            RegisterEvents();
            ResetState();
            Debug.Log("[WeaponManager] 初始化完成");
        }

        public override void Update()
        {
            // 当前无需逐帧逻辑
        }

        public override void Release()
        {
            UnregisterEvents();
            DestroyMainWeapon();
            ClearPrefabCache();
            ResetState();
            Debug.Log("[WeaponManager] 已释放");
        }

        #region 公共方法
        /// <summary>
        /// 获取主武器实例
        /// </summary>
        public MainWeapon GetMainWeapon()
        {
            return _mainWeapon;
        }

        /// <summary>
        /// 注册射速修正倍率
        /// </summary>
        /// <param name="sourceId">修正来源ID（如技能类型、幸运仪式类型等）</param>
        /// <param name="multiplier">倍率值</param>
        public void RegisterFireRateModifier(string sourceId, float multiplier)
        {
            _fireRateModifiers[sourceId] = multiplier;
            UpdateFireRateMultiplier();
            Debug.Log($"[WeaponManager] 注册射速修正，来源:{sourceId}，倍率:{multiplier:F2}，当前总倍率:{_fireRateMultiplier:F2}");
        }

        /// <summary>
        /// 注销射速修正倍率
        /// </summary>
        /// <param name="sourceId">修正来源ID</param>
        public void UnregisterFireRateModifier(string sourceId)
        {
            if (_fireRateModifiers.Remove(sourceId))
            {
                UpdateFireRateMultiplier();
                Debug.Log($"[WeaponManager] 注销射速修正，来源:{sourceId}，当前总倍率:{_fireRateMultiplier:F2}");
            }
        }

        /// <summary>
        /// 注册伤害修正倍率
        /// </summary>
        /// <param name="sourceId">修正来源ID（如技能类型、幸运仪式类型等）</param>
        /// <param name="multiplier">倍率值</param>
        public void RegisterDamageModifier(string sourceId, float multiplier)
        {
            _damageModifiers[sourceId] = multiplier;
            UpdateDamageMultiplier();
            Debug.Log($"[WeaponManager] 注册伤害修正，来源:{sourceId}，倍率:{multiplier:F2}，当前总倍率:{_damageMultiplier:F2}");
        }

        /// <summary>
        /// 注销伤害修正倍率
        /// </summary>
        /// <param name="sourceId">修正来源ID</param>
        public void UnregisterDamageModifier(string sourceId)
        {
            if (_damageModifiers.Remove(sourceId))
            {
                UpdateDamageMultiplier();
                Debug.Log($"[WeaponManager] 注销伤害修正，来源:{sourceId}，当前总倍率:{_damageMultiplier:F2}");
            }
        }

        /// <summary>
        /// 获取修正后的射速
        /// </summary>
        public float GetCurrentFireRate(int bulletId)
        {
            var bulletData = Config.GetBullet(bulletId);
            if (bulletData == null)
            {
                Debug.LogWarning($"[WeaponManager] 子弹配置不存在: {bulletId}");
                return 0f;
            }

            return bulletData.FireRate * _fireRateMultiplier;
        }

        /// <summary>
        /// 获取修正后的伤害
        /// </summary>
        public float GetCurrentDamage(int bulletId)
        {
            var bulletData = Config.GetBullet(bulletId);
            if (bulletData == null)
            {
                Debug.LogWarning($"[WeaponManager] 子弹配置不存在: {bulletId}");
                return 0f;
            }

            return bulletData.BaseDamage * _damageMultiplier;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 创建主武器
        /// </summary>
        private async UniTask CreateMainWeaponAsync()
        {
            if (_mainWeapon != null)
            {
                Debug.LogWarning("[WeaponManager] 主武器已存在，跳过创建");
                return;
            }

            // 查找Player标签的GameObject
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                Debug.LogError("[WeaponManager] 未找到Player标签的GameObject");
                return;
            }

            // 加载或使用缓存的武器预制体
            if (_weaponPrefabCache == null)
            {
                _weaponPrefabCache = await Resource.LoadAssetAsync<GameObject>(WeaponPrefabPath);
                if (_weaponPrefabCache == null)
                {
                    Debug.LogError($"[WeaponManager] 加载武器预制体失败: {WeaponPrefabPath}");
                    return;
                }
            }

            // 实例化武器并挂载到Player对象上
            var weaponObj = GameObject.Instantiate(_weaponPrefabCache, playerObj.transform);
            weaponObj.name = "MainWeapon";

            // 获取武器组件
            _mainWeapon = weaponObj.GetComponent<MainWeapon>();
            if (_mainWeapon == null)
            {
                Debug.LogError("[WeaponManager] 武器预制体缺少 MainWeapon 组件");
                GameObject.Destroy(weaponObj);
                return;
            }

            Debug.Log("[WeaponManager] 主武器创建完成");
        }

        /// <summary>
        /// 销毁主武器
        /// </summary>
        private void DestroyMainWeapon()
        {
            if (_mainWeapon == null)
                return;

            GameObject.Destroy(_mainWeapon.gameObject);
            _mainWeapon = null;
            Debug.Log("[WeaponManager] 主武器已销毁");
        }

        /// <summary>
        /// 清除预制体缓存
        /// </summary>
        private void ClearPrefabCache()
        {
            _weaponPrefabCache = null;
        }

        /// <summary>
        /// 更新射速修正倍率
        /// </summary>
        private void UpdateFireRateMultiplier()
        {
            float total = 1f;
            foreach (var modifier in _fireRateModifiers.Values)
            {
                total *= modifier;
            }
            _fireRateMultiplier = total;

            // 通知主武器更新射速
            if (_mainWeapon != null)
                _mainWeapon.OnFireRateChanged();
        }

        /// <summary>
        /// 更新伤害修正倍率
        /// </summary>
        private void UpdateDamageMultiplier()
        {
            float total = 1f;
            foreach (var modifier in _damageModifiers.Values)
            {
                total *= modifier;
            }
            _damageMultiplier = total;
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        private void ResetState()
        {
            _fireRateModifiers.Clear();
            _damageModifiers.Clear();
            _fireRateMultiplier = 1f;
            _damageMultiplier = 1f;
        }
        #endregion

        #region 事件相关
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            Event.AddListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.AddListener(RoundEvents.RoundEnded, OnRoundEnded);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void UnregisterEvents()
        {
            Event.RemoveListener(RoundEvents.RoundStarted, OnRoundStarted);
            Event.RemoveListener(RoundEvents.RoundEnded, OnRoundEnded);
        }

        /// <summary>
        /// 单局开始回调
        /// </summary>
        private async void OnRoundStarted(RoundStartedEventArgs args)
        {
            ResetState();
            await CreateMainWeaponAsync();
        }

        /// <summary>
        /// 单局结束回调
        /// </summary>
        private void OnRoundEnded(RoundEndedEventArgs args)
        {
            DestroyMainWeapon();
            ResetState();
        }
        #endregion
    }
}

