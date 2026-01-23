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

        /// <summary>
        /// 派发
        /// </summary>
        public void Spawn()
        {
            _spawnerManager.HandleSpawnRequest(this, CalculateSpawnInfo());
        }
    }
}
