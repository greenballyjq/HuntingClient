using cfg.HuntingConfig.Bean;
using cfg.HuntingConfig.Enum;
using UnityEngine;

/// <summary>
/// 更多大肉：死亡掉落草稿上按概率追加肉
/// </summary>
public sealed class LuckyMoreMeatSource : ILuckyBuffSource, IDropProcessor
{
    private readonly LuckyBuffParamMoreMeat _param;
    private RoundNumericLayer _numeric;
    private NumericHandle _handle;

    public LuckyMoreMeatSource(LuckyBuffParamMoreMeat param)
    {
        _param = param;
    }

    public void Activate()
    {
        _numeric = GameServiceLocator.GetRoundManager<RoundNumericLayer>();
        _handle = _numeric.RegisterDrop(this);
    }

    public void Deactivate()
    {
        _numeric?.UnregisterDrop(_handle);
        _numeric = null;
    }

    public void Process(DropDraft draft)
    {
        if (draft?.Rewards == null)
            return;

        if (!draft.Rewards.TryGetValue(EDropType.Meat, out int baseMeat) || baseMeat <= 0)
            return;

        if (Random.value >= _param.ExtraMeatDropProbability)
            return;

        int extra = Mathf.RoundToInt(baseMeat * _param.ExtraMeatDropMultiplier);
        if (extra <= 0)
            return;

        draft.Rewards[EDropType.Meat] = baseMeat + extra;
    }
}
