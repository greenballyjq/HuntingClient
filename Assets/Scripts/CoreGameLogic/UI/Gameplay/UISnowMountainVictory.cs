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
    private RectTransform _rectTransform;

    /// <summary>
    /// 特效管理器
    /// </summary>
    private EffectManager _effectManager => GameServiceLocator.EffectManager;

    /// <summary>
    /// 相机管理器
    /// </summary>
    private CameraManager _cameraManager => GameServiceLocator.GetAppManager<CameraManager>();

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    #region 公共方法
    /// <summary>
    /// 播放光效动画
    /// </summary>
    /// <param name="worldPosition">起点世界坐标</param>
    public async UniTask PlayLightEffectAsync(Vector3 worldPosition)
    {
        Vector3 startPosition = PointConverter.WorldPointToUiPoint(_rectTransform, worldPosition, _cameraManager.MainCamera, _cameraManager.UICamera);
        Vector3 endPosition = PointConverter.ScreenPointToUiPoint(_rectTransform, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), _cameraManager.UICamera);

        GameObject effectObj = _effectManager.PlayLoop(_lightEffectPrefab);
        effectObj.transform.SetParent(_rectTransform,true);
        effectObj.transform.position = startPosition;
        Vector3 targetScale = effectObj.transform.localScale * ScaleMultiplier;

        Sequence sequence = DOTween.Sequence()
            .SetLink(effectObj)
            .SetUpdate(_lightEffectConfig.UseUnscaledTime)
            .Join(effectObj.transform.DOMove(endPosition, _lightEffectConfig.Duration))
            .Join(effectObj.transform.DOScale(targetScale, _lightEffectConfig.Duration));

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
