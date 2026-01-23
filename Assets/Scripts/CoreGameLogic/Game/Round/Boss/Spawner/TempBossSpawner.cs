using System;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Events;
using UnityEngine;

namespace CoreGameLogic.Game.Round.Boss.Spawner
{
    /// <summary>
    /// 临时Boss派发器
    /// </summary>
    public class TempBossSpawner : MonoBehaviour
    {
        private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;
        private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;
        private EventManager _eventManager => GameServiceLocator.EventManager;

        private void Start()
        {
            _eventManager.AddListener(HiddenMapEvents.HiddenMapPlayStart, OnHiddenMapPlayStart);
        }

        private void OnDestroy()
        {
            _eventManager.AddListener(HiddenMapEvents.HiddenMapPlayStart, OnHiddenMapPlayStart);
        }

        private void OnHiddenMapPlayStart()
        {
            SpawnBoss().Forget();
        }
        
        private async UniTask SpawnBoss()
        {
            var randomBossSpecie = _configManager.GetRandomBoss();
            var spawnPoint = GameObject.Find("BossSpawnerPoint");
            var animalManager = GameServiceLocator.GetRoundManager<AnimalManager>();
            await animalManager.GenerateBossAsync(randomBossSpecie, spawnPoint.transform);
            
        }
    }
}