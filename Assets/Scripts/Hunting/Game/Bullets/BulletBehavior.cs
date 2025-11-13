using cfg.HuntingConfig;
using Hunting.Events;
using Hunting.Game.Animal;
using Hunting.Game.Bullets.Effects;
using UnityEngine;

namespace Hunting.Game.Bullets
{
    /// <summary>
    /// 子弹行为
    /// </summary>
    public class BulletBehavior : MonoBehaviour
    {
        /// <summary>
        /// 子弹数据
        /// </summary>
        private Bullet _bulletData;

        /// <summary>
        /// 子弹效果上下文
        /// </summary>
        private BulletEffectContext _bulletEffectContext;

        /// <summary>
        /// 基础命中效果
        /// </summary>
        private readonly IBulletEffect _baseEffect = new BaseBulletEffect();

        /// <summary>
        /// 额外命中效果
        /// </summary>
        private IBulletEffect _extraEffect;

        /// <summary>
        /// 移动方向
        /// </summary>
        private Vector3 _moveDirection;

        /// <summary>
        /// 子弹最大飞行时长 TODO: 之后配置化
        /// </summary>
        private const float MaxLifetime = 10f;

        /// <summary>
        /// 生命周期计时 TODO: 之后配置化
        /// </summary>
        private float _lifeTimer;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager Event => GameServiceLocator.Event;

        /// <summary>
        /// 子弹初始化
        /// </summary>
        /// <param name="bulletData">子弹数据</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="direction">子弹方向</param>
        /// <param name="owner">发射者</param>
        public void Init(Bullet bulletData, Vector3 startPosition, Vector3 direction, GameObject owner)
        {
             // 初始化数据
            _bulletData = bulletData;
            _moveDirection = direction.normalized;
            _lifeTimer = MaxLifetime;
            _bulletEffectContext = new BulletEffectContext
            {
                BulletData = bulletData,
                BaseDamage = bulletData.BaseDamage,
                Owner = owner
            };

            // 根据配置挂载额外效果
            _extraEffect = BulletEffectFactory.CreateExtraEffect(bulletData.BulletType, bulletData);

            // 设置初始化标识
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            UpdateMovement();
            UpdateLifetime();
            CheckCollision();
        }

        #region 私有方法
        /// <summary>
        /// 更新移动
        /// </summary>
        private void UpdateMovement()
        {
            transform.position += _moveDirection * _bulletData.MoveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 更新生命周期
        /// </summary>
        private void UpdateLifetime()
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0f)
                DestroyBullet();
        }

        /// <summary>
        /// 检查碰撞
        /// </summary>
        private void CheckCollision()
        {
            // 使用射线检测当前帧移动路径
            float moveDistance = _bulletData.MoveSpeed * Time.deltaTime;
            if (Physics.Raycast(transform.position, _moveDirection, out RaycastHit hit, moveDistance, LayerMask.GetMask("Animal")))
            {
                if (hit.collider.TryGetComponent(out AnimalBehavior animal))
                    HandleHit(animal, hit.point);
            }
        }

        /// <summary>
        /// 处理命中
        /// </summary>
        /// <param name="animal">命中的动物</param>
        /// <param name="hitPoint">命中位置</param>
        private void HandleHit(AnimalBehavior animal, Vector3 hitPoint)
        {
            var hitInfo = new BulletHitInfo
            {
                HitPoint = hitPoint,
                PrimaryTarget = animal
            };

            // 先处理基础命中逻辑，再附加额外效果
            _baseEffect.OnHit(_bulletEffectContext, hitInfo);
            _extraEffect?.OnHit(_bulletEffectContext, hitInfo);

            DestroyBullet();
        }

        /// <summary>
        /// 销毁子弹
        /// </summary>
        private void DestroyBullet()
        {
            Event.Trigger(BulletEvents.BulletDestroyed, new BulletDestroyedEventArgs
            {
                BulletData = _bulletData,
                Bullet = this
            });
        }
        #endregion

       

    }
}
