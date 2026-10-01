using cfg.HuntingConfig;
using GameFramework.Utility;
using Cysharp.Threading.Tasks;

/// <summary>
/// 幸运仪式增益管理器
/// </summary>
public class LuckyBuffManager : IRoundManager
{
    private ILuckyBuffSource _source;

    public UniTask InitAsync(RoundContext context)
    {
        LuckyBuff buffData = context.LuckyBuffData;
        if (buffData == null)
            return UniTask.CompletedTask;

        _source = LuckyBuffSourceFactory.Create(buffData);
        _source?.Activate();
        Log.Info("[LuckyBuffManager] 初始化完成");
        return UniTask.CompletedTask;
    }

    public void Dispose()
    {
        _source?.Deactivate();
        _source = null;
        Log.Info("[LuckyBuffManager] 已释放");
    }
}
