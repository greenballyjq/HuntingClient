using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物移动状态
    /// </summary>
    public class AnimalMoveState : AnimalState, IStayState
    {
        /// <summary>
        /// 移动速度倍率
        /// </summary>
        private const float MoveSpeedRate = 1f;

        public AnimalMoveState(StateMachine stateMachine, AnimalBehavior animal) : base(stateMachine, animal)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // 设置移动速度倍率
            animal.Moveable.SetMoveRate(MoveSpeedRate);
            
            // 播放移动动画
            animal.AnimalAnimator.PlayMove();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Resume()
        {
            base.Resume();

            animal.Moveable.SetMoveRate(MoveSpeedRate);

            animal.AnimalAnimator.PlayMove();
        }
    }
}

