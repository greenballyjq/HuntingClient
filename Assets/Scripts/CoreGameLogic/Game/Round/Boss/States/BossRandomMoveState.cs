using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// Boss随机移动状态
/// </summary>
public class BossRandomMoveState : BossState
    {
        /// <summary>
        /// 随机移动点
        /// </summary>
        private Transform[] _randomMovePoints;

        private Vector3 _currentRandomPoint;

        public BossRandomMoveState(BossBehaviour boss, StateMachine stateMachine, string animationName,
            Transform[] randomMovePoints) : base(boss, stateMachine, animationName)
        {
            _randomMovePoints = randomMovePoints;
        }

        public override void Enter()
        {
            base.Enter();
            _currentRandomPoint = SelectRandomPoint();
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            stateTimer += Time.deltaTime;
            if (boss.IsHit)
            {
                stateMachine.ChangeState(boss.GetHitState());
                boss.ResetHitFlag();
                return;
            }
            
            if (MoveToRandomPoint())
            {
                // 切换到idle状态
                stateMachine.ChangeState(boss.GetIdleState());
            }
        }

        private bool MoveToRandomPoint()
        {
            // 移动到随机点
            return boss.MoveTo(_currentRandomPoint);
        }

        /// <summary>
        /// 从随机移动点中选择两个不同的点
        /// </summary>
        private Vector3 SelectRandomPoint()
        {
            if (_randomMovePoints == null || _randomMovePoints.Length < 2)
            {
                Debug.LogWarning("随机移动点数量不足，无法选择两个不同的点");
                return Vector2.zero;
            }

            int index1 = UnityEngine.Random.Range(0, _randomMovePoints.Length);
            int index2 = index1;

            // 确保第二个点与第一个点不同
            while (index2 == index1)
            {
                index2 = UnityEngine.Random.Range(0, _randomMovePoints.Length);
            }

            var randomPoint1 = _randomMovePoints[index1];
            var randomPoint2 = _randomMovePoints[index2];
            
            float normalized = UnityEngine.Random.value;
            
            return randomPoint1.position + normalized * (randomPoint2.position - randomPoint1.position);
        }
}