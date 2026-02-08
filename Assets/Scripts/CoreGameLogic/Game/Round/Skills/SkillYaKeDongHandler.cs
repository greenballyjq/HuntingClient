using cfg.HuntingConfig;
using cfg.HuntingConfig.Skill;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 亚克东技能处理器
/// </summary>
public class SkillYaKeDongHandler : BaseSkillHandler
{
    /// <summary>
    /// 物种配置
    /// </summary>
    private Specie _specieData;

    /// <summary>
    /// 动物预制体
    /// </summary>
    private GameObject _animalPrefab;

    /// <summary>
    /// 生成数量
    /// </summary>
    private int _spawnCount;

    /// <summary>
    /// 生成间隔
    /// </summary>
    private float _spawnInterval;

    /// <summary>
    /// 生成计时器
    /// </summary>
    private float _spawnTimer;

    /// <summary>
    /// 已生成数量
    /// </summary>
    private int _spawnedCount;

    /// <summary>
    /// 屏幕四角世界坐标
    /// </summary>
    private Vector3[] _screenCorners = new Vector3[4];

    /// <summary>
    /// 目标点距离玩家距离
    /// </summary>
    private const float  TARGET_DISTANCE_FROM_PLAYER = 7f;

    /// <summary>
    /// 目标点随机范围X
    /// </summary>
    private const float TARGET_RANDOM_RANGEX = 4.5f;

    /// <summary>
    /// 目标点随机范围Z
    /// </summary>
    private const float TARGET_RANDOM_RANGEZ = 3f;

    /// <summary>
    /// 相机深度
    /// </summary>
    private const float CAMERA_DEPTH = 3.5f;

    /// <summary>
    /// 底部屏幕角Y轴偏移
    /// </summary>
    private const float BOTTOM_CORNER_Y_OFFSET = 0.0f;

    /// <summary>
    /// 事件管理器
    /// </summary>
    private EventManager _eventManager => GameServiceLocator.EventManager;

    /// <summary>
    /// 资源管理器
    /// </summary>
    private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;

    /// <summary>
    /// 对象池管理器
    /// </summary>
    private GameObjectPoolManager _gameObjectPoolManager => GameServiceLocator.GameObjectPoolManager;

    /// <summary>
    /// 相机管理器
    /// </summary>
    private CameraManager _cameraManager => GameServiceLocator.GetAppManager<CameraManager>();

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        var skillParam = _configManager.GetSkillYaKeDong(context.SkillData.ParamTableID);

        // 初始化变量
        _spawnTimer = 0f;
        _spawnedCount = 0;

        // 缓存配置
        _specieData = _configManager.GetSpecie(skillParam.SpecieDataID);
        _animalPrefab = await _resourceManager.LoadAssetAsync<GameObject>(skillParam.SkillPrefabResourcePath);

        // 随机生成数量
        _spawnCount = skillParam.SpawnCount[Random.Range(0, skillParam.SpawnCount.Length)];

        // 根据技能持续时间计算召唤间隔
        _spawnInterval = context.SkillData.Duration / _spawnCount;

        // 缓存屏幕四角世界坐标
        CacheScreenCorners();

        // 模拟播放动画
        await UniTask.Delay(1000);
    }

    protected override void OnSkillUpdate(float dt)
    {
        if (_spawnedCount >= _spawnCount)
            return;

        _spawnTimer += dt;

        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0f;
            SpawnThreeKPCoinManAnimal();
            _spawnedCount++;
        }
    }

    protected override void OnSkillEnd()
    {
        _specieData = null;
        _animalPrefab = null;
    }

    #region 私有方法
    /// <summary>
    /// 缓存屏幕四角世界坐标
    /// </summary>
    private void CacheScreenCorners()
    {
        Camera cam = _cameraManager.MainCamera;

        _screenCorners[0] = cam.ViewportToWorldPoint(new Vector3(0f, BOTTOM_CORNER_Y_OFFSET, CAMERA_DEPTH));

        _screenCorners[1] = cam.ViewportToWorldPoint(new Vector3(1f, BOTTOM_CORNER_Y_OFFSET, CAMERA_DEPTH));

        _screenCorners[2] = cam.ViewportToWorldPoint(new Vector3(1f, 1f, CAMERA_DEPTH));

        _screenCorners[3] = cam.ViewportToWorldPoint(new Vector3(0f, 1f, CAMERA_DEPTH));
    }

    /// <summary>
    /// 派发三千盘金币人动物
    /// </summary>
    private void SpawnThreeKPCoinManAnimal()
    {
        // 屏幕四角随机起点
        Vector3 startPos = _screenCorners[Random.Range(0, 4)];

        // 玩家面前指定距离，矩形范围内随机终点
        Vector3 playerPos = _player.position;
        Vector3 playerForward = _player.forward;
        Vector3 targetCenter = playerPos + playerForward * TARGET_DISTANCE_FROM_PLAYER;
        Vector3 endPos = targetCenter + new Vector3(
            Random.Range(-TARGET_RANDOM_RANGEX, TARGET_RANDOM_RANGEX),
            0f,
            Random.Range(-TARGET_RANDOM_RANGEZ, TARGET_RANDOM_RANGEZ)
        );

        // 计算方向
        Vector3 direction = (endPos - startPos).normalized;

        GameObject go = _gameObjectPoolManager.Spawn(_animalPrefab);
        go.transform.position = startPos;

        var animal = go.GetComponent<BaseAnimalBehaviour>();
        animal.Init(_specieData);
        animal.Moveable.SetDirection(direction);
        animal.Moveable.SetTargetPosition(endPos);

        _eventManager.Trigger(AnimalEvents.AnimalGenerated, new AnimalGeneratedEventArgs
        {
            Animal = animal
        });
    }
    #endregion
}
