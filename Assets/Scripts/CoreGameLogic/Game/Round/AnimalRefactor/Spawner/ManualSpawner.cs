using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 手动派发器
    /// </summary>
    public class ManualSpawner : BaseSpawner
    {
        /// <summary>
        /// 派发管理器
        /// </summary>
        private SpawnerManager _spawnerManager => GameServiceLocator.GetRoundManager<SpawnerManager>();
        
        private MovePolicyType _movePolicyType = MovePolicyType.Linear;

        /// <summary>
        /// 派发
        /// </summary>
        public void Spawn()
        {
            _spawnerManager.HandleSpawnRequest(this, CalculateSpawnInfo());
        }
        
        public void SetMovePolicyType(MovePolicyType type) => _movePolicyType = type;

        protected override SpawnInfo CalculateSpawnInfo()
        {
            Vector3 position = CalculatePosition();
            Vector3 direction = CalculateDirection();
            IMovePolicy movePolicy = MovePolicyType.Linear == _movePolicyType
                ? new LinearMovePolicy()
                : new GuardMovePolicy();
            return new SpawnInfo(position, direction, movePolicy);
        }
    }
}
