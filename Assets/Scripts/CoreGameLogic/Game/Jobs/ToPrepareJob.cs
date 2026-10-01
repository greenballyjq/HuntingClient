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
        progress?.Report(0f);
        await _uiManager.PreloadAsync(UILifetime.App);

        progress?.Report(0.3f);

        if (!RoundScopeUnload.IsPrepareOrBootstrapActive())
        {
            await _resourceManager.LoadSceneAsync(RoundScopeUnload.PrepareScene);
            UnityEngine.DynamicGI.UpdateEnvironment();
        }

        progress?.Report(0.6f);

        if (!_uiManager.IsOpen<UIPrepare>())
            await _uiManager.OpenAsync<UIPrepare>();

        UIPrepare prepare = _uiManager.Get<UIPrepare>();
        if (prepare != null)
            await prepare.PreloadRoleIconsAsync();

        progress?.Report(1f);
    }

    private void BindServices()
    {
        _uiManager = GameServiceLocator.UIManager;
        _resourceManager = GameServiceLocator.ResourceManager;
    }
}
