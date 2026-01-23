using UnityEngine;

namespace Hunting.Game.Animal
{
    public class GuardMovePolicy : IMovePolicy
    {
        private Transform _guardTargetTransform;
        
        public GuardMovePolicy(float baseSpeed, float moveRate)
        {
            BaseSpeed = baseSpeed;
            MoveRate = moveRate;

            _guardTargetTransform = Object.FindObjectOfType<BossAnimalBehaviour>().transform;
            if (_guardTargetTransform == null)
            {
                Debug.LogError("[GuardMovePolicy] Could not find BossAnimalBehaviour");
            }
        }

        public GuardMovePolicy()
        {
            _guardTargetTransform = Object.FindObjectOfType<BossAnimalBehaviour>().transform;
            if (_guardTargetTransform == null)
            {
                Debug.LogError("[GuardMovePolicy] Could not find BossAnimalBehaviour");
            }
        }
        
        public float BaseSpeed { get; set; }
        public float MoveRate { get; set; }
        public Vector3 TargetDirection { get; set; }
        public void DoMove(in Transform transform, float dt)
        {
            
        }
    }
}