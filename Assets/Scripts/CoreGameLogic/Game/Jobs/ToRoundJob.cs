using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GameFramework.Manager;
using GameFramework.UI;
using System;
using UnityEngine;

public sealed class ToRoundJob : ITransitionJob
{
    private readonly RoundContext _context;
    private UIManager _uiManager;
    private ResourceManager _resourceManager;

    public ToRoundJob(RoundContext context)
    {
        _context = context;
        BindServices();
    }

    public UniTask UnloadPrevious()
    {
        _uiManager.Close<UIPrepare>();
        return UniTask.CompletedTask;
    }

    public async UniTask LoadNext(IProgress<float> progress)
    {
        await _uiManager.PreloadAsync(UILifetime.Round, new Progress<float>(p => progress?.Report(p * 0.55f)));

        RoundLoadManifest manifest = RoundLoadManifest.ForMainMap(_context);
        await _resourceManager.LoadSceneAsync(manifest.ScenePath, true, new Progress<float>(p =>
        {
            progress?.Report(0.55f + p * 0.25f);
        }));
        DynamicGI.UpdateEnvironment();

        progress?.Report(0.8f);
        manifest.Prewarm();

        var roundFlow = HuntingAppFlow.Instance.CreateRoundFlow();
        await roundFlow.EnterRound(_context);

        progress?.Report(1f);
    }

    private void BindServices()
    {
        _uiManager = GameServiceLocator.UIManager;
        _resourceManager = GameServiceLocator.ResourceManager;
    }
}
