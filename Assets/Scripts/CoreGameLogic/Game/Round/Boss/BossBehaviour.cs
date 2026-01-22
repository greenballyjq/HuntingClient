using System.Collections.Generic;
using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using GameFramework.Manager;
using Hunting.Game.Animal;
using UnityEngine;

public class BossBehaviour : MonoBehaviour, IDamageable
{
    /// <summary>
    /// Boss移动目标点
    /// </summary>
    private Transform[] _randomMovePoints;

    /// <summary>
    /// 状态机
    /// </summary>
    private StateMachine _stateMachine;

    /// <summary>
    /// Boss 随机移动状态
    /// </summary>
    private BossRandomMoveState _randomMoveState;

    /// <summary>
    /// Boss 死亡状态
    /// </summary>
    private BossDeathState _deathState;

    /// <summary>
    /// Boss Idle状态
    /// </summary>
    private BossIdleState _idleState;

    /// <summary>
    /// Boss Hit状态
    /// </summary>
    private BossHitState _hitState;

    /// <summary>
    /// Boss Enter状态
    /// </summary>
    private BossEnterState _enterState;

    /// <summary>
    /// 配置数据
    /// </summary>
    public Specie SpecieData { get; private set; }

    /// <summary>
    /// 最大生命值
    /// </summary>
    public float MaxHP { get; private set; }

    /// <summary>
    /// 当前生命值
    /// </summary>
    public float CurrentHP { get; private set; }

    /// <summary>
    /// 当前移动速度
    /// </summary>
    public float CurrentMoveSpeed { get; private set; }

    /// <summary>
    /// 当前移动方向
    /// </summary>
    public Vector3 CurrentDirection { get; private set; }

    /// <summary>
    /// 是否被击中标志
    /// </summary>
    public bool IsHit { get; private set; }

    /// <summary>
    /// 动画组件
    /// </summary>
    public Animator Animator { get; private set; }

    /// <summary>
    /// 渲染器组件
    /// </summary>
    private Renderer _renderer;

    /// <summary>
    /// 材质
    /// </summary>
    private Material _material;

    /// <summary>
    /// 隐藏地图小怪生成器
    /// </summary>
    private HiddenMapSpawner[] _hiddenMapSpawners;

    /// <summary>
    /// 跟随动物生成时间
    /// </summary>
    private const float FOLLOW_ANIMALS_SPAWN_DURATION = 10f;

    /// <summary>
    /// 跟随动物召唤时间
    /// </summary>
    private const float FOLLOW_ANIMALS_CALL_DURATION = 15f;

    /// <summary>
    /// 跟随动物生成定时器ID
    /// </summary>
    private int _spawnAnimalsTimerId;

    /// <summary>
    /// 召唤动物定时器ID
    /// </summary>
    private int _callAnimalsTimerId;

    /// <summary>
    /// 跟随动物最大数量
    /// </summary>
    public const int MAX_FOLLOW_ANIMALS = 8;

    /// <summary>
    /// 场上最大动物数量
    /// </summary>
    public const int MAX_ANIMAL_COUNT = 30;

    /// <summary>
    /// 每轮生成跟随动物数量
    /// </summary>
    public const int PER_SPAWN_ANIMAL_COUNT = 10;

    /// <summary>
    /// 跟随时长
    /// </summary>
    public const float FOLLOW_DURATION = 10f;

    /// <summary>
    /// 跟随动物数组
    /// </summary>
    private List<BaseAnimalBehaviour> _followAnimals;

    /// <summary>
    /// 跟随动物活跃数组
    /// </summary>
    private bool[] _followAnimalActive;

    private EventManager _eventManager => GameServiceLocator.EventManager;

    private TimerManager _timerManager => GameServiceLocator.TimerManager;

    private EffectManager _effectManager =>
        GameServiceLocator.GetFrameworkManager<EffectManager>();

    private AnimalManager _animalManager => GameServiceLocator.GetRoundManager<AnimalManager>();

    private void Awake()
    {
        // 获取组件
        Animator = GetComponentInChildren<Animator>();

        // 获取渲染器和材质
        _renderer = GetComponentInChildren<Renderer>();
        _material = _renderer.material;

        // 查找所有Boss移动点
        CollectBossMovePoints();
        // 查找所有的小怪生成器
        CollectHiddenMapSpawners();

        // 创建状态机
        _stateMachine = new StateMachine();
        _enterState = new BossEnterState(this, _stateMachine, "Enter");
        _idleState = new BossIdleState(this, _stateMachine, "Idle");
        _hitState = new BossHitState(this, _stateMachine, "Hit");
        _randomMoveState = new BossRandomMoveState(this, _stateMachine, "Move", _randomMovePoints);
        _deathState = new BossDeathState(this, _stateMachine, "Death");
    }

    /// <summary>
    /// Boss初始化
    /// </summary>
    /// <param name="data">物种数据</param>
    public void Init(Specie data)
    {
        SpecieData = data;
        MaxHP = 100;
        CurrentHP = 100;
        CurrentMoveSpeed = 6;
        CurrentDirection = transform.forward;

        _followAnimals = new List<BaseAnimalBehaviour>(MAX_FOLLOW_ANIMALS);
        _followAnimalActive = new bool[MAX_FOLLOW_ANIMALS];

        _spawnAnimalsTimerId =
            _timerManager.StartTimer(FOLLOW_ANIMALS_SPAWN_DURATION, SpawnFollowAnimals, repeat: TimerManager.LOOP);
        _callAnimalsTimerId =
            _timerManager.StartTimer(FOLLOW_ANIMALS_CALL_DURATION, CallAnimals, repeat: TimerManager.LOOP);

        // _eventManager.AddListener(AnimalEvents.AnimalGenerated, OnAnimalGenerated);
        _eventManager.AddListener(AnimalEvents.AnimalReachedWall, OnAnimalReachedWall);
        _eventManager.AddListener(AnimalEvents.AnimalEnteredDeath, OnAnimalDying);

        _stateMachine.Init(_enterState);
    }

    private void Release()
    {
        _eventManager.RemoveListener(AnimalEvents.AnimalEnteredDeath, OnAnimalDying);
        _eventManager.AddListener(AnimalEvents.AnimalReachedWall, OnAnimalReachedWall);
        _timerManager.StopTimer(_spawnAnimalsTimerId);
        _timerManager.StopTimer(_callAnimalsTimerId);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        _stateMachine?.DoUpdate(dt);
    }

    public void AddFollowAnimal(BaseAnimalBehaviour animalBehavior)
    {
        if (_followAnimals.Count >= MAX_FOLLOW_ANIMALS) return;
        _followAnimals.Add(animalBehavior);
        int index = _followAnimals.IndexOf(animalBehavior);
        _followAnimalActive[index] = true;
    }

    public void RemoveFollowAnimal(BaseAnimalBehaviour animalBehavior)
    {
        int index = _followAnimals.IndexOf(animalBehavior);
        if (index == -1) return;
        _followAnimals.Remove(animalBehavior);
        _followAnimalActive[index] = false;
    }

    #region 公共方法

    /// <summary>
    /// Boss受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="hitPoint">击中点位置</param>
    /// <param name="hitNormal">击中点法线</param>
    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // 扣除生命值
        CurrentHP -= damage;

        IsHit = true;

        // PlayAnimationTrigger("Hit");
    }

    /// <summary>
    /// 重置受击标志
    /// </summary>
    public void ResetHitFlag()
    {
        IsHit = false;
    }

    /// <summary>
    /// 播放动画 bool参数
    /// </summary>
    public void PlayAnimationBool(string animationName)
    {
        Animator.SetBool(animationName, true);
    }

    /// <summary>
    /// 播放动画 trigger参数
    /// </summary>
    public void PlayAnimationTrigger(string animationName)
    {
        Animator.SetTrigger(animationName);
    }

    /// <summary>
    /// 停止动画
    /// </summary>
    public void StopAnimation(string animationName)
    {
        Animator.SetBool(animationName, false);
    }

    /// <summary>
    /// 设置移动速度
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        CurrentMoveSpeed = speed;
    }

    /// <summary>
    /// 设置移动方向
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        CurrentDirection = direction;
    }

    /// <summary>
    /// 应用速度倍率
    /// </summary>
    public void ApplySpeedMultiplier(float multiplier)
    {
        float finalSpeed = CurrentMoveSpeed * multiplier;
    }

    /// <summary>
    /// 获取动画时长
    /// </summary>
    public float GetAnimationLength(string animationName)
    {
        if (Animator == null || Animator.runtimeAnimatorController == null)
            return 0.5f;

        AnimationClip[] clips = Animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == animationName)
                return clip.length;
        }

        return 0.5f;
    }

    /// <summary>
    /// 移动到目标点
    /// </summary>
    /// <param name="point">目标点位置</param>
    public bool MoveTo(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        direction.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
            Time.deltaTime * 5f);

        transform.Translate(direction * (CurrentMoveSpeed * Time.deltaTime), Space.World);

        return Vector3.Distance(transform.position, point) < 0.5f;
    }

    /// <summary>
    /// 获取随机移动状态
    /// </summary>
    public BossRandomMoveState GetRandomMoveState()
    {
        return _randomMoveState;
    }

    /// <summary>
    /// 获取死亡状态
    /// </summary>
    public BossDeathState GetDeathState()
    {
        return _deathState;
    }

    /// <summary>
    /// 获取 idle状态
    /// </summary>
    /// <returns></returns>
    public BossIdleState GetIdleState()
    {
        return _idleState;
    }

    /// <summary>
    /// 获取 Hit状态
    /// </summary>
    /// <returns></returns>
    public BossHitState GetHitState()
    {
        return _hitState;
    }

    /// <summary>
    /// 获取 Enter状态
    /// </summary>
    /// <returns></returns>
    public BossEnterState GetEnterState()
    {
        return _enterState;
    }

    /// <summary>
    /// 进入战斗
    /// </summary>
    public void EnterCombat()
    {
        _stateMachine.ChangeState(_randomMoveState);
    }

    /// <summary>
    /// 获取可用的Follow动物索引
    /// </summary>
    /// <returns></returns>
    public int GetAvailableFollowIndex()
    {
        for (int i = 0; i < _followAnimalActive.Length; i++)
        {
            if (!_followAnimalActive[i]) return i;
        }

        return -1;
    }

    public void TriggerBossDyingEvent()
    {
        _eventManager.Trigger(BossEvents.BossDying, new BossDyingEventArgs
        {
            Sender = this,
            Boss = this
        });
        Release();
    }

    public void TriggerBossDiedEvent()
    {
        _eventManager.Trigger(BossEvents.BossDied, new BossDiedEventArgs
        {
            Sender = this,
            Boss = this
        });
        Destroy(gameObject);
    }

    public void PlayBossDeathEffect()
    {
        _effectManager.PlayOneShotAsync("Arts/Prefabs/Particles/Animal_Smoke", transform.position, Quaternion.identity).Forget();
    }

    /// <summary>
    /// 收集Boss移动点
    /// </summary>
    private void CollectBossMovePoints()
    {
        var bossMovePointsParent = GameObject.Find("BossMovePoints");
        var movePointList = new List<Transform>();
        for (int i = 0; i < bossMovePointsParent.transform.childCount; i++)
            movePointList.Add(bossMovePointsParent.transform.GetChild(i));
        _randomMovePoints = movePointList.ToArray();
    }

    private void CollectHiddenMapSpawners()
    {
        _hiddenMapSpawners = FindObjectsOfType<HiddenMapSpawner>();
    }

    #endregion

    #region 事件相关

    private void OnAnimalReachedWall(AnimalReachedWallEventArgs obj)
    {
        var index = _followAnimals.IndexOf(obj.Animal);
        if (index == -1) return;
        _followAnimalActive[index] = false;
        _followAnimals.Remove(obj.Animal);
    }

    private void OnAnimalDying(AnimalEnteredDeathEventArgs obj)
    {
        var index = _followAnimals.IndexOf(obj.Animal);
        if (index == -1) return;
        _followAnimalActive[index] = false;
        _followAnimals.Remove(obj.Animal);
    }

    private async void SpawnFollowAnimals()
    {
        if (_animalManager.GetActiveAnimalCount() >= MAX_ANIMAL_COUNT) return;

        // 如果spawner数组为空，直接返回
        if (_hiddenMapSpawners == null || _hiddenMapSpawners.Length == 0)
        {
            Debug.LogWarning("[BossBehaviour] 没有找到HiddenMapSpawner，无法生成跟随动物");
            return;
        }

        int spawnCount = 0;
        int lastSpawnCount = 0;
        while (spawnCount < PER_SPAWN_ANIMAL_COUNT)
        {
            lastSpawnCount = spawnCount;
            foreach (var spawner in _hiddenMapSpawners)
            {
                if (spawner == null) continue;

                await spawner.SpawnAsync();
                spawnCount++;
                if (_animalManager.GetActiveAnimalCount() >= MAX_ANIMAL_COUNT) return;
            }

            // 防止无限循环：如果遍历完所有spawner后spawnCount没有增加，说明无法继续生成，退出循环
            if (spawnCount == lastSpawnCount)
            {
                Debug.LogWarning("[BossBehaviour] 无法生成任何动物，退出生成循环");
                break;
            }
        }
    }

    private void CallAnimals()
    {
        // _eventManager.Trigger(BossEvents.BossCall, new BossCallEventArgs { Boss = this, GuardDuration = FOLLOW_DURATION });
        var followAnimals = _animalManager.GetCloseAnimalsFromTargetPosition(transform.position, MAX_FOLLOW_ANIMALS);
        for (var i = 0; i < followAnimals.Count; i++)
        {
            //followAnimals[i].SetGuardTarget(transform, i);
        }
    }

    #endregion
}