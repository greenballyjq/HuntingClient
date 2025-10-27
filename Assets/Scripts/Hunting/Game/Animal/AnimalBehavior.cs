using cfg;
using cfg.HuntingConfig;
using Hunting.Game.Animal.State;
using Hunting.Game.Bullet;
using Hunting.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hunting.Game.Animal
{
    // <summary>
    /// 动物基类
    /// </summary>
    public class AnimalBehavior : MonoBehaviour
    {
        public AudioClip hitClip;

        public AudioSource audioSource;

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
        public float moveSpeed;
        /// <summary>
        /// 驻场时间
        /// </summary>
        public float TimeInScene { get; private set; }
        /// <summary>
        /// 最大驻场时间
        /// </summary>
        public float StayTime { get; private set; }
        /// <summary>
        /// 初始方向
        /// </summary>
        public Vector3 initialDirection { get; private set; }

        #endregion

        /// <summary>
        /// 动画组件
        /// </summary>
        public Animator animator { get; private set; }

        /// <summary>
        /// 刚体组件
        /// </summary>
        public Rigidbody rb { get; private set; }

        #region 射线检测配置
        private float detectDistance = 10f;
        private float checkInterval = 0.05f;
        private float checkTimer = 0f;
        #endregion

        #region 射线检测结果
        private RaycastHit frontHit;

        /// <summary>
        /// 是否检测到前方障碍物
        /// </summary>
        public bool HasFrontObstacle { get; private set; }

        /// <summary>
        /// 障碍物距离（未检测到时为最大值）
        /// </summary>
        public float ObstacleDistance { get; private set; }

        /// <summary>
        /// 障碍物碰撞点
        /// </summary>
        public Vector3 ObstacleHitPoint { get; private set; }

        /// <summary>
        /// 射线起点（碰撞体高度中点）
        /// </summary>
        public Vector3 RaycastStartPoint { get; private set; }
        #endregion

        /// <summary>
        /// 检测前方障碍物
        /// </summary>
        public void CheckFrontObstacle()
        {
            // 计时器控制检测频率
            checkTimer += Time.deltaTime;
            if (checkTimer < checkInterval)
            {
                return;
            }
            checkTimer = 0f;

            // 计算射线起点（碰撞体高度中点）
            CalculateRaycastStartPoint();

            HasFrontObstacle = Physics.Raycast(RaycastStartPoint, transform.forward, out frontHit, detectDistance);
            ObstacleDistance = HasFrontObstacle ? frontHit.distance : float.MaxValue;
            ObstacleHitPoint = HasFrontObstacle ? frontHit.point : Vector3.zero;
        }

        /// <summary>
        /// 计算射线起点（碰撞体高度中点）
        /// </summary>
        private void CalculateRaycastStartPoint()
        {
            SphereCollider collider = GetComponent<SphereCollider>();
            if (collider != null)
            {
                // 计算碰撞体在世界空间中的中心点
                RaycastStartPoint = collider.bounds.center;
            }
            else
            {
                // 备用方案：使用物体位置
                RaycastStartPoint = transform.position;
            }
        }

        /// <summary>
        /// 绘制前方射线（编辑器可见）
        /// </summary>
        private void OnDrawGizmos()
        {
            CalculateRaycastStartPoint();

            // 绘制射线起点（碰撞体中心）
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(RaycastStartPoint, 0.1f);

            // 绘制射线
            Gizmos.color = HasFrontObstacle ? Color.red : Color.green;
            Gizmos.DrawRay(RaycastStartPoint, transform.forward * detectDistance);

            // 绘制碰撞点
            if (HasFrontObstacle)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(ObstacleHitPoint, 1f);

                // 绘制碰撞点法线
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(ObstacleHitPoint, frontHit.normal * 3f);
            }
        }

        private void Awake()
        {
            // 获取组件
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();

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
        public void Initialize(cfg.HuntingConfig.Specie data, float stayTime, Vector3 initialDirection)
        {
            // 初始化数据
            specieData = data;
            maxHP = data.HP;
            currentHP = maxHP;
            moveSpeed = data.MoveSpeed;
            TimeInScene = 0f;
            StayTime = 10;
            this.initialDirection = initialDirection;

            stateMachine.Initialize(moveState);
        }

        private void Update()
        {
            stateMachine.Update();

            // 更新驻场时间
            TimeInScene += Time.deltaTime;

            CheckFrontObstacle();
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



            // 切换到受伤状态
            stateMachine.ChangeState(hitState);
        }

        public void PlayHitAudio()
        {
            audioSource.clip = hitClip;
            audioSource.Play();
        }

        /// <summary>
        /// 动物死亡
        /// </summary>
        public void Die()
        {
            stateMachine.ChangeState(deathState);
        }

        /// <summary>
        /// 处理死亡掉落
        /// </summary>
        public void HandleDeathDrop()
        {
            // 触发掉落奖励事件
            OnAnimalDropReward?.Invoke(this, specieData.DropType, specieData.DropAmount);
        }
    }
}
