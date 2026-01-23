using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 自动派发器
    /// </summary>
    public class AutoSpawner : BaseSpawner
    {
        /// <summary>
        /// 派发管理器
        /// </summary>
        private SpawnerManager _spawnerManager => GameServiceLocator.GetRoundManager<SpawnerManager>();

        /// <summary>
        /// 单次派发冷却时间
        /// </summary>
        [SerializeField] private float cooldownTime;

        /// <summary>
        /// 单次派发动物数量
        /// </summary>
        [SerializeField] private int spawnCount;

        /// <summary>
        /// 单只动物派发间隔
        /// </summary>
        [SerializeField] private float spawnInterval;

        /// <summary>
        /// 是否启用
        /// </summary>
        [SerializeField] private bool isEnabled = true;

        /// <summary>
        /// 冷却计时器
        /// </summary>
        private float _cooldownTimer;

        /// <summary>
        /// 剩余派发数量
        /// </summary>
        private int _remainingSpawnCount;

        /// <summary>
        /// 派发间隔计时器
        /// </summary>
        private float _spawnIntervalTimer;

        /// <summary>
        /// 设置启用状态
        /// </summary>
        /// <param name="enabled">是否启用</param>
        public void SetEnabled(bool enabled)
        {
            isEnabled = enabled;
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            if (!isEnabled) return;

            if (_remainingSpawnCount > 0)
            {
                _spawnIntervalTimer += dt;
                if (_spawnIntervalTimer >= spawnInterval)
                {
                    SpawnInfo info = CalculateSpawnInfo();
                    _spawnerManager.HandleSpawnRequest(this, info);

                    _remainingSpawnCount--;
                    _spawnIntervalTimer = 0f;
                }
            }
            else
            {
                _cooldownTimer += dt;
                if (_cooldownTimer >= cooldownTime)
                {
                    _remainingSpawnCount = spawnCount;
                    _cooldownTimer = 0f;
                }
            }
        }
    }
}
