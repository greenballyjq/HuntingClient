using cfg.HuntingConfig;
using GameFramework.Core.UI;
using Hunting.Events;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 幸运增益展示界面
/// </summary>
public class UILuckyBuffShow : UIBase
{
    /// <summary>
    /// 标题文本
    /// </summary>
    [SerializeField] private Text _textTitle;

    /// <summary>
    /// 增益描述文本
    /// </summary>
    [SerializeField] private Text _textBuffDescription;

    /// <summary>
    /// 确认按钮
    /// </summary>
    [SerializeField] private Button _buttonConfirm;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 当前增益配置
    /// </summary>
    private LuckyBuff _currentLuckyBuff;

    private void Awake()
    {
        _buttonConfirm.onClick.AddListener(OnClickConfirm);
    }

    private void OnDestroy()
    {
        _buttonConfirm.onClick.RemoveListener(OnClickConfirm);
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        _currentLuckyBuff = userData as LuckyBuff;
        if (_currentLuckyBuff != null)
            UpdateBuffDisplay(_currentLuckyBuff);
    }

    #region 私有方法
    /// <summary>
    /// 更新展示内容
    /// </summary>
    /// <param name="buffData">幸运仪式增益配置</param>
    private void UpdateBuffDisplay(LuckyBuff buffData)
    {
        _textTitle.text = buffData.Name;
        _textBuffDescription.text = buffData.Description;
    }

    /// <summary>
    /// 触发增益确认点击事件
    /// </summary>
    private void TriggerLuckyBuffConfirmClicked()
    {
        _eventManager.Trigger(LuckyEvents.LuckyBuffConfirmClicked);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 确认按钮点击回调
    /// </summary>
    private void OnClickConfirm()
    {
        TriggerLuckyBuffConfirmClicked();
        Close();
    }
    #endregion
}
