using Cysharp.Threading.Tasks;
using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 返回按钮组件
/// </summary>
public class UIComponentReturnButton : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 返回按钮
    /// </summary>
    [SerializeField] private Button _buttonReturn;
    private PlayerDataManager _playerDataManager;

    private void Awake()
    {
        _playerDataManager = GameServiceLocator.GetAppManager<PlayerDataManager>();
        _buttonReturn.onClick.AddListener(OnReturnButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonReturn.onClick.RemoveListener(OnReturnButtonClicked);
    }

    public void Init(){}

    public void CleanUp() { }

    #region 事件相关
    private async void OnReturnButtonClicked()
    {
        _playerDataManager.Save();
        await HuntingAppFlow.Instance.EnterPrepareAsync();
    }
    #endregion
}
