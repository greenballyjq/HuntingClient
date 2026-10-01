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
        progress?.Report(0f);
        await _uiManager.PreloadAsync(UILifetime.Round);

        progress?.Report(0.3f);
        RoundLoadManifest manifest = RoundLoadManifest.ForMainMap(_context);
        await _resourceManager.LoadSceneAsync(manifest.ScenePath);
        DynamicGI.UpdateEnvironment();

        progress?.Report(0.6f);
        manifest.Prewarm();

        progress?.Report(0.8f);
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
