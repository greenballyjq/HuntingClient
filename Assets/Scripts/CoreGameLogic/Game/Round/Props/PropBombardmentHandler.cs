using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 炮火轰炸道具处理器
/// </summary>
public class PropBombardmentHandler : BasePropHandler
{
    /// <summary>
    /// 轰炸范围半径
    /// </summary>
    private float _zoneRadius;

    /// <summary>
    /// 伤害值
    /// </summary>
    private float _damageAmount;

    /// <summary>
    /// 伤害间隔
    /// </summary>
    private float _damageInterval;

    /// <summary>
    /// 开火距离
    /// </summary>
    private float _fireDistance;

    /// <summary>
    /// 伤害计时器
    /// </summary>
    private float _damageTimer;

    /// <summary>
    /// 轰炸位置
    /// </summary>
    private Vector3 _bombardmentPos;

    private EffectManager _effectManager;

    public PropBombardmentHandler()
    {
        _effectManager = GameServiceLocator.EffectManager;
    }

    /// <summary>
    /// 道具开始钩子
    /// </summary>
    protected override UniTask OnPropStart(Prop propData)
    {
        PropBombardment parameter = _configManager.GetPropBombardment(propData.ParamTableID);
        _zoneRadius = parameter.ZoneRadius;
        _fireDistance = 8;
        _damageAmount = parameter.DamageAmount;
        _damageInterval = parameter.DamageInterval;

        _bombardmentPos = _player.position + _player.forward * _fireDistance;

        GameObject effectPrefab = _configManager.PropRefSo.GetPropEffectPrefab(propData.ID);
        _effectManager.PlayOneShot(effectPrefab, _bombardmentPos);

        _damageTimer = 0f;

        return UniTask.CompletedTask;
    }

    /// <summary>
    /// 道具更新钩子
    /// </summary>
    protected override void OnPropUpdate(float dt)
    {
        _damageTimer += dt;
        if (_damageTimer < _damageInterval)
            return;

        ApplyBombardmentDamage();
        _damageTimer = 0f;
    }

    /// <summary>
    /// 道具结束钩子
    /// </summary>
    protected override void OnPropEnd() { }

    #region 私有方法

    /// <summary>
    /// 应用轰炸范围伤害
    /// </summary>
    private void ApplyBombardmentDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(_bombardmentPos, _zoneRadius, LayerMask.GetMask("Animal"));

        foreach (Collider collider in colliders)
            collider.GetComponent<IDamageable>().TakeDamage(_damageAmount);
    }

    #endregion
}
