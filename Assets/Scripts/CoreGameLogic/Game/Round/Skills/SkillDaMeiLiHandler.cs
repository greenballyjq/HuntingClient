using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 大美丽技能处理器
/// </summary>
public class SkillDaMeiLiHandler : BaseSkillHandler
{
    /// <summary>
    /// 技能参数
    /// </summary>
    private SkillDaMeiLi _skillParam;

    /// <summary>
    /// 爱心预制体
    /// </summary>
    private GameObject _lovePrefab;

    protected override void OnInit() 
    {
        _skillParam = ConfigManager.GetSkillDaMeiLi(SkillContext.SkillData.ParamTableID);
        _lovePrefab = ConfigManager.SkillRefSo.GetSkillPrefab(SkillContext.SkillData.ID);
    }

    protected override UniTask OnSkillStart()
    {
        Collider[] colliders = Physics.OverlapSphere(
            PlayableArea.GetCenter(), 
            PlayableArea.GetBoundingRadius(), 
            LayerMask.GetMask("Animal")
        );

        foreach (var collider in colliders)
        {
            var animal = collider.GetComponent<BaseAnimalBehaviour>();

            if (!PlayableArea.IsInside(animal.transform.position))
                continue;

            animal.GetComponent<IDamageable>().TakeDamage(_skillParam.DamageAmount);
            animal.GetComponent<IControlable>().TakeControl();
        }

        CreateLoveZone();

        return UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt) { }

    protected override void OnSkillEnd() {}

    #region 私有方法
    /// <summary>
    /// 创建爱心区域
    /// </summary>
    private void CreateLoveZone()
    {
        _lovePrefab = Object.Instantiate(_lovePrefab, PlayableArea.GetCenter(),Quaternion.identity);
    }
    #endregion
}