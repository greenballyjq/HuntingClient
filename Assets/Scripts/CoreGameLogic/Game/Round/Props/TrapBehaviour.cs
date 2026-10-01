using cfg.HuntingConfig.Enum;
using GameFramework.Audio;
using Hunting.Game.Animal;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 陷阱行为
/// </summary>
public class TrapBehaviour : MonoBehaviour
{
    /// <summary>
    /// 吸引区域
    /// </summary>
    [SerializeField] private TrapAttractZone _attractZone;

    /// <summary>
    /// 触发区域
    /// </summary>
    [SerializeField] private TrapTriggerZone _triggerZone;

    /// <summary>
    /// 陷阱动画控制器
    /// </summary>
    [SerializeField] private TrapAnimator _trapAnimator;

    /// <summary>
    /// 吸引半径
    /// </summary>
    private float _attractRadius;

    /// <summary>
    /// 触发半径
    /// </summary>
    private float _triggerRadius;

    /// <summary>
    /// 按体型划分的吸引半径范围比例
    /// </summary>
    private Dictionary<ESpecieType, float[]> _attractRadiusRangeByVolume;

    private EventManager _eventManager;
    private HuntingConfigManager _configManager;
    private AudioManager _audioManager;
    private RoundNumericLayer _numeric;

    private const float TrapDamage = 200f;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _configManager = GameServiceLocator.ConfigManager;
        _audioManager = GameServiceLocator.AudioManager;
    }

    private void OnEnable()
    {
        _attractZone.OnAnimalEntered += OnAnimalEnteredAttractZone;
        _triggerZone.OnAnimalEntered += OnAnimalEnteredTriggerZone;
        _trapAnimator.OnCloseAnimationTriggered += OnTrapCloseAnimationTriggered;
    }

    private void OnDisable()
    {
        _attractZone.OnAnimalEntered -= OnAnimalEnteredAttractZone;
        _triggerZone.OnAnimalEntered -= OnAnimalEnteredTriggerZone;
        _trapAnimator.OnCloseAnimationTriggered -= OnTrapCloseAnimationTriggered;
    }

    /// <summary>
    /// 初始化陷阱
    /// </summary>
    /// <param name="attractRadius">吸引半径</param>
    /// <param name="triggerRadius">触发半径</param>
    /// <param name="attractRadiusRangeByVolume">按体型划分的吸引半径范围比例</param>
    public void Init(float attractRadius, float triggerRadius, Dictionary<ESpecieType, float[]> attractRadiusRangeByVolume)
    {
        _attractRadius = attractRadius;
        _triggerRadius = triggerRadius;
        _attractRadiusRangeByVolume = attractRadiusRangeByVolume;
        _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();

        _attractZone.Init(_attractRadius);
        _triggerZone.Init(_triggerRadius);
    }

    private void OnDestroy()
    {
        _attractZone.OnAnimalEntered -= OnAnimalEnteredAttractZone;
        _triggerZone.OnAnimalEntered -= OnAnimalEnteredTriggerZone;
        _trapAnimator.OnCloseAnimationTriggered -= OnTrapCloseAnimationTriggered;
    }

    private void OnAnimalEnteredTriggerZone(BaseAnimalBehaviour animal)
    {
        if (animal.Health.CurrentHealth <= 0) 
            return;

        TriggerTrap(animal);
    }

    private void OnAnimalEnteredAttractZone(BaseAnimalBehaviour animal)
    {
        if (animal.SpecieData.SpecieType == ESpecieType.Boss)
            return;

        Vector3 targetPoint = GetTargetPointForAnimal(animal);
        Vector3 direction = (targetPoint - animal.transform.position).normalized;
        animal.GetComponent<IMoveable>().SetDirection(direction);
    }

    private Vector3 GetTargetPointForAnimal(BaseAnimalBehaviour animal)
    {
        ESpecieType volumeType = animal.SpecieData.SpecieType;
        float[] range = _attractRadiusRangeByVolume[volumeType];
        Vector3 directionToAnimal = animal.transform.position - transform.position;
        directionToAnimal.y = 0f;
        float baseAngle = Mathf.Atan2(directionToAnimal.z, directionToAnimal.x);
        float angle = baseAngle + UnityEngine.Random.Range(-90f, 90f) * Mathf.Deg2Rad;
        float minRadius = range[0] * _attractRadius;
        float maxRadius = range[1] * _attractRadius;
        return GetRandomPointInRing(angle, minRadius, maxRadius);
    }

    private Vector3 GetRandomPointInRing(float angle, float minRadius, float maxRadius)
    {
        float distance = UnityEngine.Random.Range(minRadius, maxRadius);
        return transform.position + new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
    }

    private void TriggerTrap(BaseAnimalBehaviour animal)
    {
        TriggerTrapTriggered(new TrapTriggeredEventArgs { Trap = this });
        var trapRef = _configManager.PropRefSo.Get(_configManager.GetProp(EPropType.Trap).ID);
        _audioManager.Play(trapRef?.Catch, transform.position);
        _numeric.Deal(
            animal.GetComponent<IDamageable>(),
            TrapDamage,
            new DamageContext(DamageSourceKind.Prop));
        _trapAnimator.PlayClose();
    }

    private void OnTrapCloseAnimationTriggered()
    {
        TriggerTrapDestroyed(new TrapDestroyedEventArgs { Trap = this });
    }

    #region 事件相关
    /// <summary>
    /// 触发陷阱触发事件
    /// </summary>
    private void TriggerTrapTriggered(TrapTriggeredEventArgs args)
    {
        _eventManager.Trigger(PropEvents.TrapTriggered, args);
    }

    /// <summary>
    /// 触发陷阱销毁事件
    /// </summary>
    private void TriggerTrapDestroyed(TrapDestroyedEventArgs args)
    {
        _eventManager.Trigger(PropEvents.TrapDestroyed, args);
    }
    #endregion
}
