using GameFramework.UI;
using TMPro;
using UnityEngine;
using System;
using System.Text;

/// <summary>
/// 加载提示
/// </summary>
public class UIComponentLoadingHint : MonoBehaviour, IUIComponent
{
    [Serializable]
    private class LoadingConfig
    {
        public string BaseText = "加载中";
        public float DotInterval = 0.5f;
    }

    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private LoadingConfig _loadingConfig;

    private int _dotCount;
    private float _dotTimer;
    private float _progress;
    private readonly StringBuilder _stringBuilder = new StringBuilder(24);

    public void Init()
    {
        gameObject.SetActive(true);
        _dotCount = 0;
        _dotTimer = 0f;
        _progress = 0f;
        RefreshText();
    }

    public void SetProgress(float progress)
    {
        _progress = Mathf.Clamp01(progress);
        RefreshText();
    }

    public void CleanUp()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isActiveAndEnabled || _loadingConfig == null || _loadingText == null)
            return;

        _dotTimer += Time.unscaledDeltaTime;
        if (_dotTimer < _loadingConfig.DotInterval)
            return;

        _dotTimer = 0f;
        _dotCount = (_dotCount + 1) % 4;
        RefreshText();
    }

    private void RefreshText()
    {
        if (_loadingText == null || _loadingConfig == null)
            return;

        _stringBuilder.Clear();
        _stringBuilder.Append(_loadingConfig.BaseText);
        _stringBuilder.Append(' ');
        _stringBuilder.Append(Mathf.RoundToInt(_progress * 100f));
        _stringBuilder.Append('%');
        _stringBuilder.Append('.', _dotCount);
        _loadingText.text = _stringBuilder.ToString();
    }
}
