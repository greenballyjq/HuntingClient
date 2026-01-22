using cfg.HuntingConfig.Enum;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 动物事件触发器组件基类
    /// </summary>
    public class BaseAnimalEventTrigger : MonoBehaviour
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        protected EventManager _eventManager => GameServiceLocator.EventManager;

        /// <summary>
        /// 动物行为类
        /// </summary>
        protected BaseAnimalBehaviour _animalBehaviour;

        /// <summary>
        /// 初始化动画事件触发器组件
        /// </summary>
        /// <param name="animalBehaviour">动物行为类</param>
        public void Init(BaseAnimalBehaviour animalBehaviour)
        {
            _animalBehaviour = animalBehaviour;
        }

        /// <summary>
        /// 触发动物进入死亡事件
        /// </summary>
        public void TriggerAnimalEnteredDeath()
        {
            _eventManager.Trigger(AnimalEvents.AnimalEnteredDeath, new AnimalEnteredDeathEventArgs
            {
                Sender = this,
                Animal = _animalBehaviour,
                SpecieData = _animalBehaviour.SpecieData,
            });
        }

        /// <summary>
        /// 触发动物死亡事件
        /// </summary>
        public void TriggerAnimalDied()
        {
            _eventManager.Trigger(AnimalEvents.AnimalDied, new AnimalDiedEventArgs
            {
                Sender = this,
                Animal = _animalBehaviour,
                SpecieData = _animalBehaviour.SpecieData,
            });
        }

        /// <summary>
        /// 触发动物掉落奖励事件
        /// </summary>
        public void TriggerAnimalDropReward()
        {
            _eventManager.Trigger(AnimalEvents.AnimalDropReward, new AnimalDropRewardEventArgs
            {
                Sender = this,
                Animal = _animalBehaviour,
                DropRewards = new Dictionary<EDropType, int>(_animalBehaviour.SpecieData.DropRewards)
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Border"))
            {
                _eventManager.Trigger(AnimalEvents.AnimalReachedWall,new AnimalReachedWallEventArgs 
                { 
                    Sender = this, 
                    Animal = _animalBehaviour 
                });
            }
        }
    }

}
