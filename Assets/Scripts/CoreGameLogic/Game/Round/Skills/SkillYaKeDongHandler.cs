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
    /// 技能参数缓存
    /// </summary>
    private SkillYaKeDong _skillParamCache;

    /// <summary>
    /// 物种配置缓存
    /// </summary>
    private Specie _specieDataCache;

    /// <summary>
    /// 技能预制体缓存
    /// </summary>
    private GameObject _skillPrefabCache;

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
    /// 屏幕四角世界坐标缓存
    /// </summary>
    private Vector3[] _screenCornersCache = new Vector3[4];

    private EventManager _eventManager;
    private GameObjectPoolManager _gameObjectPoolManager;
    private CameraManager _cameraManager;

    private const float TARGET_DISTANCE_FROM_PLAYER = 8f;
    private const float TARGET_RANDOM_RANGEX = 3;
    private const float TARGET_RANDOM_RANGEZ = 5f;
    private const float CAMERA_DEPTH = 3f;

    public SkillYaKeDongHandler() : base() 
    {
        _effectManager = GameServiceLocator.EffectManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _cameraManager = GameServiceLocator.GetAppManager<CameraManager>();

        CacheScreenCorners();
    }

    protected override async UniTask OnSkillStart(SkillContext context)
    {
        if (_skillParamCache == null)
            _skillParamCache = _configManager.GetSkillYaKeDong(context.SkillData.ParamTableID);

        if (_specieDataCache == null)
            _specieDataCache = _configManager.GetSpecie(_skillParamCache.SpecieDataID);

        if(_skillPrefabCache == null)
            _skillPrefabCache = _configManager.SkillRefSo.GetSkillPrefab(context.SkillData.ID);

        _spawnTimer = 0f;
        _spawnedCount = 0;

        _spawnCount = _skillParamCache.SpawnCount[Random.Range(0, _skillParamCache.SpawnCount.Length)];

        _spawnInterval = context.SkillData.Duration / _spawnCount;

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
            SpawnThreeKPCoinMan();
            _spawnedCount++;
        }
    }

    protected override void OnSkillEnd(){}

    #region 私有方法
    /// <summary>
    /// 缓存屏幕四角世界坐标
    /// </summary>
    private void CacheScreenCorners()
    {
        Camera cam = _cameraManager.MainCamera;
        _screenCornersCache[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0, CAMERA_DEPTH));
        _screenCornersCache[1] = cam.ViewportToWorldPoint(new Vector3(1f, 0, CAMERA_DEPTH));
        _screenCornersCache[2] = cam.ViewportToWorldPoint(new Vector3(1f, 1f, CAMERA_DEPTH));
        _screenCornersCache[3] = cam.ViewportToWorldPoint(new Vector3(0f, 1f, CAMERA_DEPTH));
    }

    /// <summary>
    /// 派发三千盘金币人
    /// </summary>
    private void SpawnThreeKPCoinMan()
    {
        Vector3 startPos = _screenCornersCache[Random.Range(0, 4)];

        // 玩家面前指定距离，矩形范围内随机终点
        Vector3 playerPos = _player.position;
        Vector3 playerForward = _player.forward;
        Vector3 targetCenter = playerPos + playerForward * TARGET_DISTANCE_FROM_PLAYER;
        Vector3 endPos = targetCenter + new Vector3(
            Random.Range(-TARGET_RANDOM_RANGEX, TARGET_RANDOM_RANGEX),
            0f,
            Random.Range(-TARGET_RANDOM_RANGEZ, TARGET_RANDOM_RANGEZ)
        );

        Vector3 direction = (endPos - startPos).normalized;

        GameObject go = _gameObjectPoolManager.Spawn(_skillPrefabCache);
        go.transform.position = startPos;

        var animal = go.GetComponent<BaseAnimalBehaviour>();
        animal.Init(_specieDataCache);
        animal.Moveable.SetDirection(direction);
        animal.Moveable.SetTargetPosition(endPos);

        _eventManager.Trigger(AnimalEvents.AnimalGenerated, new AnimalGeneratedEventArgs
        {
            Animal = animal
        });
    }
    #endregion
}
