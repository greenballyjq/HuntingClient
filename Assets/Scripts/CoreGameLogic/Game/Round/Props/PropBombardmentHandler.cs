using System.Runtime.CompilerServices;
using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 炮火轰炸道具处理器
/// </summary>
public class PropBombardmentHandler : IPropHandler
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

    /// <summary>
    /// 玩家组件
    /// </summary>
    private Transform _player;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager;

    /// <summary>
    /// 玩家管理器
    /// </summary>
    private GameplaySceneItemManager _playerManager => GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();

    public PropBombardmentHandler()
    {
        _configManager = GameServiceLocator.ConfigManager;
        _effectManager = GameServiceLocator.EffectManager;
    }

    /// <summary>
    /// 道具效果开始
    /// </summary>
    public void OnPropStart(PropContext context)
    {
        _player = _playerManager.Player;

        // 读取配置参数
        PropBombardment parameter = _configManager.GetPropBombardment(context.PropData.ParamTableID);
        _zoneRadius = parameter.ZoneRadius;
        _fireDistance = parameter.FireDistance;
        _damageAmount = parameter.DamageAmount;
        _damageInterval = parameter.DamageInterval;

        // 计算轰炸中心点
        _bombardmentPos = CalculateBombardmentCenter();

        // 播放特效
        _effectManager.PlayOneShotAsync(parameter.EffectPrefabPath, _bombardmentPos).Forget();

        _damageTimer = 0f;
    }

    /// <summary>
    /// 道具效果更新
    /// </summary>
    public void OnPropUpdate(PropContext context, float deltaTime)
    {
        _damageTimer += deltaTime;
        if (_damageTimer < _damageInterval)
            return;

        // 执行范围伤害
        ApplyBombardmentDamage();
        _damageTimer = 0f;
    }

    /// <summary>
    /// 道具效果结束
    /// </summary>
    public void OnPropEnd(PropContext context) { }

    #region 私有方法
    /// <summary>
    /// 计算轰炸中心点
    /// </summary>
    private Vector3 CalculateBombardmentCenter()
    {
        return _player.position + _player.forward * _fireDistance;
    }

    /// <summary>
    /// 应用轰炸范围伤害
    /// </summary>
    private void ApplyBombardmentDamage()
    {
        // 检测范围内的所有动物
        Collider[] colliders = Physics.OverlapSphere(_bombardmentPos, _zoneRadius, LayerMask.GetMask("Animal"));

        // 对范围内的动物造成伤害
        foreach (Collider collider in colliders)
            collider.GetComponent<IDamageable>().TakeDamage(_damageAmount);
    }
    #endregion
}