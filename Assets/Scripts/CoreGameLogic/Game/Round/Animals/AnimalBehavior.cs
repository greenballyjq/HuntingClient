using System;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core.Pool;
using System.Collections.Generic;
using CoreGameLogic.Game.Round.Animals.States;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 动物基类
/// </summary>
public class AnimalBehavior : MonoBehaviour, IPoolItem, IDamageable
{
    /// <summary>
    /// 状态机
    /// </summary>
    private StateMachine _stateMachine;

    private AnimalIdleState _idleState;
    
    /// <summary>
    /// 移动状态
    /// </summary>
    private AnimalMoveState _moveState;

    /// <summary>
    /// 受击状态
    /// </summary>
    private AnimalHitState _hitState;

    /// <summary>
    /// 死亡状态
    /// </summary>
    private AnimalDeathState _deathState;

    /// <summary>
    /// 逃跑状态
    /// </summary>
    private AnimalFleeState _fleeState;

    /// <summary>
    /// 是否已初始化
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// 配置数据
    /// </summary>
    public Specie SpecieData { get; private set; }

    /// <summary>
    /// 最大生命值
    /// </summary>
    public float MaxHP { get; private set; }

    /// <summary>
    /// 当前生命值
    /// </summary>
    public float CurrentHP { get; private set; }

    /// <summary>
    /// 当前移动速度
    /// </summary>
    public float CurrentMoveSpeed { get; private set; }

    /// <summary>
    /// 当前移动方向
    /// </summary>
    public Vector3 CurrentDirection { get; private set; }

    /// <summary>
    /// 在场景中的时间
    /// </summary>
    public float TimeInScene { get; private set; }

    /// <summary>
    /// 最大驻场时间
    /// </summary>
    public float StayTime { get; private set; }

    /// <summary>
    /// 是否被击中标志
    /// </summary>
    public bool IsHit { get; private set; }

    /// <summary>
    /// 是否正在逃跑中
    /// </summary>
    public bool IsFleeing { get; private set; }

    /// <summary>
    /// 逃跑剩余时间
    /// </summary>
    public float FleeTimeRemaining { get; set; }

    /// <summary>
    /// 受击减速倍率
    /// </summary>
    public float HitSpeedMultiplier { get; private set; } = 0f;

    /// <summary>
    /// 逃跑加速倍率
    /// </summary>
    public float FleeSpeedMultiplier { get; private set; } = 2f;

    /// <summary>
    /// 动画组件
    /// </summary>
    public Animator Animator { get; private set; }

    /// <summary>
    /// RVO避障移动组件
    /// </summary>
    public RVOMovement RVO { get; private set; }

    /// <summary>
    /// 守卫移动组件
    /// </summary>
    public GuardMovement Guard { get; private set; }

    /// <summary>
    /// 碰撞体组件
    /// </summary>
    public Collider Collider { get; private set; }

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 粒子特效管理器
    /// </summary>
    private ParticleEffectManager _particleEffectManager =>
        GameServiceLocator.GetFrameworkManager<ParticleEffectManager>();

    #region 对象池接口

    public void OnSpawned()
    {
        gameObject.SetActive(true);
        SetColliderEnabled(true);
        RVO?.Enable();
        //Movement.SetEnabled(true);
    }

    public void OnDespawned()
    {
        RVO?.Disable();
        //Movement.SetEnabled(false);
        SetColliderEnabled(false);

        CurrentHP = 0;
        CurrentMoveSpeed = 0;
        CurrentDirection = Vector3.zero;
        TimeInScene = 0;
        IsHit = false;
        IsFleeing = false;
        FleeTimeRemaining = 0;

        gameObject.SetActive(false);
    }

    #endregion

    private void Awake()
    {
        // 获取组件
        Animator = GetComponentInChildren<Animator>();
        RVO = GetComponent<RVOMovement>();
        Guard = GetComponent<GuardMovement>();
        // Debug.Log($"[{GetType().Name}] RVO：{RVO != null}");
        //Movement = GetComponent<AnimalMovement>();
        Collider = GetComponent<Collider>();

        // 创建状态机
        _stateMachine = new StateMachine();

        // 创建状态
        _idleState = new AnimalIdleState(this, _stateMachine, "Idle");
        _moveState = new AnimalMoveState(this, _stateMachine, "Move");
        _hitState = new AnimalHitState(this, _stateMachine, "Hit");
        _deathState = new AnimalDeathState(this, _stateMachine, "Death");
        _fleeState = new AnimalFleeState(this, _stateMachine, "Flee");
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="data"></param>
    /// <param name="stayTime"></param>
    /// <param name="inHiddenMap"></param>
    public void Init(Specie data, float stayTime, bool inHiddenMap = false)
    {
        SpecieData = data;
        MaxHP = data.HP;
        StayTime = stayTime;
        CurrentHP = MaxHP;
        CurrentMoveSpeed = data.MoveSpeed;
        CurrentDirection = transform.forward;

        EnableRVO();
        DisableGuard();

        if (!inHiddenMap)
        {
            _stateMachine.Init(_moveState);
        }
        else
        {
            var guardState = new AnimalGuardState(this, _stateMachine, "Move");
            _stateMachine.Init(guardState);
        }
    }
    
    private void Update()
    {
        TimeInScene += Time.deltaTime;
        //Movement.UpdateMovement(Time.deltaTime);
        _stateMachine.Update();
    }

    #region 公共方法

    /// <summary>
    /// 动物受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="hitPoint">击中点位置</param>
    /// <param name="hitNormal">命中法线</param>
    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal = default)
    {
        // 扣除生命值
        CurrentHP -= damage;

        // 设置受击标志
        IsHit = true;

        // PlayAnimationTrigger("Hit");
    }

    /// <summary>
    /// 启用RVO移动策略
    /// </summary>
    public void EnableRVO()
    {
        RVO.Enable();
        RVO.SyncPosition();
        RVO.SetMoveDirection(CurrentDirection);
        RVO.SetMaxSpeed(CurrentMoveSpeed);
    }

    /// <summary>
    /// 禁用RVO移动策略
    /// </summary>
    public void DisableRVO()
    {
        RVO.Disable(false);
    }

    /// <summary>
    /// 启用Guard移动策略
    /// </summary>
    public void EnableGuard()
    {
        Guard.SetGuard(true);
    }

    /// <summary>
    /// 禁用Guard移动策略
    /// </summary>
    public void DisableGuard()
    {
        Guard.SetGuard(false);
    }
    
    /// <summary>
    /// 是否有Guard目标
    /// </summary>
    /// <returns></returns>
    public bool HasGuardTarget() => Guard.HasTarget();

    /// <summary>
    /// 重置受击标志
    /// </summary>
    public void ResetHitFlag()
    {
        IsHit = false;
    }

    /// <summary>
    /// 设置逃跑状态标志
    /// </summary>
    public void SetFleeing(bool fleeing)
    {
        IsFleeing = fleeing;
    }

    /// <summary>
    /// 播放动画 bool参数
    /// </summary>
    public void PlayAnimationBool(string animationName)
    {
        Animator.SetBool(animationName, true);
    }

    /// <summary>
    /// 播放动画 trigger参数
    /// </summary>
    public void PlayAnimationTrigger(string animationName)
    {
        Animator.SetBool(animationName, true);
    }

    /// <summary>
    /// 停止动画
    /// </summary>
    public void StopAnimation(string animationName)
    {
        Animator.SetBool(animationName, false);
    }

    /// <summary>
    /// 设置移动速度
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        CurrentMoveSpeed = speed;
        RVO?.SetMaxSpeed(speed);
        //Movement.SetBaseSpeed(speed);
    }

    /// <summary>
    /// 设置移动方向
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        CurrentDirection = direction;
        RVO?.SetMoveDirection(direction);
        //Movement.SetDirection(direction);
    }

    /// <summary>
    /// 设置碰撞体启用状态
    /// </summary>
    public void SetColliderEnabled(bool enabled)
    {
        Collider.enabled = enabled;
    }

    /// <summary>
    /// 应用速度倍率
    /// </summary>
    public void ApplySpeedMultiplier(float multiplier)
    {
        float finalSpeed = CurrentMoveSpeed * multiplier;
        RVO?.SetMaxSpeed(finalSpeed);
        //Movement.SetSpeedMultiplier(multiplier);
    }

    /// <summary>
    /// 获取动画时长
    /// </summary>
    public float GetAnimationLength(string animationName)
    {
        if (Animator == null || Animator.runtimeAnimatorController == null)
            return 0.5f;

        AnimationClip[] clips = Animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == animationName)
                return clip.length;
        }

        return 0.5f;
    }

    /// <summary>
    /// 播放死亡特效
    /// </summary>
    public void PlayDeathEffect()
    {
        _particleEffectManager.SpawnParticleEffectAsync("Animal_Smoke", transform.position).Forget();
    }

    #endregion

    public AnimalIdleState GetIdleState()
    {
        return _idleState;
    }

    /// <summary>
    /// 获取移动状态
    /// </summary>
    public AnimalMoveState GetMoveState()
    {
        return _moveState;
    }

    /// <summary>
    /// 获取受击状态
    /// </summary>
    public AnimalHitState GetHitState()
    {
        return _hitState;
    }

    /// <summary>
    /// 获取死亡状态
    /// </summary>
    public AnimalDeathState GetDeathState()
    {
        return _deathState;
    }

    /// <summary>
    /// 获取逃跑状态
    /// </summary>
    public AnimalFleeState GetFleeState()
    {
        return _fleeState;
    }

    /// <summary>
    /// 触发动物进入死亡事件
    /// </summary>
    public void TriggerAnimalDying()
    {
        _eventManager.Trigger(AnimalEvents.AnimalDying, new AnimalDyingEventArgs
        {
            Animal = this,
            SpecieData = SpecieData,
        });
        Guard.SetGuard(false);
        RVO.Disable();
    }

    /// <summary>
    /// 触发动物死亡事件
    /// </summary>
    public void TriggerAnimalDied()
    {
        // 触发掉落奖励事件
        _eventManager.Trigger(AnimalEvents.AnimalDropReward, new AnimalDropRewardEventArgs
        {
            Sender = this,
            Animal = this,
            DropRewards = new Dictionary<EDropType, int>(SpecieData.DropRewards)
        });

        // 触发死亡事件
        _eventManager.Trigger(AnimalEvents.AnimalDied, new AnimalDiedEventArgs
        {
            Sender = this,
            Animal = this,
            SpecieData = SpecieData,
        });
    }

    /// <summary>
    /// 触发动物逃跑事件
    /// </summary>
    public void TriggerAnimalFled()
    {
        _eventManager.Trigger(AnimalEvents.AnimalFled, new AnimalFledEventArgs
        {
            Sender = this,
            Animal = this,
            SpecieData = SpecieData
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Border"))
        {
            _eventManager.Trigger(AnimalEvents.AnimalReachedWall, new AnimalReachedWallEventArgs { Sender = this, Animal = this });
        }
    }
}