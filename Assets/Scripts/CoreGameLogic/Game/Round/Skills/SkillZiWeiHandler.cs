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
    /// 技能参数
    /// </summary>
    private SkillZiWei _skillParam;

    /// <summary>
    /// 武器预制体
    /// </summary>
    private GameObject _weaponPrefab;

    /// <summary>
    /// 武器列表
    /// </summary>
    private List<SkillWeapon> _weapons = new List<SkillWeapon>();

    private const float WEAPON_OFFSET_X = 2f;

    protected override void OnInit() 
    {
        _skillParam = ConfigManager.GetSkillZiWei(SkillContext.SkillData.ParamTableID);
        _weaponPrefab = ConfigManager.SkillRefSo.GetSkillPrefab(SkillContext.SkillData.ID);
    }

    protected override UniTask OnSkillStart()
    {
        CreateWeapons();

        return UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt)
    {
        foreach (var weapon in _weapons)
            weapon.DoUpdate(dt);
    }

    protected override void OnSkillEnd()
    {
        DestroyAllWeapons();
    }

    #region 私有方法
    /// <summary>
    /// 创建武器
    /// </summary>
    private void CreateWeapons()
    {
        Vector3 playerPos = Player.position;
        Vector3 playerRight = Vector3.right;

        Vector3 spawnPos;

        for (int i = 0; i < _skillParam.WeaponCountPerSide; i++)
        {
            spawnPos = playerPos + playerRight * (-WEAPON_OFFSET_X * (i + 1));
            CreateWeapon(spawnPos, _skillParam.FireInterval);
        }

        for (int i = 0; i < _skillParam.WeaponCountPerSide; i++)
        {
            spawnPos = playerPos + playerRight * (WEAPON_OFFSET_X * (i + 1));
            CreateWeapon(spawnPos, _skillParam.FireInterval);
        }
    }

    /// <summary>
    /// 创建单个武器
    /// </summary>
    private void CreateWeapon(Vector3 position, float fireInterval)
    {
        GameObject weaponObj = Object.Instantiate(_weaponPrefab);
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
            Object.Destroy(weapon.gameObject);
            
        _weapons.Clear();
    }
    #endregion
}
