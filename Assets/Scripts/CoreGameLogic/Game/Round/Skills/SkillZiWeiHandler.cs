using System.Collections.Generic;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 紫薇技能处理器
/// </summary>
public class SkillZiWeiHandler : BaseSkillHandler
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        var skillParam = _configManager.GetSkillZiWei(context.SkillData.ParamTableID);
        CreateSkillWeaponsAsync(skillParam).Forget();

        await UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt) {}

    protected override void OnSkillEnd()
    {
        DestroyAllSkillWeapons();
    }


    #region 私有方法
    /// <summary>
    /// 创建技能武器
    /// </summary>
    private async UniTask CreateSkillWeaponsAsync(SkillZiWei skillParam)
    {
        // 加载或使用缓存的武器预制体
        if (_weaponPrefabCache == null)
            _weaponPrefabCache = await _resourceManager.LoadAssetAsync<GameObject>(SkillWeaponPrefabPath);

        // 计算生成位置并创建武器
        int gunCountPerSide = Mathf.RoundToInt(skillParam.GunCountPerSide);
        Vector3 playerPosition = _player.position;

        Vector3 worldRight = Vector3.right;

        // 创建左侧武器
        for (int i = 0; i < gunCountPerSide; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition(playerPosition, worldRight, skillParam.GunOffsetX, i, true);
            CreateSkillWeapon(spawnPosition, skillParam.FireInterval);
        }

        // 创建右侧武器
        for (int i = 0; i < gunCountPerSide; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition(playerPosition, worldRight, skillParam.GunOffsetX, i, false);
            CreateSkillWeapon(spawnPosition, skillParam.FireInterval);
        }
    }

    /// <summary>
    /// 创建单个技能武器
    /// </summary>
    private void CreateSkillWeapon(Vector3 position, float fireInterval)
    {
        var weaponObj = GameObject.Instantiate(_weaponPrefabCache);
        weaponObj.transform.position = position;

        var skillWeapon = weaponObj.GetComponent<SkillWeapon>();
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
        Vector3 position = playerPosition + playerRight * offset;
        return position;
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
    }
    #endregion

    #region 测试
    /// <summary>
    /// 技能武器预制体资源路径
    /// </summary>
    private const string SkillWeaponPrefabPath = "Assets/Arts/Prefabs/Skills/pf_skillweapon";

    /// <summary>
    /// 技能武器预制体缓存
    /// </summary>
    private GameObject _weaponPrefabCache;

    /// <summary>
    /// 技能武器列表
    /// </summary>
    private List<SkillWeapon> _skillWeapons = new List<SkillWeapon>();

    #endregion
}