using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework.Core.UI;
using GameFramework.Manager;
using GameFramework.Utility;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 雪山胜利界面
/// </summary>
public class UISnowMountainVictory : UIBase
{
    /// <summary>
    /// 光效配置
    /// </summary>
    [Serializable]
    private class LightEffectConfig
    {
        /// <summary>
        /// 动画时长（秒）
        /// </summary>
        public float Duration = 5f;

        /// <summary>
        /// 是否不受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime = true;

        /// <summary>
        /// 缩放动画曲线
        /// </summary>
        public AnimationCurve ScaleEaseCurve;
    }

    /// <summary>
    /// 全家福配置
    /// </summary>
    [Serializable]
    private class FamilyPortraitConfig
    {
        /// <summary>
        /// 动画时长（秒）
        /// </summary>
        public float Duration = 5f;

        /// <summary>
        /// 是否不受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime = true;
    }

    /// <summary>
    /// 光效预制体
    /// </summary>
    [SerializeField] private GameObject _lightEffectPrefab;

    /// <summary>
    /// 光效配置
    /// </summary>
    [SerializeField] private LightEffectConfig _lightEffectConfig;

    /// <summary>
    /// 全家福图片
    /// </summary>
    [SerializeField] private Image _imageFamilyPortrait;

    /// <summary>
    /// 全家福配置
    /// </summary>
    [SerializeField] private FamilyPortraitConfig _familyPortraitConfig;

    /// <summary>
    /// 放大倍率
    /// </summary>
    private const float ScaleMultiplier = 100f;

    /// <summary>
    /// 雪山胜利矩形变换
    /// </summary>
    private RectTransform _snowMountainVictoryRectTransform;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager;

    /// <summary>
    /// 相机管理器
    /// </summary>
    private CameraManager _cameraManager;

    private void Awake()
    {
        _snowMountainVictoryRectTransform = GetComponent<RectTransform>();
        _effectManager = GameServiceLocator.EffectManager;
        _cameraManager = GameServiceLocator.GetAppManager<CameraManager>();
    }

    #region 公共方法
    /// <summary>
    /// 播放光效动画
    /// </summary>
    /// <param name="worldPosition">起点世界坐标</param>
    public async UniTask PlayLightEffectAsync(Vector3 worldPosition)
    {
        Vector3 startPosition = PointConverter.WorldPointToUiPoint(_snowMountainVictoryRectTransform, worldPosition, _cameraManager.MainCamera, _cameraManager.UICamera);
        Vector3 endPosition = PointConverter.ScreenPointToUiPoint(_snowMountainVictoryRectTransform, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), _cameraManager.UICamera);

        GameObject effectObj = _effectManager.PlayLoop(_lightEffectPrefab);
        effectObj.transform.position = startPosition;
        Vector3 targetScale = effectObj.transform.localScale * ScaleMultiplier;

        var scaleTween = effectObj.transform.DOScale(targetScale, _lightEffectConfig.Duration);
        scaleTween.SetEase(_lightEffectConfig.ScaleEaseCurve);

        Sequence sequence = DOTween.Sequence()
            .SetLink(effectObj)
            .SetUpdate(_lightEffectConfig.UseUnscaledTime)
            .Join(effectObj.transform.DOMove(endPosition, _lightEffectConfig.Duration))
            .Join(scaleTween);

        await sequence.ToUniTask();
    }

    /// <summary>
    /// 播放全家福淡入动画
    /// </summary>
    public async UniTask PlayFamilyPortraitFadeInAsync()
    {
        await _imageFamilyPortrait
            .DOFade(1f, _familyPortraitConfig.Duration)
            .SetUpdate(_familyPortraitConfig.UseUnscaledTime)
            .ToUniTask();
    }
    #endregion
}
