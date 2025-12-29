using cfg.HuntingConfig.Prop;

/// <summary>
/// 道具上下文
/// </summary>
public class PropContext
{
    /// <summary>
    /// 道具配置
    /// </summary>
    public Prop PropData { get; set; }
}

/// <summary>
/// 道具处理器接口
/// </summary>
public interface IPropHandler
{
    /// <summary>
    /// 道具效果开始
    /// </summary>
    /// <param name="context">道具上下文</param>
    void OnPropStart(PropContext context);

    /// <summary>
    /// 道具效果更新
    /// </summary>
    /// <param name="context">道具上下文</param>
    /// <param name="deltaTime">时间增量</param>
    void OnPropUpdate(PropContext context, float deltaTime);

    /// <summary>
    /// 道具效果结束
    /// </summary>
    /// <param name="context">道具上下文</param>
    void OnPropEnd(PropContext context);
}