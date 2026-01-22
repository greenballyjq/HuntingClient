using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;
using UnityEngine;

/// <summary>
/// 技能武器类
/// </summary>
public class SkillWeapon : MonoBehaviour
{
    /// <summary>
    /// 使用的子弹ID
    /// </summary>
    [Header("武器配置")]
    [SerializeField] private int bulletId = 1;

    /// <summary>
    /// 枪口位置
    /// </summary>
    [SerializeField] private Transform muzzlePoint;

    /// <summary>
    /// 是否平滑旋转
    /// </summary>
    [Header("旋转配置")]
    [SerializeField] private bool rotationSmooth = true;

    /// <summary>
    /// 旋转速度
    /// </summary>
    [SerializeField] private float rotationSpeed = 20f;

    /// <summary>
    /// 射击间隔（秒）
    /// </summary>
    private float _fireInterval;

    /// <summary>
    /// 上次射击的时间戳
    /// </summary>
    private float _lastFireTime;

    /// <summary>
    /// 当前瞄准目标
    /// </summary>
    private BaseAnimalBehaviour _currentTarget;

    /// <summary>
    /// 子弹管理器
    /// </summary>
    private BulletManager _bulletManager => GameServiceLocator.GetRoundManager<BulletManager>();

    private void Update()
    {
        UpdateAiming();
        UpdateShooting();
    }

    #region 公共方法
    /// <summary>
    /// 初始化技能武器
    /// </summary>
    /// <param name="fireInterval">射击间隔（秒）</param>
    public void Init(float fireInterval)
    {
        _fireInterval = fireInterval;
        _lastFireTime = Time.time - _fireInterval; // 允许立即射击
        Debug.Log($"[SkillWeapon] 初始化完成，射击间隔: {_fireInterval}秒");
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 更新瞄准逻辑
    /// </summary>
    private void UpdateAiming()
    {
        _currentTarget = FindNearestAnimal();
        if (_currentTarget == null)
            return;

        RotateToTarget(_currentTarget.transform.position);
    }

    /// <summary>
    /// 更新射击逻辑
    /// </summary>
    private void UpdateShooting()
    {
        if (_currentTarget == null)
            return;

        TryShoot();
    }

    /// <summary>
    /// 查找最近的动物
    /// </summary>
    private BaseAnimalBehaviour FindNearestAnimal()
    {
        BaseAnimalBehaviour[] animals = Object.FindObjectsOfType<BaseAnimalBehaviour>();
        if (animals == null || animals.Length == 0)
            return null;

        BaseAnimalBehaviour nearestAnimal = null;
        float nearestDistance = float.MaxValue;
        Vector3 weaponPosition = transform.position;

        foreach (var animal in animals)
        {
            if (animal == null)
                continue;

            float distance = Vector3.Distance(weaponPosition, animal.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestAnimal = animal;
            }
        }

        return nearestAnimal;
    }

    /// <summary>
    /// 旋转朝向目标
    /// </summary>
    private void RotateToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f; // 保持水平旋转

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (rotationSmooth)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }

    /// <summary>
    /// 尝试射击
    /// </summary>
    private void TryShoot()
    {
        // 检查射击间隔
        if (Time.time - _lastFireTime < _fireInterval)
            return;

        // 检查枪口位置
        if (muzzlePoint == null)
        {
            Debug.LogWarning("[SkillWeapon] 枪口位置为空，无法射击");
            return;
        }

        // 执行射击
        SpawnBulletAsync().Forget();
        PlayFireSound();
        _lastFireTime = Time.time;
    }

    /// <summary>
    /// 生成子弹
    /// </summary>
    private async UniTask SpawnBulletAsync()
    {
        Vector3 spawnPosition = muzzlePoint.position;
        Vector3 spawnDirection = muzzlePoint.forward;

        await _bulletManager.SpawnBullet(bulletId, spawnPosition, spawnDirection);
    }
    #endregion

    #region TODO: 测试代码
    // TODO: 音效系统待实现
    /// <summary>
    /// 射击音效
    /// </summary>
    [Header("音效")]
    [SerializeField] private AudioClip fireAudioClip;

    /// <summary>
    /// 音效组件
    /// </summary>
    private AudioSource _audioSource;

    private void Awake()
    {
        InitializeAudioSource();
    }

    /// <summary>
    /// 初始化音效组件
    /// </summary>
    private void InitializeAudioSource()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// 播放射击音效
    /// </summary>
    private void PlayFireSound()
    {
        if (fireAudioClip != null && _audioSource != null)
        {
            _audioSource.clip = fireAudioClip;
            _audioSource.Play();
        }
    }
    #endregion
}