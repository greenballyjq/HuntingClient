using UnityEngine;

namespace Hunting.Game.Animal
{
    public class LinearMovePolicy : IMovePolicy
    {
        public LinearMovePolicy(float baseSpeed, float moveRate)
        {
            BaseSpeed = baseSpeed;
            MoveRate = moveRate;
        }

        public LinearMovePolicy()
        {
            
        }
        
        public float BaseSpeed { get; set; }
        public float MoveRate { get; set; }

        public Vector3 TargetDirection { get; set; }

        public void DoMove(in Transform transform, float dt)
        {
            float actualSpeed = BaseSpeed * MoveRate;
            Vector3 movement = TargetDirection * actualSpeed * dt;
            transform.position += movement;
        }
    }
}