using System;
using cfg;
using Cysharp.Threading.Tasks;
using Hunting.Game.Animal;
using Hunting.Game.Bullet;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.MaterialProperty;

namespace Hunting.Game
{
    public class PlayerControl : MonoBehaviour
    {
        #region 测试用

        private bool initialized;

        private async UniTask Init()
        {
            // 等待 HuntingGameConfigManager 初始化完成
            while (HuntingGameConfigManager.Instance == null || !HuntingGameConfigManager.Instance.Initialized)
            {
                await UniTask.Delay(10);
            }
            initialized = true;
        }

        #endregion
        
        #region 输入系统相关
        /// <summary>
        /// 输入系统
        /// </summary>
        private PlayerInputSet input;

        /// <summary>
        /// 获取屏幕坐标
        /// </summary>
        private void OnAimPositionPerformed(InputAction.CallbackContext context)
        {
            // 获取屏幕坐标
            aimPosition = context.ReadValue<Vector2>();
        }

        private void OnEnable()
        {
            // 启用输入系统并绑定事件
            input.Enable();
            input.Player.AimPosition.performed += OnAimPositionPerformed;
        }

        private void OnDisable()
        {
            // 禁用输入系统并解绑事件
            input.Disable();
            input.Player.AimPosition.performed -= OnAimPositionPerformed;

            // 隐藏瞄准线
            if (aimLine != null)
                aimLine.enabled = false;
        }
        #endregion

        #region 瞄准相关字段
        /// <summary>
        /// 触摸屏的瞄准位置（屏幕坐标）
        /// </summary>
        private Vector2 aimPosition;
        /// <summary>
        /// 主摄像机
        /// </summary>
        private Camera mainCamera;
        /// <summary>
        /// 瞄准线渲染
        /// </summary>
        private LineRenderer aimLine;
        /// <summary>
        /// 枪口位置
        /// </summary>
        private Transform muzzlePoint;
        #endregion

        #region 射击相关字段
        /// <summary>
        /// 上次射击的时间戳
        /// </summary>
        private float lastFireTime;
        /// <summary>
        /// 射击间隔
        /// </summary>
        private float fireInterval;
        /// <summary>
        /// 是否在有效操作区域内
        /// </summary>
        private bool isInValidArea;
        /// <summary>
        /// 特殊子弹剩余持续时间（秒）
        /// </summary>
        private float specialBulletRemainingTime;

        /// <summary>
        /// 是否处于特殊子弹状态
        /// </summary>
        private bool isUsingSpecialBullet;
        #endregion

        #region 编辑器可编辑字段
        [Header("旋转设置")]
        [SerializeField] private float rotationSpeed = 20f; // 枪械旋转平滑速度
        [SerializeField] private float aimDistance = 10f; // 瞄准目标点的世界空间深度

        [Header("瞄准线设置")]
        [SerializeField] private Color aimLineColor = Color.red; // 瞄准线颜色
        [SerializeField] private float aimLineWidth = 0.05f; // 瞄准线宽度
        [SerializeField] private float aimLineLength = 20f; // 瞄准线长度

        [Header("射击设置")]
        [SerializeField] private int currentBulletId = 1; // 当前子弹类型ID

        [Header("音效")]
        [SerializeField] private AudioClip audioClip;
        private AudioSource audioSource;
        #endregion
        private async void Awake()
        {
            // 初始化输入系统
            input = new PlayerInputSet();
            mainCamera = Camera.main;


            // 测试用
            await Init();

            // 获取组件
            audioSource = GetComponent<AudioSource>();

            // 初始化组件
            InitializeMuzzlePoint();
            InitializeAimLine();
            UpdateFireInterval();

            // 订阅事件
            Animal.AnimalBehavior.OnAnimalDropReward += OnAnimalDropReward;
        }

        private void OnAnimalDropReward(Animal.AnimalBehavior animal, EDropType dropType, int arg3)
        {
            if (dropType == EDropType.Bullet)
            {
                // 掉落子弹奖励，获取随机特殊子弹
                var specialBullet = HuntingGameConfigManager.Instance.GetRandomSpecialBullet();
                if (specialBullet != null)
                {
                    // 切换到特殊子弹
                    ChangeBullet(specialBullet.ID);
                    Debug.LogWarning($"[PlayerControl] 获得特殊子弹: {specialBullet.Name}，持续时间: {specialBullet.Duration}秒");
                }
            }
        }

        /// <summary>
        /// 初始化枪口位置点
        /// </summary>
        private void InitializeMuzzlePoint()
        {
            // 查找现有的枪口点，不存在则创建
            muzzlePoint = transform.Find("Muzzle");
            if (muzzlePoint == null)
            {
                GameObject muzzleObj = new GameObject("Muzzle");
                muzzleObj.transform.SetParent(transform);
                muzzleObj.transform.localPosition = new Vector3(0, 0, 1f); // 枪口在枪前方1米位置
                muzzlePoint = muzzleObj.transform;
            }
        }

        /// <summary>
        /// 初始化瞄准线组件
        /// </summary>
        private void InitializeAimLine()
        {
            // 获取或添加LineRenderer组件
            aimLine = GetComponent<LineRenderer>();
            if (aimLine == null)
                aimLine = gameObject.AddComponent<LineRenderer>();

            // 配置瞄准线外观
            aimLine.material = new Material(Shader.Find("Sprites/Default"));
            aimLine.startColor = aimLineColor;
            aimLine.endColor = aimLineColor;
            aimLine.startWidth = aimLineWidth;
            aimLine.endWidth = aimLineWidth;
            aimLine.positionCount = 2;    // 起点和终点两个点
            aimLine.enabled = true;       // 始终显示瞄准线
        }

        /// <summary>
        /// 根据子弹配置更新射击间隔
        /// </summary>
        private void UpdateFireInterval()
        {
            var bulletConfig = HuntingGameConfigManager.Instance.GetBullet(currentBulletId);
            if (bulletConfig != null)
            {
                // 射击间隔 = 1 / 射速（发/秒）
                fireInterval = 1f / bulletConfig.FireRate;
            }
        }

        private void Update()
        {
            // 测试用
            if (!initialized) return;

            // 按鼠标右键切换子弹
            if (Input.GetMouseButtonDown(1))
            {
                ChangeBullet((currentBulletId % 4) + 1);
                Debug.LogWarning($"当前子弹ID：{currentBulletId}");
            }

            // 检查操作区域
            CheckValidArea();

            // 更新瞄准方向
            UpdateAim();

            // 更新瞄准线显示
            UpdateAimLine();

            // 处理射击逻辑
            UpdateShooting();

            // 更新特殊子弹计时器
            UpdateSpecialBulletTimer();
        }

        /// <summary>
        /// 检查输入位置是否在有效操作区域内
        /// </summary>
        private void CheckValidArea()
        {
            float screenHeight = Screen.height;
            float minY = screenHeight * 0.2f; // 底部20%为无效区域
            float maxY = screenHeight * 0.8f; // 顶部20%为无效区域
            isInValidArea = aimPosition.y >= minY && aimPosition.y <= maxY;
        }

        /// <summary>
        /// 更新枪械瞄准方向
        /// </summary>
        private void UpdateAim()
        {
            if (!isInValidArea) return;

            // 按下射击键时更新瞄准
            if (input.Player.Shoot.IsPressed())
            {
                // 将屏幕坐标转换为世界坐标
                Vector3 targetPosition = mainCamera.ScreenToWorldPoint(
                    new Vector3(aimPosition.x, aimPosition.y, aimDistance));

                // 保持枪械高度不变，只在水平面旋转
                targetPosition.y = transform.position.y;

                // 计算朝向目标的方向向量
                Vector3 direction = targetPosition - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                // 平滑旋转到目标方向
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        /// <summary>
        /// 击中点标记小球
        /// </summary>
        private GameObject hitMarker;

        /// <summary>
        /// 更新瞄准线显示
        /// </summary>
        private void UpdateAimLine()
        {
            if (aimLine == null || muzzlePoint == null) return;

            // 设置瞄准线起点为枪口位置
            Vector3 startPoint = muzzlePoint.position;
            // 设置瞄准线方向为枪口前方
            Vector3 direction = muzzlePoint.forward;

            // 进行射线检测，排除子弹层
            int layerMask = ~(1 << LayerMask.NameToLayer("Bullet")); // 排除子弹层
            RaycastHit hit;
            bool hasHit = Physics.Raycast(startPoint, direction, out hit, aimLineLength, layerMask);

            // 根据是否击中物体设置瞄准线颜色
            if (hasHit)
            {
                aimLine.startColor = Color.green;
                aimLine.endColor = Color.green;

                // 在击中点创建标记小球
                CreateHitMarker(hit.point);
            }
            else
            {
                aimLine.startColor = Color.red;
                aimLine.endColor = Color.red;

                // 隐藏击中点标记
                HideHitMarker();
            }

            // 设置瞄准线终点
            Vector3 endPoint = hasHit ? hit.point : startPoint + direction * aimLineLength;

            // 更新LineRenderer的点位
            aimLine.SetPosition(0, startPoint);
            aimLine.SetPosition(1, endPoint);
        }


        /// <summary>
        /// 在击中位置创建标记小球
        /// </summary>
        /// <param name="hitPoint">击中点位置</param>
        private void CreateHitMarker(Vector3 hitPoint)
        {
            if (hitMarker == null)
            {
                // 创建小球对象
                hitMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hitMarker.name = "HitMarker";

                // 移除碰撞体
                Collider collider = hitMarker.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                // 设置材质和颜色
                Renderer renderer = hitMarker.GetComponent<Renderer>();
                renderer.material.color = Color.green;

                // 设置小球大小（增大到0.3）
                hitMarker.transform.localScale = Vector3.one * 0.3f;
            }

            // 更新位置并显示
            hitMarker.transform.position = hitPoint;
            hitMarker.SetActive(true);
        }

        /// <summary>
        /// 隐藏击中点标记
        /// </summary>
        private void HideHitMarker()
        {
            if (hitMarker != null)
            {
                hitMarker.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // 取消订阅事件
            Animal.AnimalBehavior.OnAnimalDropReward -= OnAnimalDropReward;


            // 销毁击中点标记
            if (hitMarker != null)
                Destroy(hitMarker);
        }

        /// <summary>
        /// 更新射击逻辑
        /// </summary>
        private void UpdateShooting()
        {
            if (!isInValidArea) return;

            // 检测射击输入
            if (input.Player.Shoot.IsPressed())
            {
                TryShoot();
            }
        }

        /// <summary>
        /// 尝试发射子弹（考虑射击冷却）
        /// </summary>
        private void TryShoot()
        {
            // 检查射击间隔
            if (Time.time - lastFireTime >= fireInterval)
            {
                SpawnBullet();
                lastFireTime = Time.time;

                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }

        /// <summary>
        /// 生成并初始化子弹
        /// </summary>
        private void SpawnBullet()
        {
            // 从配置管理器获取子弹数据
            var bulletConfig = HuntingGameConfigManager.Instance.GetBullet(currentBulletId);
            if (bulletConfig == null) return;

            // 根据子弹类型获取预制体名称
            string bulletName = GetBulletPrefabName(bulletConfig.BulletType);
            // 从Resources加载子弹预制体
            GameObject bulletPrefab = Resources.Load<GameObject>($"Bullets/{bulletName}");

            if (bulletPrefab == null)
            {
                Debug.LogError($"子弹预制体不存在: {bulletName}");
                return;
            }

            // 实例化子弹对象
            GameObject bulletObj = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
            BulletBehavior bullet = bulletObj.GetComponent<BulletBehavior>();

            // 初始化子弹行为
            if (bullet != null)
            {
                bullet.Initialize(bulletConfig, muzzlePoint.position, muzzlePoint.forward);
            }
        }

        /// <summary>
        /// 根据子弹类型枚举获取预制体资源名称
        /// </summary>
        private string GetBulletPrefabName(EBulletType bulletType)
        {
            return bulletType switch
            {
                EBulletType.Normal => "Bullet_Normal",
                EBulletType.Explosive => "Bullet_Explosive",
                EBulletType.HighDamage => "Bullet_HighDamage",
                EBulletType.HighSpeed => "Bullet_HighSpeed",
                _ => "Bullet_Normal"
            };
        }

        /// <summary>
        /// 检查并更新特殊子弹持续时间
        /// </summary>
        private void UpdateSpecialBulletTimer()
        {
            if (!isUsingSpecialBullet) return;

            specialBulletRemainingTime -= Time.deltaTime;

            if (specialBulletRemainingTime <= 0f)
            {
                // 恢复为普通子弹
                ChangeBullet(1);
                isUsingSpecialBullet = false;
                Debug.LogWarning("[PlayerControl] 特殊子弹时间到，已切回普通子弹");
            }
        }

        #region 公共方法

        /// <summary>
        /// 切换当前使用的子弹类型
        /// </summary>
        /// <param name="newBulletId">新的子弹ID</param>
        public void ChangeBullet(int newBulletId)
        {
            currentBulletId = newBulletId;
            UpdateFireInterval(); // 更新射击间隔

            var bulletConfig = HuntingGameConfigManager.Instance.GetBullet(newBulletId);

            // 如果是特殊子弹（持续时间 > 0），则启动计时
            if (bulletConfig != null && bulletConfig.Duration > 0)
            {
                specialBulletRemainingTime = bulletConfig.Duration;
                isUsingSpecialBullet = true;
            }
            else
            {
                isUsingSpecialBullet = false;
            }
        }

        #endregion
    }

}