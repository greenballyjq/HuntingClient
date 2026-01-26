using Cysharp.Threading.Tasks;

/// <summary>
/// 资源预加载接口
/// </summary>
public interface IResourcePreloader
{
    /// <summary>
    /// 预加载资源（异步）
    /// </summary>
    UniTask PreloadAsync();
}

