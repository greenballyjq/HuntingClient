using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;


public interface IAnimalSpawner
{
    public UniTask<AnimalBehavior> SpawnAsync();
}