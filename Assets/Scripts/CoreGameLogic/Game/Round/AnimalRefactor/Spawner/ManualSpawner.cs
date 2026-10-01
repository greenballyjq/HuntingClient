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
        private SpawnerManager _spawnerManager;

        /// <summary>
        /// 派发
        /// </summary>
        public BaseAnimalBehaviour Spawn()
        {
            if (_spawnerManager == null)
                _spawnerManager = GameServiceLocator.GetRoundManager<SpawnerManager>();

            return _spawnerManager.HandleSpawnRequest(this, CalculateSpawnInfo());
        }

    }
}
