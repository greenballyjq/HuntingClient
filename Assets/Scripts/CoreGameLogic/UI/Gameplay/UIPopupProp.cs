using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using GameFramework.UI;
using GameFramework.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 道具弹窗
/// </summary>
[UIForm(UILayer.Popup)]
public class UIPopupProp : UIForm<EPropType>
{
    /// <summary>
    /// 道具图片
    /// </summary>
    [SerializeField] private Image _imageProp;

    /// <summary>
    /// 获取按钮
    /// </summary>
    [SerializeField] private Button _buttonGet;

    /// <summary>
    /// 取消按钮
    /// </summary>
    [SerializeField] private Button _buttonCancel;

    /// <summary>
    /// 关闭按钮
    /// </summary>
    [SerializeField] private Button _buttonClose;

    /// <summary>
    /// 道具名称文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textPropName;

    /// <summary>
    /// 获取按钮文本
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textGet;

    /// <summary>
    /// 当前道具配置
    /// </summary>
    private Prop _currentProp;

    /// <summary>
    /// 当前道具价格
    /// </summary>
    private int _propPrice;

    private RoundFlow _roundFlow => HuntingAppFlow.Instance.RoundFlow;
    private HuntingConfigManager _configManager;
    private PlayerDataManager _playerDataManager;

    private void Awake()
    {
        BindServices();
        _buttonGet.onClick.AddListener(OnGetButtonClicked);
        _buttonClose.onClick.AddListener(OnCloseButtonClicked);
        _buttonCancel.onClick.AddListener(OnCancelButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonGet.onClick.RemoveListener(OnGetButtonClicked);
        _buttonClose.onClick.RemoveListener(OnCloseButtonClicked);
        _buttonCancel.onClick.RemoveListener(OnCancelButtonClicked);
    }

    protected override void OnOpen(EPropType args)
    {
        _currentProp = _configManager.GetProp(args);
        RefreshPropDisplay();
    }

    #region 私有方法
    /// <summary>
    /// 注册服务
    /// </summary>
    private void BindServices()
    {
        _configManager = GameServiceLocator.ConfigManager;
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
    }

    /// <summary>
    /// 刷新道具展示
    /// </summary>
    private void RefreshPropDisplay()
    {
        _textPropName.text = _currentProp.Name;
        _imageProp.sprite = _configManager.PropRefSo.GetPropIcon(_currentProp.ID);

        _propPrice = _configManager.GetPropPrice();
        if (_textGet != null)
            _textGet.text = _propPrice.ToString();
        _buttonGet.interactable = _playerDataManager.EnoughThreeKp(_propPrice);
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 获取按钮点击回调
    /// </summary>
    private void OnGetButtonClicked()
    {
        if (!_playerDataManager.EnoughThreeKp(_propPrice))
            return;

        _playerDataManager.UpdateThreeKPCoinAmount(-_propPrice);
        _playerDataManager.UpdatePropCount(_currentProp.PropType, 1);
        _roundFlow.ResumeRound();
        Close();
    }

    /// <summary>
    /// 取消按钮点击回调
    /// </summary>
    private void OnCancelButtonClicked()
    {
        _roundFlow.ResumeRound();
        Close();
    }

    /// <summary>
    /// 关闭按钮点击回调
    /// </summary>
    private void OnCloseButtonClicked()
    {
        _roundFlow.ResumeRound();
        Close();
       
    }             
    #endregion
}
