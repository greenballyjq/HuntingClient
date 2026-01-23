using UnityEngine;

namespace Hunting.Game.Animal
{
    public interface IMovePolicy
    {
        public float BaseSpeed { get; set; }
        public float MoveRate { get; set; }
        public Vector3 TargetDirection { get; set; }
        void DoMove(in Transform transform, float dt);
    }

    public enum MovePolicyType
    {
        Linear,
        Guard,
        Boss
    }
}