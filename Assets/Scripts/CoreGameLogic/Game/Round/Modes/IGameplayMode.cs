using Cysharp.Threading.Tasks;

/// <summary>
/// 玩法模式接口
/// </summary>
public interface IGameplayMode
{
    /// <summary>
    /// 进场
    /// </summary>
    UniTask EnterAsync();

    /// <summary>
    /// 退场
    /// </summary>
    /// <param name="nextTarget">下一模式，null 表示整局结束</param>
    UniTask ExitAsync(EGameplayMode? nextTarget);

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    void DoUpdate(float dt);
}
