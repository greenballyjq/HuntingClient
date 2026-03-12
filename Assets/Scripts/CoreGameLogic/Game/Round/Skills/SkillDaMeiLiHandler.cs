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
    /// 技能参数缓存
    /// </summary>
    private SkillDaMeiLi _skillParamCache;

    /// <summary>
    /// 技能预制体缓存
    /// </summary>
    private GameObject _skillPrefabCache;

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        if(_skillParamCache == null)
            _skillParamCache = _configManager.GetSkillDaMeiLi(context.SkillData.ParamTableID);

        if (_skillPrefabCache == null)
            _skillPrefabCache = _configManager.SkillRefSo.GetSkillPrefab(context.SkillData.ID);

        Collider[] colliders = Physics.OverlapSphere(
            _playableArea.GetCenter(), 
            _playableArea.GetBoundingRadius(), 
            LayerMask.GetMask("Animal")
        );

        foreach (var collider in colliders)
        {
            var animal = collider.GetComponent<BaseAnimalBehaviour>();

            if (!_playableArea.IsInside(animal.transform.position))
                continue;

            animal.GetComponent<IDamageable>().TakeDamage(_skillParamCache.DamageAmount);
            animal.GetComponent<IControlable>().TakeControl();
        }

        // 模拟动画播放
        await UniTask.Delay(1000);

        CreateLove();
    }

    protected override void OnSkillUpdate(float dt) { }

    protected override void OnSkillEnd()
    {
        Object.Destroy(_skillPrefabCache);
    }

    #region 私有方法
    /// <summary>
    /// 创建爱心
    /// </summary>
    private void CreateLove()
    {
        _skillPrefabCache = Object.Instantiate(_skillPrefabCache, _playableArea.GetCenter(),Quaternion.identity);
    }
    #endregion
}