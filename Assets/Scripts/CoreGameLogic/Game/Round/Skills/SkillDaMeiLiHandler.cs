using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class SkillDaMeiLiHandler : BaseSkillHandler
{
    protected override async UniTask OnSkillStart(SkillContext context)
    {
            await UniTask.CompletedTask;
    }

    protected override void OnSkillUpdate(float dt)
    {

    }

    protected override void OnSkillEnd()
    {
        
    }
}