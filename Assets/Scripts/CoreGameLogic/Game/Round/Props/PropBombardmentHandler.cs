using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
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
    /// 轰炸中心点
    /// </summary>
    private Vector3 _bombardmentCenter;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager => GameServiceLocator.GetFrameworkManager<EffectManager>();

    /// <summary>
    /// 道具效果开始
    /// </summary>
    public void OnPropStart(PropContext context)
    {
        Transform playerTransform = FindPlayerTransform();

        // 读取配置参数
        PropBombardment parameter = _configManager.GetPropBombardment(context.PropData.ParamTableID);
        _zoneRadius = parameter.ZoneRadius;
        _fireDistance = parameter.FireDistance;
        _damageAmount = parameter.DamageAmount;
        _damageInterval = parameter.DamageInterval;

        // 计算轰炸中心点
        _bombardmentCenter = CalculateBombardmentCenter();

        // 播放特效
        _effectManager.PlayOneShotAsync(parameter.EffectPrefabPath, _bombardmentCenter, Quaternion.identity).Forget();

        // 创建范围指示器（测试用）
        CreateRangeIndicator();

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
    public void OnPropEnd(PropContext context)
    {
        // 销毁范围指示器（测试用）
        DestroyRangeIndicator();
    }

    #region 私有方法
    /// <summary>
    /// 计算轰炸中心点
    /// </summary>
    private Vector3 CalculateBombardmentCenter()
    {
        return _playerTransform.position + _playerTransform.forward * _fireDistance;
    }

    /// <summary>
    /// 应用轰炸范围伤害
    /// </summary>
    private void ApplyBombardmentDamage()
    {
        // 检测范围内的所有动物
        Collider[] colliders = Physics.OverlapSphere(_bombardmentCenter, _zoneRadius, LayerMask.GetMask("Animal"));

        // 对范围内的动物造成伤害
        foreach (Collider collider in colliders)
            collider.GetComponent<AnimalBehavior>().TakeDamage(_damageAmount, _bombardmentCenter);
    }
    #endregion

    /// <summary>
    /// 玩家Transform
    /// </summary>
    private Transform _playerTransform;

    /// <summary>
    /// 获取玩家Transform
    /// </summary>
    private Transform FindPlayerTransform()
    {
        if (_playerTransform == null)
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        return _playerTransform;
    }

    #region 测试
    /// <summary>
    /// 范围指示器（测试用）
    /// </summary>
    private GameObject _rangeIndicator;

    /// <summary>
    /// 创建范围指示器（测试用）
    /// </summary>
    private void CreateRangeIndicator()
    {
        _rangeIndicator = new GameObject("BombardmentRangeIndicator");
        _rangeIndicator.transform.position = _bombardmentCenter;
        _rangeIndicator.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        LineRenderer lineRenderer = _rangeIndicator.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0f, 0f, 0.6f);
        lineRenderer.endColor = new Color(1f, 0f, 0f, 0.6f);
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;

        const int segments = 64;
        lineRenderer.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;
            Vector3 point = _bombardmentCenter + new Vector3(Mathf.Cos(angle) * _zoneRadius, 0.05f, Mathf.Sin(angle) * _zoneRadius);
            lineRenderer.SetPosition(i, point);
        }
    }

    /// <summary>
    /// 销毁范围指示器（测试用）
    /// </summary>
    private void DestroyRangeIndicator()
    {
        if (_rangeIndicator == null)
            return;

        Object.Destroy(_rangeIndicator);
        _rangeIndicator = null;
    }
    #endregion

}