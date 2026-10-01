using Cysharp.Threading.Tasks;
using GameFramework.Game;
using GameFramework.UI;
using System;

public sealed class ToHiddenMapJob : ITransitionJob
{
    private UIManager _uiManager;

    public ToHiddenMapJob()
    {
        BindServices();
    }

    public UniTask UnloadPrevious()
    {
        _uiManager.Close<UIPopupEnterHiddenMap>();
        return UniTask.CompletedTask;
    }

    public async UniTask LoadNext(IProgress<float> progress)
    {
        progress?.Report(0.2f);
        await HuntingAppFlow.Instance.RoundFlow.SwitchToHiddenMapAsync();
        progress?.Report(1f);
    }

    private void BindServices()
    {
        _uiManager = GameServiceLocator.UIManager;
    }
}
