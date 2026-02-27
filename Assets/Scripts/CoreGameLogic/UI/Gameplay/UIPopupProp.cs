using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Prop;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 道具弹窗
/// </summary>
public class UIPopupProp : UIBase
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
    /// 当前道具配置
    /// </summary>
    private Prop _currentProp;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// 玩家数据管理器
    /// </summary>
    private PlayerDataManager _playerDataManager => GameServiceLocator.GetAppManager<PlayerDataManager>();

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    private void Awake()
    {
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

    public override void OnInit(object userData)
    {
        base.OnInit(userData);
        _currentProp = _configManager.GetProp((EPropType)userData);
        UpdatePropDisplayAsync().Forget();
    }

    #region 私有方法
    /// <summary>
    /// 更新道具展示
    /// </summary>
    private async UniTask UpdatePropDisplayAsync()
    {
        _textPropName.text = _currentProp.Name;
        var sprite = await _resourceManager.LoadAssetAsync<Sprite>(_currentProp.IconResourcePath);
        _imageProp.sprite = sprite;
    }
    #endregion

    #region 事件相关
    /// <summary>
    /// 获取按钮点击回调
    /// </summary>
    private void OnGetButtonClicked()
    {
        _playerDataManager.UpdatePropCount(_currentProp.PropType, 1);
        RoundFlow.Instance.ResumeRound();
        Close();
    }

    /// <summary>
    /// 取消按钮点击回调
    /// </summary>
    private void OnCancelButtonClicked()
    {
        RoundFlow.Instance.ResumeRound();
        Close();
    }

    /// <summary>
    /// 关闭按钮点击回调
    /// </summary>
    private void OnCloseButtonClicked()
    {
        RoundFlow.Instance.ResumeRound();
        Close();
       
    }             
    #endregion
}
