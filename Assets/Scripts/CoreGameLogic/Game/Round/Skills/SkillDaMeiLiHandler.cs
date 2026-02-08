using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 大美丽技能处理器
/// </summary>
public class SkillDaMeiLiHandler : BaseSkillHandler
{
    /// <summary>
    /// 技能预制体实例
    /// </summary>
    private GameObject _skillPrefabInstance;

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    /// <summary>
    /// 游戏游玩场景元素管理器
    /// </summary>
    private GameplaySceneItemManager _gameplaySceneItemManager => GameServiceLocator.GetRoundManager<GameplaySceneItemManager>();

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        var skillParam = _configManager.GetSkillDaMeiLi(context.SkillData.ParamTableID);

        // 获取可游玩区域
        var playableArea = _gameplaySceneItemManager.PlayableArea;
        Vector3 areaCenter = playableArea.GetCenter();
        float boundingRadius = playableArea.GetBoundingRadius();

        // 判断是否在可游玩区域内
        Collider[] colliders = Physics.OverlapSphere(areaCenter, boundingRadius, LayerMask.GetMask("Animal"));
        foreach (var collider in colliders)
        {
            var animal = collider.GetComponent<BaseAnimalBehaviour>();
            Vector3 animalPos = animal.transform.position;
            
            if (!playableArea.IsInside(animalPos))
                continue;

            // 伤害并控制
            animal.GetComponent<IDamageable>().TakeDamage(skillParam.DamageAmount);
            animal.GetComponent<IControlable>().TakeControl();
        }

        // 模拟动画播放
        await UniTask.Delay(1000);

        // 创建技能预制体
        await CreateSkillPrefabAsync(skillParam);
    }

    protected override void OnSkillUpdate(float dt) { }

    protected override void OnSkillEnd()
    {
        GameObject.Destroy(_skillPrefabInstance);
        _skillPrefabInstance = null;
    }

    #region 私有方法
    /// <summary>
    /// 创建技能预制体
    /// </summary>
    private async UniTask CreateSkillPrefabAsync(SkillDaMeiLi skillParam)
    {
        var prefab = await _resourceManager.LoadAssetAsync<GameObject>(skillParam.SkillPrefabResourcePath);
        Vector3 spawnPosition = _gameplaySceneItemManager.PlayableArea.GetCenter();
        _skillPrefabInstance = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
    #endregion
}