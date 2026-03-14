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
    /// 道具参数
    /// </summary>
    private PropBombardment _propParam;

    /// <summary>
    /// 轰炸特效预制体
    /// </summary>
    private GameObject _bombardmentEffectPrefab;

    /// <summary>
    /// 轰炸区域位置列表
    /// </summary>
    private Vector3[] _zonePositions;

    /// <summary>
    /// 伤害计时器
    /// </summary>
    private float _damageTimer;

    private const float ZONE_MIN_DISTANCE = 8f;
    private const float AREA_BOUNDARY_OFFSET = -24f;
    private const int MAX_ATTEMPTS = 20;

    protected override  void OnInit()
    {
        _propParam = ConfigManager.GetPropBombardment(PropData.ParamTableID);
        _bombardmentEffectPrefab = ConfigManager.PropRefSo.GetPropEffectPrefab(PropData.ID);
    }

    protected override UniTask OnPropStart()
    {
        _damageTimer = 0f;

        CreateBombardmentZones();

        return UniTask.CompletedTask;
    }

    protected override void OnPropUpdate(float dt)
    {
        _damageTimer += dt;

        if (_damageTimer < _propParam.DamageInterval)
            return;

        _damageTimer = 0f;

        ApplyBombardmentDamage();
    }

    protected override void OnPropEnd() { }

    #region 私有方法
    /// <summary>
    /// 创建轰炸区域
    /// </summary>
    private void CreateBombardmentZones()
    {
        _zonePositions = new Vector3[_propParam.ZoneCount];

        for (int i = 0; i < _propParam.ZoneCount; i++)
        {
            Vector3 position = default;
            for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
            {
                position = PlayableArea.GetRandomPoint(100, AREA_BOUNDARY_OFFSET);
                if (IsValidZonePosition(position, i))
                    break;
            }
            // 超限未找到有效位置时，用最后一次随机点兜底
            _zonePositions[i] = position;
            EffectManager.PlayOneShot(_bombardmentEffectPrefab, _zonePositions[i]);
        }
    }

    /// <summary>
    /// 应用轰炸伤害
    /// </summary>
    private void ApplyBombardmentDamage()
    {
        for (int i = 0; i < _zonePositions.Length; i++)
        {
            Collider[] colliders = Physics.OverlapSphere(_zonePositions[i], _propParam.ZoneRadius, LayerMask.GetMask("Animal"));

            foreach (var collider in colliders)
                collider.GetComponent<IDamageable>().TakeDamage(_propParam.DamageAmount);
        }
    }

    /// <summary>
    /// 是否是有效轰炸区域
    /// </summary>
    private bool IsValidZonePosition(Vector3 position, int existingCount)
    {
        for (int i = 0; i < existingCount; i++)
        {
            if (Vector3.Distance(position, _zonePositions[i]) < ZONE_MIN_DISTANCE)
                return false;
        }
        return true;
    }
    #endregion
}
