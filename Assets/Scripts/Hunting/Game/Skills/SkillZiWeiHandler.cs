using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using cfg.HuntingConfig.Skill;
using GameFramework.Core;
using Hunting.Game.Weapons;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Skills
{
    /// <summary>
    /// 紫薇技能处理器
    /// </summary>
    public class SkillZiWeiHandler : ISkillHandler
    {
        /// <summary>
        /// 技能武器预制体资源路径
        /// </summary>
        private const string SkillWeaponPrefabPath = "Arts/Prefabs/Weapons/SkillWeapon";

        /// <summary>
        /// 技能武器列表
        /// </summary>
        private List<SkillWeapon> _skillWeapons = new List<SkillWeapon>();

        /// <summary>
        /// 技能武器预制体缓存
        /// </summary>
        private GameObject _weaponPrefabCache;

        /// <summary>
        /// 玩家Transform
        /// </summary>
        private Transform _playerTransform;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager Resource => GameServiceLocator.Resource;

        /// <summary>
        /// 技能开始
        /// </summary>
        public void OnSkillStart(SkillContext context)
        {
            CreateSkillWeaponsAsync(context).Forget();
        }

        /// <summary>
        /// 技能更新
        /// </summary>
        public void OnSkillUpdate(SkillContext context, float deltaTime)
        {
            // 当前技能无需逐帧逻辑
        }

        /// <summary>
        /// 技能结束
        /// </summary>
        public void OnSkillEnd(SkillContext context)
        {
            DestroyAllSkillWeapons();
        }

        #region 私有方法
        /// <summary>
        /// 创建技能武器
        /// </summary>
        private async UniTask CreateSkillWeaponsAsync(SkillContext context)
        {
            var parameter = Config.GetSkillZiWei(context.SkillData.ParamTableID);
            if (parameter == null)
            {
                Debug.LogWarning("[SkillZiWeiHandler] 未找到技能参数配置");
                return;
            }

            var player = FindPlayerTransform();
            if (player == null)
            {
                Debug.LogWarning("[SkillZiWeiHandler] 未找到玩家Transform");
                return;
            }

            // 加载或使用缓存的武器预制体
            if (_weaponPrefabCache == null)
            {
                _weaponPrefabCache = await Resource.LoadAssetAsync<GameObject>(SkillWeaponPrefabPath);
                if (_weaponPrefabCache == null)
                {
                    Debug.LogError($"[SkillZiWeiHandler] 加载技能武器预制体失败: {SkillWeaponPrefabPath}");
                    return;
                }
            }

            // 计算生成位置并创建武器
            int gunCountPerSide = Mathf.RoundToInt(parameter.GunCountPerSide);
            Vector3 playerPosition = player.position;
            Vector3 playerRight = player.right;

            // 创建左侧武器
            for (int i = 0; i < gunCountPerSide; i++)
            {
                Vector3 spawnPosition = CalculateSpawnPosition(playerPosition, playerRight, parameter.GunOffsetX, i, true);
                CreateSkillWeapon(spawnPosition, parameter.FireInterval);
            }

            // 创建右侧武器
            for (int i = 0; i < gunCountPerSide; i++)
            {
                Vector3 spawnPosition = CalculateSpawnPosition(playerPosition, playerRight, parameter.GunOffsetX, i, false);
                CreateSkillWeapon(spawnPosition, parameter.FireInterval);
            }

            Debug.Log($"[SkillZiWeiHandler] 创建了 {_skillWeapons.Count} 把技能武器");
        }

        /// <summary>
        /// 创建单个技能武器
        /// </summary>
        private void CreateSkillWeapon(Vector3 position, float fireInterval)
        {
            var weaponObj = GameObject.Instantiate(_weaponPrefabCache);
            weaponObj.transform.position = position;
            weaponObj.name = "SkillWeapon";

            var skillWeapon = weaponObj.GetComponent<SkillWeapon>();
            if (skillWeapon == null)
            {
                Debug.LogError("[SkillZiWeiHandler] 技能武器预制体缺少 SkillWeapon 组件");
                GameObject.Destroy(weaponObj);
                return;
            }

            skillWeapon.Init(fireInterval);
            _skillWeapons.Add(skillWeapon);
        }

        /// <summary>
        /// 计算生成位置
        /// </summary>
        /// <param name="playerPosition">玩家位置</param>
        /// <param name="playerRight">玩家右侧方向</param>
        /// <param name="offsetX">水平偏移</param>
        /// <param name="index">索引</param>
        /// <param name="isLeft">是否为左侧</param>
        private Vector3 CalculateSpawnPosition(Vector3 playerPosition, Vector3 playerRight, float offsetX, int index, bool isLeft)
        {
            float offset = (index + 1) * offsetX;
            if (isLeft)
                offset = -offset;
            return playerPosition + playerRight * offset;
        }

        /// <summary>
        /// 销毁所有技能武器
        /// </summary>
        private void DestroyAllSkillWeapons()
        {
            foreach (var weapon in _skillWeapons)
            {
                if (weapon != null)
                    GameObject.Destroy(weapon.gameObject);
            }
            _skillWeapons.Clear();
            Debug.Log("[SkillZiWeiHandler] 已销毁所有技能武器");
        }

        /// <summary>
        /// 获取玩家Transform
        /// </summary>
        private Transform FindPlayerTransform()
        {
            if (_playerTransform == null)
                _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            return _playerTransform;
        }
        #endregion
    }
}

