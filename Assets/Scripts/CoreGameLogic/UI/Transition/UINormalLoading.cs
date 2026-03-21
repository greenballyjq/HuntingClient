using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 加载界面
/// </summary>
public class UINormalLoading : UIBase
{
    [SerializeField] private GameObject _effectPrefab;
    private bool _fadeInDone;
    private bool _stopRequested;
    private EffectManager _effectManager;

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _effectManager = GameServiceLocator.EffectManager;
    }

    /// <summary>
    /// 播放淡入
    /// </summary>
    public async UniTask PlayFadeInAsync()
    {
        _fadeInDone = false;
        _stopRequested = false;
        _effectManager.PlayOneShot(_effectPrefab, dontDestroyOnLoad: true);
        await UniTask.Delay(1000);
        _fadeInDone = true;
        LoopPlaying().Forget();
    }

    /// <summary>
    /// 播放淡出
    /// </summary>
    public async UniTask PlayFadeOutAsync()
    {
        while (!_fadeInDone) 
            await UniTask.Yield();

        _stopRequested = true;
        Close();
    }

    /// <summary>
    /// 循环播放
    /// </summary>
    private async UniTask LoopPlaying()
    {
        while (!_stopRequested)
        {
            _effectManager.PlayOneShot(_effectPrefab, dontDestroyOnLoad: true);
            await UniTask.Delay(1000);
        }
    }
}
