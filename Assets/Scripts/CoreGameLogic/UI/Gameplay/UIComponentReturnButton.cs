using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 返回按钮组件（仅负责返回，无结算功能）
/// </summary>
public class UIComponentReturnButton : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 返回按钮
    /// </summary>
    [SerializeField] private Button _buttonReturn;

    /// <summary>
    /// 返回文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textReturn;

    /// <summary>
    /// 返回按钮矩形变换组件
    /// </summary>
    private RectTransform _rectTransformReturnButton;
    public RectTransform RectTransformReturnButton => _rectTransformReturnButton;

    private void Awake()
    {
        _buttonReturn.onClick.AddListener(OnReturnButtonClicked);
        _rectTransformReturnButton = GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        _buttonReturn.onClick.RemoveListener(OnReturnButtonClicked);
    }

    public void Init()
    {
        _textReturn.text = "返回";
    }

    public void CleanUp() { }

    private async void OnReturnButtonClicked()
    {
        await HuntingAppFlow.Instance.EnterPrepareAsync();
    }
}
