using Hunting.Game.Animal;
using UnityEngine;


namespace Hunting.Game.Animal.State
{
    /// <summary>
    /// 动物移动状态
    /// </summary>
    public class AnimalMoveState : AnimalState
    {
        /// <summary>
        /// 避障转向速度曲线
        /// </summary>
        private AnimationCurve avoidTurnCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        /// <summary>
        /// 是否正在避障中
        /// </summary>
        private bool isAvoiding = false;

        /// <summary>
        /// 避障目标方向
        /// </summary>
        private Vector3 avoidTargetDirection;

        /// <summary>
        /// 避障起始旋转
        /// </summary>
        private Quaternion avoidStartRotation;

        /// <summary>
        /// 避障持续时间
        /// </summary>
        private float avoidDuration = 1f;
        private float avoidTimer = 0f;

        /// <summary>
        /// 避障冷却时间
        /// </summary>
        private float avoidCooldown = 1f;
        private float cooldownTimer = 0f;

        /// <summary>
        /// 避障时的移动速度比例
        /// </summary>
        private float avoidSpeedMultiplier = 0.5f;

        /// <summary>
        /// 动物原始移动速度
        /// </summary>
        private float originalMoveSpeed;

        public AnimalMoveState(AnimalBehavior animal, StateMachine stateMachine, string animName) : base(animal, stateMachine, animName) { }

        public override void Enter()
        {
            base.Enter();
            // 保存原始移动速度
            originalMoveSpeed = animal.moveSpeed;
        }

        public override void Update()
        {
            base.Update();

            // 避障冷却计时
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }

            if (isAvoiding)
            {
                // 正在避障中，缓动转向并减速
                ContinueAvoidance();
            }
            else if (animal.HasFrontObstacle && cooldownTimer <= 0)
            {
                // 开始新的避障
                StartAvoidance();
            }
            else
            {
                // 正常移动（确保速度恢复）
                animal.moveSpeed = originalMoveSpeed;
                animal.transform.Translate(animal.transform.forward * animal.moveSpeed * Time.deltaTime, Space.World);
            }
        }

        /// <summary>
        /// 开始避障
        /// </summary>
        private void StartAvoidance()
        {
            isAvoiding = true;
            avoidTimer = 0f;
            avoidStartRotation = animal.transform.rotation;
            avoidTargetDirection = GetAvoidDirection();

            // 避障时减速
            animal.moveSpeed = originalMoveSpeed * avoidSpeedMultiplier;
        }

        /// <summary>
        /// 继续避障转向（缓动效果）
        /// </summary>
        private void ContinueAvoidance()
        {
            avoidTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(avoidTimer / avoidDuration);

            // 使用缓动曲线计算旋转进度
            float curveProgress = avoidTurnCurve.Evaluate(progress);

            // 缓动旋转
            Quaternion targetRotation = Quaternion.LookRotation(avoidTargetDirection);
            animal.transform.rotation = Quaternion.Slerp(avoidStartRotation, targetRotation, curveProgress);

            // 转向同时减速移动
            animal.transform.Translate(animal.transform.forward * animal.moveSpeed * Time.deltaTime, Space.World);

            // 检查是否完成转向
            if (progress >= 1f)
            {
                isAvoiding = false;
                cooldownTimer = avoidCooldown;
                // 恢复原始移动速度
                animal.moveSpeed = originalMoveSpeed;
            }
        }

        /// <summary>
        /// 获取避障方向
        /// </summary>
        private Vector3 GetAvoidDirection()
        {
            // 50%概率左转，50%概率右转
            float turnAngle = Random.Range(0, 2) == 0 ? 60f : -60f;
            Vector3 avoidDir = Quaternion.Euler(0, turnAngle, 0) * animal.transform.forward;

            return avoidDir;
        }
    }
}