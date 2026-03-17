using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Core.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 调试组件
/// </summary>
public class UIComponentPrepareDebug : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 快速开始按钮
    /// </summary>
    [SerializeField] private Button _buttonFastStart;

    /// <summary>
    /// 配置管理器
    /// </summary>
    private HuntingConfigManager _configManager => GameServiceLocator.ConfigManager;

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager _uiManager => GameServiceLocator.UIManager;

    public void Init() { }

    public void CleanUp() { }

    private void Awake()
    {
        _buttonFastStart.onClick.AddListener(OnFastStartButtonClicked);
    }

    private void OnDestroy()
    {
        _buttonFastStart.onClick.RemoveListener(OnFastStartButtonClicked);
    }

    #region 事件相关
    /// <summary>
    /// 快速开始按钮回调
    /// </summary>
    private async void OnFastStartButtonClicked()
    {
        Role roleData = _configManager.GetRandomRole();
        Skill skillData = _configManager.GetSkill(roleData.LinkedSkillId);
        Map mapData = _configManager.GetRandomMainMap();

        await SceneManager.LoadSceneAsync("GameplayForestScene").ToUniTask();
        DynamicGI.UpdateEnvironment();

        _uiManager.CloseUI("UIPrepare");

        await HuntingAppFlow.Instance.EnterRound(new RoundContext
        {
            RoleData = roleData,
            MapData = mapData,
            SkillData = skillData,
            LuckyBuffData = null,
            HasLinkage = roleData.LinkedMapId == mapData.ID,
            HasHiddenMap = true,
            HiddenMapData = _configManager.GetMap(EMapType.Hidden)
        }); 
    }
    #endregion
}
