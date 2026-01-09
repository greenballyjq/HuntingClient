using GameFramework.Game.UI;
using GameFramework.Manager;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Debug 控制组件
/// </summary>
public class UIComponentDebug : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// 增加派发间隔按钮
    /// </summary>
    [SerializeField] private Button _buttonIncreaseInterval;

    /// <summary>
    /// 减少派发间隔按钮
    /// </summary>
    [SerializeField] private Button _buttonDecreaseInterval;

    /// <summary>
    /// 派发间隔文本
    /// </summary>
    [SerializeField] private Text _textSpawnInterval;

    /// <summary>
    /// 动物数量文本
    /// </summary>
    [SerializeField] private Text _textAnimalCount;

    /// <summary>
    /// 帧率文本
    /// </summary>
    [SerializeField] private Text _textFPS;

    /// <summary>
    /// 派发器管理器
    /// </summary>
    private SpawnerManager _spawnerManager => GameServiceLocator.GetRoundManager<SpawnerManager>();

    /// <summary>
    /// 动物管理器
    /// </summary>
    private AnimalManager _animalManager => GameServiceLocator.GetRoundManager<AnimalManager>();

    /// <summary>
    /// 间隔修改单位（秒）
    /// </summary>
    private const float IntervalStep = 0.1f;

    private void Awake()
    {
        _buttonIncreaseInterval.onClick.AddListener(OnIncreaseIntervalClicked);
        _buttonDecreaseInterval.onClick.AddListener(OnDecreaseIntervalClicked);
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        _buttonIncreaseInterval.onClick.RemoveListener(OnIncreaseIntervalClicked);
        _buttonDecreaseInterval.onClick.RemoveListener(OnDecreaseIntervalClicked);
    }

    public void Init()
    {
    }

    public void CleanUp()
    {
    }

    #region 私有方法
    /// <summary>
    /// 更新显示
    /// </summary>
    private void UpdateDisplay()
    {
        UpdateSpawnIntervalDisplay();
        UpdateAnimalCountDisplay();
        UpdateFPSDisplay();
    }

    /// <summary>
    /// 更新派发间隔显示
    /// </summary>
    private void UpdateSpawnIntervalDisplay()
    {
        Spawner firstSpawner = _spawnerManager.GetSpawner(0);
        if (firstSpawner != null)
        {
            float interval = firstSpawner.GetSpawnInterval();
            _textSpawnInterval.text = $"派发间隔: {interval:F1}s";
        }
    }

    /// <summary>
    /// 更新动物数量显示
    /// </summary>
    private void UpdateAnimalCountDisplay()
    {
        int count = _animalManager.GetActiveAnimalCount();
        _textAnimalCount.text = $"动物数量: {count}";
    }

    /// <summary>
    /// 更新帧率显示
    /// </summary>
    private void UpdateFPSDisplay()
    {
        float fps = 1.0f / Time.deltaTime;
        _textFPS.text = $"帧率: {fps:F0} FPS";
    }

    /// <summary>
    /// 增加派发间隔按钮点击回调
    /// </summary>
    private void OnIncreaseIntervalClicked()
    {
        _spawnerManager.IncreaseAllSpawnInterval(IntervalStep);
    }

    /// <summary>
    /// 减少派发间隔按钮点击回调
    /// </summary>
    private void OnDecreaseIntervalClicked()
    {
        _spawnerManager.DecreaseAllSpawnInterval(IntervalStep);
    }
    #endregion
}

