using cfg;
using cfg.HuntingConfig;
using Hunting.Game.Animal.State;
using Hunting.Game.Bullet;
using Hunting.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物基类
    /// </summary>
    public class AnimalBehavior : MonoBehaviour,IPoolable
    {
        #region 测试
        public AudioClip hitClip;

        public AudioSource audioSource;

        public void PlayHitAudio()
        {
            audioSource.clip = hitClip;
            audioSource.Play();
        }
        #endregion

        #region 事件定义
        /// <summary>
        /// 动物掉落奖励事件
        /// </summary>
        public static event Action<AnimalBehavior, EDropType, int> OnAnimalDropReward;
        #endregion

        #region 状态机与状态
        /// <summary>
        /// 状态机
        /// </summary>
        private StateMachine stateMachine;

        /// <summary>
        /// 行走状态
        /// </summary>
        public AnimalMoveState moveState { get; private set; }

        /// <summary>
        /// 被命中状态
        /// </summary>
        public AnimalHitState hitState { get; private set; }

        /// <summary>
        /// 死亡状态
        /// </summary>
        public AnimalDeathState deathState { get; private set; }

        /// <summary>
        /// 逃跑状态
        /// </summary>
        public AnimalFleeState fleeState { get; private set; }

        #endregion

        #region 配置数据与运行时属性
        /// <summary>
        /// 动物配置数据
        /// </summary>
        public cfg.HuntingConfig.Specie specieData { get; private set; }
        /// <summary>
        /// 最大生命值
        /// </summary>
        public float maxHP { get; private set; }
        /// <summary>
        /// 当前生命值
        /// </summary>
        public float currentHP;
        /// <summary>
        /// 移动速度
        /// </summary>
        public float currentMoveSpeed;
        /// <summary>
        /// 初始方向
        /// </summary>
        public Vector3 currentDirection;
        /// <summary>
        /// 驻场时间
        /// </summary>
        public float timeInScene { get; private set; }
        /// <summary>
        /// 最大驻场时间
        /// </summary>
        public float stayTime { get; private set; }
        

        #endregion

        /// <summary>
        /// 动画组件
        /// </summary>
        public Animator animator { get; private set; }

        /// <summary>
        /// RVO避障移动
        /// </summary>
        public RVOMovement rvo { get; private set; }

        private void Awake()
        {
            #region 测试
            audioSource = GetComponent<AudioSource>();
            #endregion

            // 获取组件
            animator = GetComponent<Animator>();
            rvo = GetComponent<RVOMovement>();

            // 新建状态
            stateMachine = new StateMachine();
            moveState = new AnimalMoveState(this, stateMachine, "Move");
            hitState = new AnimalHitState(this, stateMachine, "Hit");
            deathState = new AnimalDeathState(this, stateMachine, "Death");
            fleeState = new AnimalFleeState(this, stateMachine, "Flee");
            
            
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data">动物配置数据</param>
        /// <param name="stayTime">驻场时间</param>
        /// <param name="moveDirection">生成方向</param>
        public void Init(cfg.HuntingConfig.Specie data, float stayTime, Vector3 initialDirection)
        {
            // 初始化数据
            specieData = data;
            maxHP = data.HP;
            currentHP = maxHP;
            currentMoveSpeed = data.MoveSpeed;
            timeInScene = 0f;
            this.stayTime = stayTime;

            currentDirection = initialDirection;

            rvo.SetMoveDirection(currentDirection);
            rvo.SetMaxSpeed(currentMoveSpeed);
            rvo.Init();
            
           

            stateMachine.Initialize(moveState);
        }

        private void Update()
        {
            // 更新状态机
            stateMachine.Update();

            // 更新驻场时间
            timeInScene += Time.deltaTime;
        }

        /// <summary>
        /// 动物受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="hitPoint">击中点位置</param>
        public void TakeDamage(float damage, Vector3 hitPoint)
        {
            // 扣除生命值
            currentHP -= damage;

            // 检查是否死亡
            if (currentHP <= 0)
            {
                Die();
                PlayHitAudio();
                return;
            }

            // 切换到受伤状态/暂时写在这里
            stateMachine.ChangeState(hitState);
        }

        /// <summary>
        /// 动物死亡：切入死亡状态并做资源清理
        /// </summary>
        public void Die()
        {
            // 切换到死亡状态
            stateMachine.ChangeState(deathState);
            // 可在此停止移动组件等（示例：RVO释放）
            rvo.Release();
        }

        /// <summary>
        /// 通知管理器：该动物已死亡（由死亡状态在动画播放完成后调用）
        /// </summary>
        public void NotifyDeath()
        {
            var mgr = GameServiceLocator.GetGameManager<AnimalManager>();
            if (mgr != null)
            {
                mgr.OnAnimalDiedNotice(this, specieData != null ? specieData.DropType : EDropType.Meat, specieData != null ? specieData.DropAmount : 0);
            }
        }

        /// <summary>
        /// 通知管理器：该动物已逃离场景（由逃跑状态到时调用）
        /// </summary>
        public void NotifyFled()
        {
            var mgr = GameServiceLocator.GetGameManager<AnimalManager>();
            if (mgr != null)
            {
                mgr.OnAnimalFledNotice(this);
            }
        }

        /// <summary>
        /// 处理死亡掉落（已不直接使用，掉落由管理器在接收通知后统一触发）
        /// </summary>
        public void HandleDeathDrop()
        {
            // 触发掉落奖励事件
            OnAnimalDropReward?.Invoke(this, specieData.DropType, specieData.DropAmount);
        }

        public void OnPull()
        {   
            
        }

        public void OnPush()
        {
            rvo.Release();
        }

        public void OnPoolDestroy()
        {

        }
    }
}
