using System.Data.Common;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 亚克东技能处理器
/// </summary>
public class SkillYaKeDongHandler : BaseSkillHandler
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 对象池管理器
    /// </summary>
    private GameObjectPoolManager _gameObjectPoolManager => GameServiceLocator.GameObjectPoolManager;

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        var skillParam = _configManager.GetSkillYaKeDong(context.SkillData.ParamTableID);
        SpawnCoinAnimalsAsync(skillParam).Forget();

        // 模拟动画播放
        await UniTask.Delay(6000);
    }

    protected override void OnSkillUpdate(float dt) {}

    protected override void OnSkillEnd() { }

    #region 私有方法
    /// <summary>
    /// 生成金币怪
    /// </summary>
    private async UniTask SpawnCoinAnimalsAsync(SkillYaKeDong skillParam)
    {
        var specie = _configManager.GetSpecie(skillParam.SpawnAnimalID);
        int spawnCount = Random.Range(skillParam.SpawnCount[0], skillParam.SpawnCount[1] + 1);

        Vector3 forward = _player.forward;
        Vector3 right = _player.right;
        Vector3 basePosition = _player.position + forward * SpawnForwardDistance;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition(basePosition, right, i);
            await SpawnAnimalAsync(specie, spawnPosition, forward);
        }
    }

    /// <summary>
    /// 异步生成动物
    /// </summary>
    /// <param name="specie">物种数据</param>
    /// <param name="position">生成位置</param>
    /// <param name="direction">移动方向</param>
    private async UniTask SpawnAnimalAsync(cfg.HuntingConfig.Specie specie, Vector3 position, Vector3 direction)
    {
        var go = await _gameObjectPoolManager.SpawnAsync(specie.PrefabResourcePath);
        var animal = go.GetComponent<BaseAnimalBehaviour>();
        go.transform.position = position;

        animal.Init(specie);
        animal.Moveable.SetDirection(direction);

        _eventManager.Trigger(AnimalEvents.AnimalGenerated, new AnimalGeneratedEventArgs
        {
            Animal = animal
        });
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
    #endregion

    #region 测试 
    /// <summary>
    /// 生成前方距离
    /// </summary>
    private const float SpawnForwardDistance = 5f;

    /// <summary>
    /// 左右偏移距离
    /// </summary>
    private const float SpawnSideOffset = 8f;
    #endregion
}
