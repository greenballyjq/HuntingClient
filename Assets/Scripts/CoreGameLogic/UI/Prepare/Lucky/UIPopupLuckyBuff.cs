using cfg.HuntingConfig;
using GameFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 幸运仪式增益弹窗
/// </summary>
[UIForm(UILayer.Popup, lifetime: UILifetime.App)]
public class UIPopupLuckyBuff : UIForm<LuckyBuff>
{
    /// <summary>
    /// 描述文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textDescription;

    /// <summary>
    /// 确认按钮
    /// </summary>
    [SerializeField] private Button _buttonConfirm;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager;

    /// <summary>
    /// 当前幸运仪式增益配置
    /// </summary>
    private LuckyBuff _currentLuckyBuffData;

    private void Awake()
    {
        _eventManager = GameServiceLocator.EventManager;
        _buttonConfirm.onClick.AddListener(OnConfirmButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonConfirm.onClick.RemoveListener(OnConfirmButtonClicked);
    }

    protected override void OnOpen(LuckyBuff args)
    {
        _currentLuckyBuffData = args;
        UpdateBuffDisplay(_currentLuckyBuffData);
    }

    #region 私有方法
    /// <summary>
    /// 更新增益展示
    /// </summary>
    /// <param name="buffData">幸运仪式增益配置</param>
    private void UpdateBuffDisplay(LuckyBuff buffData)
    {
        _textDescription.text = buffData.Description;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 触发幸运仪式增益确认按钮点击事件
    /// </summary>
    private void TriggerLuckyBuffConfirmButtonClicked()
    {
        _eventManager.Trigger(LuckyEvents.LuckyBuffConfirmButtonClicked);
    }

    /// <summary>
    /// 确认按钮点击回调
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        TriggerLuckyBuffConfirmButtonClicked();
        Close();
    }
    #endregion
}
