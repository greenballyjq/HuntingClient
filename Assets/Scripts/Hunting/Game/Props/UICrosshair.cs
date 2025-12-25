using Hunting.Game.Weapons;
using Hunting.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.Game.Props
{
    /// <summary>
    /// 指哪打哪瞄准镜视觉组件
    /// </summary>
    public class UICrosshair : MonoBehaviour
    {
        /// <summary>
        /// 默认大小
        /// </summary>
        [SerializeField] private Vector2 _defaultSize = new Vector2(100, 100);

        /// <summary>
        /// 跟随速度
        /// </summary>
        [SerializeField] private float _followSpeed = 10f;

        /// <summary>
        /// 最小大小比例
        /// </summary>
        [SerializeField] private float _minScaleRatio = 0.5f;

        /// <summary>
        /// 最小透明度
        /// </summary>
        [SerializeField] private float _minAlpha = 0.3f;

        /// <summary>
        /// 瞄准镜图片
        /// </summary>
        private Image _crosshairImage;

        /// <summary>
        /// 瞄准镜RectTransform
        /// </summary>
        private RectTransform _rectTransform;

        /// <summary>
        /// 最小锁定距离
        /// </summary>
        private float _minLockDistance;

        /// <summary>
        /// 最大锁定距离
        /// </summary>
        private float _maxLockDistance;

        /// <summary>
        /// 目标大小比例
        /// </summary>
        private float _targetScaleRatio;

        /// <summary>
        /// 目标透明度
        /// </summary>
        private float _targetAlpha;

        /// <summary>
        /// 目标位置
        /// </summary>
        private Vector2 _targetPosition;

        /// <summary>
        /// 当前颜色
        /// </summary>
        private Color _currentColor;

        /// <summary>
        /// 输入管理器
        /// </summary>
        private InputManager Input => GameServiceLocator.GetAppManager<InputManager>();

        /// <summary>
        /// 武器管理器
        /// </summary>
        private WeaponManager Weapon => GameServiceLocator.GetAppManager<WeaponManager>();

        private void Awake()
        {
            _crosshairImage = GetComponentInChildren<Image>();
            _rectTransform = _crosshairImage.GetComponent<RectTransform>();

            _currentColor = _crosshairImage.color;

            _rectTransform.sizeDelta = _defaultSize;
        }

        private void Update()
        {
            // 平滑移动到目标位置
            Vector2 currentPos = _rectTransform.position;
            _rectTransform.position = Vector2.Lerp(currentPos, _targetPosition, Time.deltaTime * _followSpeed);

            // 平滑调整大小
            float currentScaleRatio = _rectTransform.sizeDelta.x / _defaultSize.x;
            float newScaleRatio = Mathf.Lerp(currentScaleRatio, _targetScaleRatio, Time.deltaTime * _followSpeed);
            _rectTransform.sizeDelta = _defaultSize * newScaleRatio;

            // 平滑调整透明度
            float currentAlpha = _currentColor.a;
            float newAlpha = Mathf.Lerp(currentAlpha, _targetAlpha, Time.deltaTime * _followSpeed);
            _currentColor.a = newAlpha;
            _crosshairImage.color = _currentColor;
        }

        #region 公共方法
        /// <summary>
        /// 设置锁定距离范围
        /// </summary>
        /// <param name="minDistance">最小锁定距离</param>
        /// <param name="maxDistance">最大锁定距离</param>
        public void SetDistanceRange(float minDistance, float maxDistance)
        {
            _minLockDistance = minDistance;
            _maxLockDistance = maxDistance;
        }

        /// <summary>
        /// 根据目标更新瞄准镜显示
        /// </summary>
        /// <param name="target">目标</param>
        public void UpdateByTarget(Transform target)
        {
            // 无目标时居中
            if (target == null)
            {
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                _targetPosition = screenCenter;
                _targetScaleRatio = 1f;
                _targetAlpha = _minAlpha;
                return;
            }

            // 获取武器位置 TODO: 待调整
            var weapon = Weapon.GetMainWeapon();
            if (weapon == null)
                return;

            // 获取目标的碰撞器中心点
            Collider targetCollider = target.GetComponent<Collider>();
            Vector3 worldPosition;
            if (targetCollider != null)
                worldPosition = targetCollider.bounds.center;
            else
                worldPosition = target.position;
                
            // 计算目标屏幕位置
            Vector3 screenPosition = Input.WorldToScreen(worldPosition);
            _targetPosition = new Vector2(screenPosition.x, screenPosition.y);

            // 计算距离
            float distance = Vector3.Distance(weapon.transform.position, worldPosition);

            // 计算距离比例
            float distanceRange = _maxLockDistance - _minLockDistance;
            if (distanceRange <= 0f)
            {
                _targetScaleRatio = 1f;
                _targetAlpha = 1f;
                return;
            }
            float distanceRatio = Mathf.Clamp01(1f - (distance - _minLockDistance) / distanceRange);

            // 计算大小比例
            _targetScaleRatio = Mathf.Lerp(_minScaleRatio, 1f, distanceRatio);

            // 计算透明度
            _targetAlpha = Mathf.Lerp(_minAlpha, 1f, distanceRatio);
        }
        #endregion
    }
}
