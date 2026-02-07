using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using System;
using System.Text;

/// <summary>
/// 登录加载提示组件
/// </summary>
public class UIComponentLoginLoading : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 加载动画配置
    /// </summary>
    [Serializable]
    private class LoadingConfig
    {
        /// <summary>
        /// 基础文字
        /// </summary>
        public string BaseText = "加载中";

        /// <summary>
        /// 点数变化间隔（秒）
        /// </summary>
        public float DotInterval = 0.5f;
    }

    /// <summary>
    /// 加载文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _loadingText;

    /// <summary>
    /// 加载配置
    /// </summary>
    [SerializeField] private LoadingConfig _loadingConfig;

    /// <summary>
    /// 当前点数数量
    /// </summary>
    private int _dotCount = 0;

    /// <summary>
    /// 计时器
    /// </summary>
    private float _timer = 0f;

    /// <summary>
    /// 字符串构建器
    /// </summary>
    private StringBuilder _stringBuilder = new StringBuilder();

    public void Init() { }

    public void CleanUp() { }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _loadingConfig.DotInterval)
        {
            _timer = 0f;
            _dotCount = (_dotCount + 1) % 4;
            
            _stringBuilder.Clear();
            _stringBuilder.Append(_loadingConfig.BaseText);
            _stringBuilder.Append('.', _dotCount);
            _loadingText.text = _stringBuilder.ToString();
        }
    }
}
