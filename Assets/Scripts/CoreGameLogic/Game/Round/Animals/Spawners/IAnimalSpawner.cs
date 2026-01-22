using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;


public interface IAnimalSpawner
{
    public UniTask<AnimalBehaviour> SpawnAsync();
}