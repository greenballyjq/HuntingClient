using cfg;
using cfg.HuntingConfig;
using GameFramework.Core;
using Hunting.Game.Animal;
using System;
using UnityEngine;

namespace Hunting.Game.Bullet
{
    public class BulletBehavior : MonoBehaviour
    {
        /// <summary>
        /// 子弹配置数据
        /// </summary>
        private cfg.HuntingConfig.Bullet bulletData;
        /// <summary>
        /// 子弹当前伤害
        /// </summary>
        private float currentDamage;
        /// <summary>
        /// 子弹当前移动速度
        /// </summary>
        private float currentMoveSpeed;
        /// <summary>
        /// 子弹移动方向
        /// </summary>
        private Vector3 moveDirection;

        /// <summary>
        /// 子弹初始化
        /// </summary>
        /// <param name="config">子弹配置数据</param>
        /// <param name="startPosition">子弹起始位置</param>
        /// <param name="direction">子弹方向</param>
        public void Initialize(cfg.HuntingConfig.Bullet config, Vector3 startPosition, Vector3 direction)
        {
            // 初始化子弹属性
            this.bulletData = config;
            this.currentDamage = config.BaseDamage;
            this.currentMoveSpeed = config.MoveSpeed;
            this.moveDirection = direction.normalized;

            // 设置子弹位置和方向
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(direction);

            // 10秒后自动销毁
            Invoke(nameof(DestroyBullet), 10f);
        }

        /// <summary>
        /// 子弹移动
        /// </summary>
        private void UpdateMovement()
        {
            transform.position += moveDirection * currentMoveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        private void CheckCollision()
        {
            // 计算子弹移动的距离
            float moveDistance = currentMoveSpeed * Time.deltaTime;
            RaycastHit hit;

            // 射线检测，检测子弹是否击中动物
            if (Physics.Raycast(transform.position, moveDirection, out hit, moveDistance))
            {
                if (hit.collider.TryGetComponent<AnimalBehavior>(out var animal))
                {
                    if (animal != null)
                    {
                        animal.TakeDamage(currentDamage, hit.point); // 对动物造成伤害
                    }

                    DestroyBullet(); // 击中动物立即销毁
                }
            }
        }

        /// <summary>
        /// 销毁子弹
        /// </summary>
        private void DestroyBullet()
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            UpdateMovement();
            CheckCollision();
        }

    }
}
