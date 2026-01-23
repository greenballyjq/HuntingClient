using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;
using UnityEngine;

namespace CoreGameLogic.Game.Round.Animals.Spawners
{
    /// <summary>
    /// 守卫动物派发器
    /// </summary>
    public class GuardAnimalSpawner : BaseSpawner
    {
        
        public override async UniTask<BaseAnimalBehaviour> SpawnAsync()
        {
            var configManager = GameServiceLocator.ConfigManager;
            var bossFollowSpecie = configManager.GetRandomBossFollow();
            
            // 计算生成位置与移动方向
            Vector3 spawnPosition = CalculateSpawnPosition();
            Vector3 moveDirection = CalculateMoveDirection();

            return await _animalManager.GenerateGuardAnimalAsync(bossFollowSpecie, spawnPosition, moveDirection);
        }
    }
}