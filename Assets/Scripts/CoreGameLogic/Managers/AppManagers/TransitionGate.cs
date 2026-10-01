using Cysharp.Threading.Tasks;
using GameFramework.Audio;
using GameFramework.Game;
using GameFramework.UI;
using GameFramework.Utility;
using System;

/// <summary>
/// 转场门禁：唯一允许打开/关闭加载遮罩的地方。
/// </summary>
public class TransitionGate : IAppManager
{
    private UIManager _uiManager;
    private AudioManager _audioManager;
    private bool _isCovering;

    public UniTask InitAsync()
    {
        _uiManager = GameServiceLocator.UIManager;
        _audioManager = GameServiceLocator.AudioManager;
        _isCovering = false;
        Log.Info("[TransitionGate] 初始化完成");
        return UniTask.CompletedTask;
    }

    public void Dispose()
    {
        if (_isCovering && _uiManager != null && _uiManager.IsOpen<UITransition>())
            _uiManager.Close<UITransition>();

        _isCovering = false;
        Log.Info("[TransitionGate] 已释放");
    }

    public async UniTask Run(ITransitionJob job)
    {
        await CoverAsync();
        try
        {
            await job.UnloadPrevious();
            await job.LoadNext(Progress.Create<float>(p =>
            {
                UITransition transition = _uiManager.Get<UITransition>();
                transition?.SetProgress(p);
            }));
        }
        finally
        {
            await UncoverAsync();
        }
    }

    private async UniTask CoverAsync()
    {
        if (_isCovering)
            return;

        _audioManager.StopMusic(0f);
        await _uiManager.OpenAsync<UITransition>();
        _isCovering = true;
    }

    private UniTask UncoverAsync()
    {
        if (!_isCovering)
            return UniTask.CompletedTask;

        if (_uiManager.IsOpen<UITransition>())
            _uiManager.Close<UITransition>();

        _isCovering = false;
        return UniTask.CompletedTask;
    }
}
