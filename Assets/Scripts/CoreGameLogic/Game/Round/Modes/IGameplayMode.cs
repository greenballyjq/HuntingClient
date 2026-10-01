using Cysharp.Threading.Tasks;

public interface IGameplayMode
{
    UniTask EnterAsync();
    UniTask PresentAsync();
    UniTask ExitAsync();
    void DoUpdate(float dt);
}
