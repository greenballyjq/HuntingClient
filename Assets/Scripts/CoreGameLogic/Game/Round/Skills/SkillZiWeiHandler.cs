using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 紫薇技能处理器
/// </summary>
public class SkillZiWeiHandler : BaseSkillHandler
{
    /// <summary>
    /// 武器预制体
    /// </summary>
    private GameObject _weaponPrefab;

    /// <summary>
    /// 武器列表
    /// </summary>
    private List<SkillWeapon> _weapons = new List<SkillWeapon>();

    /// <summary>
    /// X轴偏移
    /// </summary>
    private const float WEAPON_OFFSET_X = 2f;

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        var skillParam = _configManager.GetSkillZiWei(context.SkillData.ParamTableID);

        // 缓存武器预制体
        _weaponPrefab = await _resourceManager.LoadAssetAsync<GameObject>(skillParam.SkillPrefabResourcePath);

        // 生成武器
        CreateWeapons(skillParam);

        // 模拟播放动画
        await UniTask.Delay(1000);
    }

    protected override void OnSkillUpdate(float dt)
    {
        foreach (var weapon in _weapons)
            weapon.DoUpdate(dt);
    }

    protected override void OnSkillEnd()
    {
        DestroyAllWeapons();
        _weaponPrefab = null;
    }

    #region 私有方法
    /// <summary>
    /// 生成武器
    /// </summary>
    private void CreateWeapons(SkillZiWei skillParam)
    {
        Vector3 playerPos = _player.position;
        Vector3 playerRight = Vector3.right;

        // 创建左侧武器
        for (int i = 0; i < skillParam.WeaponCountPerSide; i++)
        {
            Vector3 spawnPos = playerPos + playerRight * (-WEAPON_OFFSET_X * (i + 1));
            CreateWeapon(spawnPos, skillParam.FireInterval);
        }

        // 创建右侧武器
        for (int i = 0; i < skillParam.WeaponCountPerSide; i++)
        {
            Vector3 spawnPos = playerPos + playerRight * (WEAPON_OFFSET_X * (i + 1));
            CreateWeapon(spawnPos, skillParam.FireInterval);
        }
    }

    /// <summary>
    /// 创建单个武器
    /// </summary>
    private void CreateWeapon(Vector3 position, float fireInterval)
    {
        GameObject weaponObj = GameObject.Instantiate(_weaponPrefab);
        weaponObj.transform.position = position;

        var weapon = weaponObj.GetComponent<SkillWeapon>();
        weapon.Init(fireInterval);
        _weapons.Add(weapon);
    }

    /// <summary>
    /// 销毁所有武器
    /// </summary>
    private void DestroyAllWeapons()
    {
        foreach (var weapon in _weapons)
            GameObject.Destroy(weapon.gameObject);
            
        _weapons.Clear();
    }
    #endregion
}
