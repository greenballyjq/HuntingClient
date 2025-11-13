using Cysharp.Threading.Tasks;
using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Hunting.Manager;
using UnityEngine;

namespace Hunting.Game.Skills
{
    /// <summary>
    /// 亚克东技能处理器
    /// </summary>
    public class SkillYaKeDongHandler : ISkillHandler
    {
        /// <summary>
        /// 生成前方距离
        /// </summary>
        private const float SpawnForwardDistance = 5f;

        /// <summary>
        /// 左右偏移距离
        /// </summary>
        private const float SpawnSideOffset = 2f;

        /// <summary>
        /// 配置管理器
        /// </summary>
        private HuntingGameConfigManager Config => GameServiceLocator.Config;

        /// <summary>
        /// 动物管理器
        /// </summary>
        private AnimalManager Animal => GameServiceLocator.GetGameManager<AnimalManager>();

        /// <summary>
        /// 玩家Transform
        /// </summary>
        private Transform _playerTransform;

        public void OnSkillStart(SkillContext context)
        {
            SpawnCoinAnimalsAsync(context).Forget();
        }

        public void OnSkillUpdate(SkillContext context, float deltaTime)
        {
            // 当前技能无需逐帧逻辑
        }

        public void OnSkillEnd(SkillContext context)
        {
            // 当前技能结束时无需额外处理
        }

        /// <summary>
        /// 生成金币怪
        /// </summary>
        private async UniTaskVoid SpawnCoinAnimalsAsync(SkillContext context)
        {
            var parameter = Config.GetSkillYaKeDong(context.SkillData.ParamTableID);
            if (parameter == null)
            {
                Debug.LogWarning("[SkillYaKeDongHandler] 未找到技能参数配置");
                return;
            }

            var specie = Config.GetSpecie(parameter.SpawnAnimalID);
            if (specie == null)
            {
                Debug.LogWarning($"[SkillYaKeDongHandler] 未找到物种配置: {parameter.SpawnAnimalID}");
                return;
            }

            var player = FindPlayerTransform();
            if (player == null)
            {
                Debug.LogWarning("[SkillYaKeDongHandler] 未找到玩家Transform");
                return;
            }

            int spawnCount = GetSpawnCount(parameter);
            float stayTime = Config.GetStayTimeByVolumeType(context.RoundContext.MapId, specie.VolumeType);

            Vector3 forward = player.forward;
            Vector3 right = player.right;
            Vector3 basePosition = player.position + forward * SpawnForwardDistance;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPosition = CalculateSpawnPosition(basePosition, right, i);
                await Animal.SpawnAnimalAsync(specie, spawnPosition, forward, stayTime);
            }
        }

        /// <summary>
        /// 计算单个生成位置
        /// </summary>
        private Vector3 CalculateSpawnPosition(Vector3 basePosition, Vector3 right, int index)
        {
            if (index == 0)
                return basePosition;

            int offsetLayer = (index + 1) / 2;
            int directionSign = (index % 2 == 1) ? -1 : 1;
            Vector3 offset = right * directionSign * offsetLayer * SpawnSideOffset;
            return basePosition + offset;
        }

        /// <summary>
        /// 获取生成数量
        /// </summary>
        private int GetSpawnCount(SkillYaKeDong parameter)
        {
            return Random.Range(parameter.SpawnCount[0], parameter.SpawnCount[1] + 1);
        }

        /// <summary>
        /// 获取玩家Transform
        /// </summary>
        private Transform FindPlayerTransform()
        {
            if (_playerTransform == null)
                _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            return _playerTransform;
        }
    }
}


