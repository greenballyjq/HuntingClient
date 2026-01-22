using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;


public interface IAnimalSpawner
{
    public UniTask<BaseAnimalBehaviour> SpawnAsync();
}
