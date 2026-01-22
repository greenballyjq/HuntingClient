using UnityEngine;

/// <summary>
/// 武器视觉效果
/// </summary>
public class WeaponVisual : MonoBehaviour
{
    [Header("枪口Transform")] [SerializeField]
    private Transform muzzlePoint;
    
    /// <summary>
    /// 枪口位置
    /// </summary>
    public Transform MuzzlePoint => muzzlePoint;
    
    [Header("瞄准线设置")]
    [SerializeField] private Color aimLineColor = Color.red; // 瞄准线颜色
    [SerializeField] private float aimLineWidth = 0.05f; // 瞄准线宽度
    [SerializeField] private float aimLineLength = 20f; // 瞄准线长度

    /// <summary>
    /// 枪口瞄准辅助线
    /// </summary>
    private LineRenderer _aimLine;
    
    private void Awake()
    {
        _aimLine = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        InitializeAimLine();
    }
    
    /// <summary>
    /// 初始化瞄准线组件
    /// </summary>
    private void InitializeAimLine()
    {
        // 获取或添加LineRenderer组件
        _aimLine = GetComponent<LineRenderer>();
        if (_aimLine == null)
            _aimLine = gameObject.AddComponent<LineRenderer>();

        // 配置瞄准线外观
        _aimLine.material = new Material(Shader.Find("Sprites/Default"));
        _aimLine.startColor = aimLineColor;
        _aimLine.endColor = aimLineColor;
        _aimLine.startWidth = aimLineWidth;
        _aimLine.endWidth = aimLineWidth;
        _aimLine.positionCount = 2;    // 起点和终点两个点
        _aimLine.enabled = true;       // 始终显示瞄准线
    }

    private void Update()
    {
        UpdateAimLine();
    }

    private void UpdateAimLine()
    {
        if (_aimLine == null || muzzlePoint == null) return;
        // 设置瞄准线起点为枪口位置
        Vector3 startPoint = muzzlePoint.position;
        // 获取枪口方向
        Vector3 direction = muzzlePoint.forward;

        // 进行射线检测，排除子弹层
        int layerMask = LayerMask.NameToLayer("Animal");
        RaycastHit hit;
        bool hasHit = Physics.Raycast(startPoint, direction, out hit, aimLineLength, layerMask);

        // 根据是否击中物体设置瞄准线颜色
        if (hasHit)
        {
            _aimLine.startColor = Color.green;
            _aimLine.endColor = Color.green;

            // 在击中点创建标记小球
            CreateHitMarker(hit.point);
        }
        else
        {
            _aimLine.startColor = Color.red;
            _aimLine.endColor = Color.red;

            // 隐藏击中点标记
            HideHitMarker();
        }

        // 设置瞄准线终点
        Vector3 endPoint = hasHit ? hit.point : startPoint + direction * aimLineLength;

        // 更新LineRenderer的点位
        _aimLine.SetPosition(0, startPoint);
        _aimLine.SetPosition(1, endPoint);
    }
    
    /// <summary>
    /// 击中点标记小球
    /// </summary>
    private GameObject _hitMarker;
    
    /// <summary>
    /// 在击中位置创建标记小球
    /// </summary>
    /// <param name="hitPoint">击中点位置</param>
    private void CreateHitMarker(Vector3 hitPoint)
    {
        if (_hitMarker == null)
        {
            // 创建小球对象
            _hitMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _hitMarker.name = "HitMarker";

            // 移除碰撞体
            Collider collider = _hitMarker.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            // 设置材质和颜色
            Renderer renderer = _hitMarker.GetComponent<Renderer>();
            renderer.material.color = Color.green;

            // 设置小球大小（增大到0.3）
            _hitMarker.transform.localScale = Vector3.one * 0.1f;
        }

        // 更新位置并显示
        _hitMarker.transform.position = hitPoint;
        _hitMarker.SetActive(true);
    }
    
    /// <summary>
    /// 隐藏击中点标记
    /// </summary>
    private void HideHitMarker()
    {
        if (_hitMarker != null)
        {
            _hitMarker.SetActive(false);
        }
    }
}