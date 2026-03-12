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
    /// 技能参数缓存
    /// </summary>
    private SkillZiWei _skillParamCache;

    /// <summary>
    /// 技能预制体缓存
    /// </summary>
    private GameObject _skillPrefabCache;

    /// <summary>
    /// 武器列表
    /// </summary>
    private List<SkillWeapon> _weapons = new List<SkillWeapon>();

    private const float WEAPON_OFFSET_X = 2f;

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        if(_skillParamCache == null)
            _skillParamCache = _configManager.GetSkillZiWei(context.SkillData.ParamTableID);

        if(_skillPrefabCache == null)
            _skillPrefabCache = _configManager.SkillRefSo.GetSkillPrefab(context.SkillData.ID);

        CreateWeapons();

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
    }

    #region 私有方法
    /// <summary>
    /// 创建武器
    /// </summary>
    private void CreateWeapons()
    {
        Vector3 playerPos = _player.position;
        Vector3 playerRight = Vector3.right;

        Vector3 spawnPos;

        for (int i = 0; i < _skillParamCache.WeaponCountPerSide; i++)
        {
            spawnPos = playerPos + playerRight * (-WEAPON_OFFSET_X * (i + 1));
            CreateWeapon(spawnPos, _skillParamCache.FireInterval);
        }

        for (int i = 0; i < _skillParamCache.WeaponCountPerSide; i++)
        {
            spawnPos = playerPos + playerRight * (WEAPON_OFFSET_X * (i + 1));
            CreateWeapon(spawnPos, _skillParamCache.FireInterval);
        }
    }

    /// <summary>
    /// 创建单个武器
    /// </summary>
    private void CreateWeapon(Vector3 position, float fireInterval)
    {
        GameObject weaponObj = Object.Instantiate(_skillPrefabCache);
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
