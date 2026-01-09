using System;
using cfg.HuntingConfig;
using GameLogic.Game.Round.Boss.States;
using UnityEngine;

namespace GameLogic.Game.Round.Boss
{
    public class BossBehaviour : MonoBehaviour, IDamageable
    {
        /// <summary>
        /// Boos移动目标点
        /// </summary>
        [SerializeField] private Transform[] randomMovePoints;

        /// <summary>
        /// 状态机
        /// </summary>
        private StateMachine _stateMachine;

        /// <summary>
        /// Boss 随机移动状态
        /// </summary>
        private BossRandomMoveState _randomMoveState;

        /// <summary>
        /// Boss 死亡状态
        /// </summary>
        private BossDeathState _deathState;
        
        /// <summary>
        /// Boss Idle状态
        /// </summary>
        private BossIdleState _idleState;

        /// <summary>
        /// Boss Idle状态
        /// </summary>
        private BossHitState _hitState;
        
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
        /// 最大驻场时间
        /// </summary>
        public float StayTime { get; private set; }
        
        /// <summary>
        /// 是否被击中标志
        /// </summary>
        public bool IsHit { get; private set; }

        /// <summary>
        /// 动画组件
        /// </summary>
        public Animator Animator { get; private set; }

        /// <summary>
        /// 渲染器组件
        /// </summary>
        private Renderer _renderer;

        /// <summary>
        /// 材质
        /// </summary>
        private Material _material;

        /// <summary>
        /// 原始颜色
        /// </summary>
        private Color _originalColor;

        private void Awake()
        {
            // 获取组件
            Animator = GetComponentInChildren<Animator>();

            // 获取渲染器和材质
            _renderer = GetComponentInChildren<Renderer>();
            _material = _renderer.material;
            _originalColor = _material.color;

            // 创建状态机
            _stateMachine = new StateMachine();
            _idleState = new BossIdleState(this, _stateMachine, "Idle");
            _hitState = new BossHitState(this, _stateMachine, "Hit");
            _randomMoveState = new BossRandomMoveState(this, _stateMachine, "Move", randomMovePoints);
            _deathState = new BossDeathState(this, _stateMachine, "Death");
        }

        private void Start()
        {
            Init(null, 2f);
        }

        /// <summary>
        /// TODO Boss初始化，后续根据数值表填入
        /// </summary>
        public void Init(Specie data, float stayTime)
        {
            // SpecieData = data;
            MaxHP = 1000;
            StayTime = stayTime;
            CurrentHP = MaxHP;
            CurrentMoveSpeed = 4f;
            CurrentDirection = transform.forward;
            
            _stateMachine.Init(_idleState);
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        #region 公共方法

        /// <summary>
        /// Boss受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="hitPoint">击中点位置</param>
        /// <param name="hitNormal">击中点法线</param>
        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            // 扣除生命值
            CurrentHP -= damage;

            IsHit = true;

            // PlayAnimationTrigger("Hit");
        }
        
        /// <summary>
        /// 重置受击标志
        /// </summary>
        public void ResetHitFlag()
        {
            IsHit = false;
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
            Animator.SetTrigger(animationName);
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
        }

        /// <summary>
        /// 设置移动方向
        /// </summary>
        public void SetDirection(Vector3 direction)
        {
            CurrentDirection = direction;
        }

        /// <summary>
        /// 应用速度倍率
        /// </summary>
        public void ApplySpeedMultiplier(float multiplier)
        {
            float finalSpeed = CurrentMoveSpeed * multiplier;
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
        /// 移动到目标点
        /// </summary>
        /// <param name="point">目标点位置</param>
        public bool MoveTo(Vector3 point)
        {
            Vector3 direction = (point - transform.position).normalized;
            direction.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                Time.deltaTime * 5f);

            transform.Translate(direction * (CurrentMoveSpeed * Time.deltaTime), Space.World);

            return Vector3.Distance(transform.position, point) < 0.5f;
        }

        /// <summary>
        /// 获取随机移动状态
        /// </summary>
        public BossRandomMoveState GetRandomMoveState()
        {
            return _randomMoveState;
        }

        /// <summary>
        /// 获取死亡状态
        /// </summary>
        public BossDeathState GetDeathState()
        {
            return _deathState;
        }
        
        /// <summary>
        /// 获取 idle状态
        /// </summary>
        /// <returns></returns>
        public BossIdleState GetIdleState()
        {
            return _idleState;
        }
        
        /// <summary>
        /// 获取 idle状态
        /// </summary>
        /// <returns></returns>
        public BossHitState GetHitState()
        {
            return _hitState;
        }

        #endregion
    }
}