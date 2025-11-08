using cfg;
using cfg.HuntingConfig;
using DG.Tweening;
using Hunting.Game.Animal.State;
using System.Collections.Generic;
using UnityEngine;


namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物基类
    /// </summary>
    public class AnimalBehavior : MonoBehaviour,IPoolable
    {
        #region 测试（TODO: 规范化音效系统后移除）
        public AudioClip hitClip;
        public AudioSource audioSource;

        public void PlayHitAudio()
        {
            audioSource.clip = hitClip;
            audioSource.Play();
        }
        #endregion

        /// <summary>
        /// 状态机
        /// </summary>
        private StateMachine _stateMachine;

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
        public float TimeInScene { get; private  set; }

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
        /// 动画组件
        /// </summary>
        public Animator Animator { get; private set; }

        /// <summary>
        /// RVO避障移动组件
        /// </summary>
        public RVOMovement RVO { get; private set; }

        /// <summary>
        /// 碰撞体组件
        /// </summary>
        public Collider Collider { get; private set; }

        /// <summary>
        /// 所有渲染器组件
        /// </summary>
        private Renderer[] _renderers;

        /// <summary>
        /// 所有材质
        /// </summary>
        private Material[] _materials;

        /// <summary>
        /// 原始颜色
        /// </summary>
        private Color[] _originalColors;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 受击减速倍率
        /// </summary>
        public float HitSpeedMultiplier { get; private set; } = 0.5f;

        /// <summary>
        /// 逃跑加速倍率
        /// </summary>
        public float FleeSpeedMultiplier { get; private set; } = 2f;

        private void Awake()
        {
            // 获取组件
            audioSource = GetComponent<AudioSource>();
            Animator = GetComponent<Animator>();
            RVO = GetComponent<RVOMovement>();
            Collider = GetComponent<Collider>();
            
            // 获取所有渲染器和材质
            _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers != null && _renderers.Length > 0)
            {
                _materials = new Material[_renderers.Length];
                _originalColors = new Color[_renderers.Length];
                
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _materials[i] = _renderers[i].material;
                    _originalColors[i] = _materials[i].color;
                }
            }

            // 创建状态机
            _stateMachine = new StateMachine();
            
            // 预创建状态
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
        /// <param name="initialDirection"></param>
        public void Init(Specie data, float stayTime, Vector3 initialDirection)
        {
            // 初始化数据
            SpecieData = data;
            MaxHP = data.HP;
            CurrentHP = MaxHP;
            CurrentMoveSpeed = data.MoveSpeed;
            CurrentDirection = initialDirection;
            TimeInScene = 0f;
            StayTime = stayTime;
            IsHit = false;
            IsFleeing = false;
            FleeTimeRemaining = 0;

            // 启用碰撞体
            SetColliderEnabled(true);

            // 初始化RVO组件
            RVO.Init(CurrentDirection, CurrentMoveSpeed);

            // 初始化状态机
            _stateMachine.Init(_moveState);

            IsInitialized = true;
        }

        private void Update()
        {
            if(!IsInitialized)
                return;

            // 更新驻场时间
            TimeInScene += Time.deltaTime;

            // 更新状态机
            _stateMachine.Update();


        }

        #region 公共方法
        /// <summary>
        /// 动物受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="hitPoint">击中点位置</param>
        public void TakeDamage(float damage, Vector3 hitPoint)
        {
            // 扣除生命值
            CurrentHP -= damage;

            // 设置受击标志
            IsHit = true;
        }

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
        /// 播放动画
        /// </summary>
        public void PlayAnimation(string animationName)
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
            RVO.SetMaxSpeed(speed);
        }

        /// <summary>
        /// 设置移动方向
        /// </summary>
        public void SetDirection(Vector3 direction)
        {
            CurrentDirection = direction;
            RVO.SetMoveDirection(direction);
        }

        /// <summary>
        /// 设置碰撞体启用状态
        /// </summary>
        public void SetColliderEnabled(bool enabled)
        {
            Collider.enabled = enabled;
        }

        /// <summary>
        /// 暂停移动
        /// </summary>
        public void PauseMovement()
        {
            RVO.PauseMovement();
        }

        /// <summary>
        /// 恢复移动
        /// </summary>
        public void ResumeMovement()
        {
            RVO.ResumeMovement();
        }

        /// <summary>
        /// 释放RVO
        /// </summary>
        public void ReleaseRVO()
        {
            RVO.Release();
        }

        /// <summary>
        /// 应用速度倍率
        /// </summary>
        public void ApplySpeedMultiplier(float multiplier)
        {
            float finalSpeed = CurrentMoveSpeed * multiplier;
            RVO.SetMaxSpeed(finalSpeed);
        }

        /// <summary>
        /// 播放受击变红效果
        /// </summary>
        public void PlayHitEffect()
        {
            if (_materials == null || _materials.Length == 0) 
                return;
            
            // 对所有材质应用变红效果（支持LOD模型）
            for (int i = 0; i < _materials.Length; i++)
            {
                Material mat = _materials[i];
                Color originalColor = _originalColors[i];
                
                // 停止之前的动画，避免重复播放冲突
                mat.DOKill();
                
                // 变红再恢复（0.1秒变红，0.2秒恢复）
                mat.DOColor(Color.red, 0.1f).OnComplete(() =>
                {
                    mat.DOColor(originalColor, 0.2f);
                });
            }
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
        #endregion

        /// <summary>
        /// 获取移动状态
        /// </summary>
        public AnimalMoveState GetMoveState() { return _moveState; }

        /// <summary>
        /// 获取受击状态
        /// </summary>
        public AnimalHitState GetHitState() { return _hitState; }

        /// <summary>
        /// 获取死亡状态
        /// </summary>
        public AnimalDeathState GetDeathState() { return _deathState; }

        /// <summary>
        /// 获取逃跑状态
        /// </summary>
        public AnimalFleeState GetFleeState() { return _fleeState; }

        /// <summary>
        /// 触发动物死亡事件
        /// </summary>
        public void TriggerAnimalDied()
        {
            // 触发掉落奖励事件
            Event.Trigger(AnimalEvents.AnimalDropReward, new AnimalDropRewardEventArgs
            {
                Sender = this,
                Animal = this,
                DropRewards = new Dictionary<EDropType, int>(SpecieData.DropRewards)
            });

            // 触发死亡事件
            Event.Trigger(AnimalEvents.AnimalDied, new AnimalDiedEventArgs
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
            Event.Trigger(AnimalEvents.AnimalFled, new AnimalFledEventArgs
            {
                Sender = this,
                Animal = this,
                SpecieData = SpecieData
            });
        }

        public void OnPull()
        {
            // 从对象池取出时（初始化在Init中）
        }

        public void OnPush()
        {
            // 释放RVO代理
            RVO.Release();
            
            // 禁用碰撞体
            SetColliderEnabled(false);
            
            // 清理所有数据
            SpecieData = null;
            MaxHP = 0;
            CurrentHP = 0;
            CurrentMoveSpeed = 0;
            CurrentDirection = Vector3.zero;
            TimeInScene = 0;
            StayTime = 0;
            IsHit = false;
            IsFleeing = false;
            FleeTimeRemaining = 0;

            IsInitialized = false;
        }
            
        public void OnPoolDestroy()
        {
            // 对象池销毁时
        }
    }
}


