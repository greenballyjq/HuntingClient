using cfg.HuntingConfig;
using Hunting.Game.Animal;
using UnityEngine;

public class SkillThreeKPCoinAnimalBehaviour : LeaveAnimalBehaviour
{
    /// <summary>
    /// 入场状态
    /// </summary>
    public AnimalEnterState EnterState { get; private set; }

    #region 公共方法
    public override void Init(Specie data)
    {
        EnterState = new SkillThreeKPCoinManAnimalEnterState(_stateMachine, this);

        base.Init(data);

        _stateMachine.Init(EnterState);
    }
    #endregion
}
