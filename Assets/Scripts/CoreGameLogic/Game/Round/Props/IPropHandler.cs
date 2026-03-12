using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;

/// <summary>
/// 道具处理器接口
/// </summary>
public interface IPropHandler
{
    /// <summary>
    /// 道具开始
    /// </summary>
    /// <param name="propData">道具配置</param>
    UniTask StartProp(Prop propData);

    /// <summary>
    /// 每帧更新
    /// </summary>
    /// <param name="dt">时间增量</param>
    void DoUpdate(float dt);

    /// <summary>
    /// 道具结束
    /// </summary>
    void EndProp();
}

/// <summary>
/// 道具阶段
/// </summary>
public enum PropPhase
{
    /// <summary>
    /// 无阶段
    /// </summary>
    None,

    ///<summary>
    /// 道具开始阶段
    /// </summary>
    Starting,

    /// <summary>
    /// 道具运行阶段
    /// </summary>
    Running,

    /// <summary>
    /// 道具结束阶段
    /// </summary>
    Finished
}