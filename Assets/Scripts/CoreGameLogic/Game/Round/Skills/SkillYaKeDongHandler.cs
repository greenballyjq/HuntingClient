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
    /// 技能参数
    /// </summary>
    private SkillYaKeDong _skillParam;

    /// <summary>
    /// 物种配置
    /// </summary>
    private Specie _specieData;

    /// <summary>
    /// 三千盘金币人预制体
    /// </summary>
    private GameObject _ThreeKPCoinManPrefab;

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

    private const float TARGET_DISTANCE_FROM_PLAYER = 8f;
    private const float TARGET_RANDOM_RANGEX = 3;
    private const float TARGET_RANDOM_RANGEZ = 5f;
    private const float CAMERA_DEPTH = 3f;

    protected override void OnInit() 
    {
        EffectManager = GameServiceLocator.EffectManager;
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;

        CacheScreenCorners();

        _skillParam = ConfigManager.GetSkillYaKeDong(SkillContext.SkillData.ParamTableID);
        _specieData = ConfigManager.GetSpecie(_skillParam.SpecieDataID);
        _ThreeKPCoinManPrefab = ConfigManager.SkillRefSo.GetSkillPrefab(SkillContext.SkillData.ID);
    }

    protected override UniTask OnSkillStart()
    {
        _spawnTimer = 0f;
        _spawnedCount = 0;

        _spawnCount = _skillParam.SpawnCount[Random.Range(0, _skillParam.SpawnCount.Length)];

        _spawnInterval = SkillContext.SkillData.Duration / _spawnCount;

        return UniTask.CompletedTask;
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
        var cam = GameServiceLocator.GetAppManager<CameraManager>().MainCamera;

        _screenCornersCache[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0, CAMERA_DEPTH));
        _screenCornersCache[1] = cam.ViewportToWorldPoint(new Vector3(1f, 0, CAMERA_DEPTH));
        _screenCornersCache[2] = cam.ViewportToWorldPoint(new Vector3(1f, 1f, CAMERA_DEPTH));
        _screenCornersCache[3] = cam.ViewportToWorldPoint(new Vector3(0f, 1f, CAMERA_DEPTH));
    }

    /// <summary>
    /// 生成三千盘金币人
    /// </summary>
    private void SpawnThreeKPCoinMan()
    {
        Vector3 startPos = _screenCornersCache[Random.Range(0, 4)];

        // 玩家面前指定距离，矩形范围内随机终点
        Vector3 targetCenter = Player.position + Player.forward * TARGET_DISTANCE_FROM_PLAYER;
        Vector3 endPos = targetCenter + new Vector3(
            Random.Range(-TARGET_RANDOM_RANGEX, TARGET_RANDOM_RANGEX),
            0f,
            Random.Range(-TARGET_RANDOM_RANGEZ, TARGET_RANDOM_RANGEZ)
        );

        Vector3 direction = (endPos - startPos).normalized;

        GameObject go = _gameObjectPoolManager.Spawn(_ThreeKPCoinManPrefab);
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
