using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;

namespace CoreGameLogic.Game.Round.Animals.Spawners
{
    public interface IAnimalSpawner
    {
        public UniTask<AnimalBehavior> SpawnAsync();
    }
}