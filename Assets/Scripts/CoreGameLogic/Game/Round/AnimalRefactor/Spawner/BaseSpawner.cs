using UnityEngine;

namespace Hunting.Game.Animal
{
    /// <summary>
    /// 派发区域类型
    /// </summary>
    public enum SpawnAreaType
    {
        /// <summary>
        /// 点
        /// </summary>
        Point,
        /// <summary>
        /// 线
        /// </summary>
        Line,
        /// <summary>
        /// 矩形
        /// </summary>
        Rect
    }

    /// <summary>
    /// 派发信息
    /// </summary>
    public struct SpawnInfo
    {
        /// <summary>
        /// 生成位置
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// 移动方向
        /// </summary>
        public Vector3 Direction;
        
        public SpawnInfo(Vector3 pos, Vector3 dir)
        {
            Position = pos;
            Direction = dir.normalized;
        }
    }

    /// <summary>
    /// 派发器基类
    /// </summary>
    public abstract class BaseSpawner : MonoBehaviour
    {
        /// <summary>
        /// 派发器标签
        /// </summary>
        [SerializeField] protected string spawnerTag;
        public string Tag => spawnerTag;

        /// <summary>
        /// 派发区域类型
        /// </summary>
        [SerializeField] protected SpawnAreaType areaType = SpawnAreaType.Point;

        /// <summary>
        /// 线半长
        /// </summary>
        [SerializeField] protected float lineHalfLength;

        /// <summary>
        /// 矩形半尺寸
        /// </summary>
        [SerializeField] protected Vector2 rectHalfSize;

        /// <summary>
        /// 最小派发角度
        /// </summary>
        [SerializeField] protected float minSpawnAngle;

        /// <summary>
        /// 最大派发角度
        /// </summary>
        [SerializeField] protected float maxSpawnAngle;
        
        #region 私有方法
        /// <summary>
        /// 计算派发信息
        /// </summary>
        /// <returns>派发信息</returns>
        protected virtual SpawnInfo CalculateSpawnInfo()
        {
            Vector3 position = CalculatePosition();
            Vector3 direction = CalculateDirection();
            return new SpawnInfo(position, direction);
        }

        /// <summary>
        /// 计算生成位置
        /// </summary>
        /// <returns>生成位置</returns>
        protected Vector3 CalculatePosition()
        {
            Vector3 basePos = transform.position;

            switch (areaType)
            {
                case SpawnAreaType.Point:
                    return basePos;

                case SpawnAreaType.Line:
                    float offset = Random.Range(-lineHalfLength, lineHalfLength);
                    return basePos + transform.rotation * new Vector3(0f, 0f, offset);

                case SpawnAreaType.Rect:
                    float x = Random.Range(-rectHalfSize.x, rectHalfSize.x);
                    float z = Random.Range(-rectHalfSize.y, rectHalfSize.y);
                    return basePos + transform.rotation * new Vector3(x, 0f, z);
                default:
                    return basePos;
            }
        }

        /// <summary>
        /// 计算派发方向
        /// </summary>
        /// <returns></returns>
        protected Vector3 CalculateDirection()
        {
            float angle = Random.Range(minSpawnAngle, maxSpawnAngle);
            return Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
        }
        #endregion

        #region Gizmos可视化
        private void OnDrawGizmos()
        {
            DrawSpawnArea();
            DrawAngleRange();
        }

        /// <summary>
        /// 绘制派发区域
        /// </summary>
        private void DrawSpawnArea()
        {
            Gizmos.color = Color.blue;
            Vector3 origin = transform.position;

            switch (areaType)
            {
                case SpawnAreaType.Point:
                    Gizmos.DrawSphere(origin, 0.2f);
                    break;

                case SpawnAreaType.Line:
                    Vector3 start = origin + transform.rotation * new Vector3(0f, 0f, -lineHalfLength);
                    Vector3 end = origin + transform.rotation * new Vector3(0f, 0f, lineHalfLength);
                    Gizmos.DrawLine(start, end);
                    Gizmos.DrawSphere(start, 0.15f);
                    Gizmos.DrawSphere(end, 0.15f);
                    Gizmos.DrawSphere(origin, 0.1f);
                    break;

                case SpawnAreaType.Rect:
                    Vector3[] corners = new Vector3[4];
                    corners[0] = origin + transform.rotation * new Vector3(-rectHalfSize.x, 0f, -rectHalfSize.y);
                    corners[1] = origin + transform.rotation * new Vector3(rectHalfSize.x, 0f, -rectHalfSize.y);
                    corners[2] = origin + transform.rotation * new Vector3(rectHalfSize.x, 0f, rectHalfSize.y);
                    corners[3] = origin + transform.rotation * new Vector3(-rectHalfSize.x, 0f, rectHalfSize.y);

                    Gizmos.DrawLine(corners[0], corners[1]);
                    Gizmos.DrawLine(corners[1], corners[2]);
                    Gizmos.DrawLine(corners[2], corners[3]);
                    Gizmos.DrawLine(corners[3], corners[0]);
                    Gizmos.DrawSphere(origin, 0.1f);
                    break;
            }
        }

        /// <summary>
        /// 绘制角度范围
        /// </summary>
        private void DrawAngleRange()
        {
            Gizmos.color = Color.red;
            Vector3 origin = transform.position;
            Vector3 forward = transform.forward;
            float arcLength = 3f;

            Vector3 minDir = Quaternion.AngleAxis(minSpawnAngle, Vector3.up) * forward;
            Vector3 maxDir = Quaternion.AngleAxis(maxSpawnAngle, Vector3.up) * forward;

            Gizmos.DrawLine(origin, origin + minDir * arcLength);
            Gizmos.DrawLine(origin, origin + maxDir * arcLength);

            int segments = 20;
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                float angle = Mathf.Lerp(minSpawnAngle, maxSpawnAngle, t);
                Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;
                if (i > 0)
                {
                    float prevAngle = Mathf.Lerp(minSpawnAngle, maxSpawnAngle, (float)(i - 1) / segments);
                    Vector3 prevDir = Quaternion.AngleAxis(prevAngle, Vector3.up) * forward;
                    Gizmos.DrawLine(origin + prevDir * arcLength, origin + dir * arcLength);
                }
            }
        }
        #endregion
    }
}
