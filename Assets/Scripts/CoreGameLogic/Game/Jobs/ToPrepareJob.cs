using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GameFramework.Manager;
using GameFramework.UI;
using System;

public sealed class ToPrepareJob : ITransitionJob
{
    private UIManager _uiManager;
    private ResourceManager _resourceManager;

    public ToPrepareJob()
    {
        BindServices();
    }

    public async UniTask UnloadPrevious()
    {
        RoundFlow roundFlow = HuntingAppFlow.Instance.RoundFlow;
        if (roundFlow == null)
            return;

        await roundFlow.EndRound();
        HuntingAppFlow.Instance.ClearRoundFlow();
    }

    public async UniTask LoadNext(IProgress<float> progress)
    {
        await _uiManager.PreloadAsync(UILifetime.App, new Progress<float>(p => progress?.Report(p * 0.7f)));

        if (!RoundScopeUnload.IsPrepareOrBootstrapActive())
        {
            await _resourceManager.LoadSceneAsync(RoundScopeUnload.PrepareScene, true, new Progress<float>(p =>
            {
                progress?.Report(0.7f + p * 0.15f);
            }));
            UnityEngine.DynamicGI.UpdateEnvironment();
        }
        else
        {
            progress?.Report(0.85f);
        }

        if (!_uiManager.IsOpen<UIPrepare>())
            await _uiManager.OpenAsync<UIPrepare>();

        UIPrepare prepare = _uiManager.Get<UIPrepare>();
        if (prepare != null)
        {
            await prepare.PreloadRoleIconsAsync(new Progress<float>(p =>
            {
                progress?.Report(0.85f + p * 0.15f);
            }));
        }

        progress?.Report(1f);
    }

    private void BindServices()
    {
        _uiManager = GameServiceLocator.UIManager;
        _resourceManager = GameServiceLocator.ResourceManager;
    }
}
