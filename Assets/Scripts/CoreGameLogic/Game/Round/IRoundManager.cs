using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;

public interface IRoundManager
{
    UniTask InitAsync(RoundContext context);
    void Dispose();
}

public interface IRoundUpdatable
{
    void DoUpdate(float dt);
}

public interface IRoundPausable
{
    void Pause();
    void Resume();
}

/// <summary>
/// 跟地图走：换图时 Unbind，进新图 Bind。
/// </summary>
public interface IMapWorld : IRoundManager
{
    void Unbind();
    void Bind(Map mapData);
}
