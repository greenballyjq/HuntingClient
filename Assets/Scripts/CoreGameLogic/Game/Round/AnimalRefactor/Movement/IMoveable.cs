using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 可移动接口
    /// </summary>
    public interface IMoveable
    {
        /// <summary>
        /// 当前速度
        /// </summary>
        float CurrentSpeed { get; }
        
        /// <summary>
        /// 当前目标变换组件
        /// </summary>
        Transform CurrentTargetTransform { get; }
        
        /// <summary>
        /// 当前目标方向
        /// </summary>
        Vector3 CurrentTargetDirection { get; }
        
        /// <summary>
        /// 当前移动方向
        /// </summary>
        Vector3 CurrentMoveDirection { get; }
        
        /// <summary>
        /// 是否正在移动
        /// </summary>
        bool IsMoving { get; }
        
        /// <summary>
        /// 初始化移动组件
        /// </summary>
        void Init();

        /// <summary>
        /// 初始化移动组件
        /// </summary>
        /// <param name="movePolicy">移动策略</param>
        void Init(IMovePolicy movePolicy);
        
        /// <summary>
        /// 每帧更新移动
        /// </summary>
        /// <param name="dt">时间增量</param>
        void DoUpdate(float dt);
        
        /// <summary>
        /// 设置移动速度
        /// </summary>
        /// <param name="speed">速度值</param>
        void SetSpeed(float speed);
        
        /// <summary>
        /// 设置移动速率
        /// </summary>
        /// <param name="rate">速率倍数</param>
        void SetMoveRate(float rate);
        
        /// <summary>
        /// 设置移动方向
        /// </summary>
        /// <param name="direction">移动方向</param>
        void SetDirection(Vector3 direction);
        
        /// <summary>
        /// 设置目标
        /// </summary>
        /// <param name="target">目标</param>
        void SetTarget(Vector3 target);
        
        /// <summary>
        /// 开始移动
        /// </summary>
        void StartMove();
        
        /// <summary>
        /// 停止移动
        /// </summary>
        void StopMove();
    }
}

