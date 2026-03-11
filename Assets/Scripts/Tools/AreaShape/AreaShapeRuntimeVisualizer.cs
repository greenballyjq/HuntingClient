using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区域形状运行时可视化
/// 编辑模式与Play模式均可显示
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class AreaShapeRuntimeVisualizer : MonoBehaviour
{
    /// <summary>
    /// 是否显示
    /// </summary>
    [SerializeField] private bool _show = true;

    /// <summary>
    /// 线宽
    /// </summary>
    [SerializeField] private float _lineWidth = 0.25f;

    /// <summary>
    /// 线条渲染组件
    /// </summary>
    private LineRenderer _lineRenderer;

    /// <summary>
    /// 区域形状
    /// </summary>
    private BaseAreaShape _shape;

    /// <summary>
    /// 轮廓顶点缓冲区
    /// </summary>
    private readonly List<Vector3> _pointBuffer = new List<Vector3>(64);

    private void Awake()
    {
        CacheComponents();
    }

    private void OnEnable()
    {
        CacheComponents();
    }

    private void LateUpdate()
    {
        if (!_show)
        {
            _lineRenderer.enabled = false;
            return;
        }

        _pointBuffer.Clear();
        _shape.GetOutlinePoints(_pointBuffer);

        if (_pointBuffer.Count < 2)
        {
            _lineRenderer.enabled = false;
            return;
        }

        _lineRenderer.enabled = true;
        _lineRenderer.startColor = _shape.BorderColor;
        _lineRenderer.endColor = _shape.BorderColor;
        _lineRenderer.positionCount = _pointBuffer.Count;

        for (int i = 0; i < _pointBuffer.Count; i++)
            _lineRenderer.SetPosition(i, _pointBuffer[i]);
    }

    #region 私有方法
    /// <summary>
    /// 缓存组件
    /// </summary>
    private void CacheComponents()
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();

        if (_shape == null)
            _shape = GetComponent<BaseAreaShape>();

        if (_lineRenderer != null)
            SetupLineRenderer();
    }

    /// <summary>
    /// 设置线条渲染组件基础参数
    /// </summary>
    private void SetupLineRenderer()
    {
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.loop = true;
        _lineRenderer.startWidth = _lineWidth;
        _lineRenderer.endWidth = _lineWidth;

        if (_lineRenderer.sharedMaterial == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            _lineRenderer.sharedMaterial = new Material(shader);
        }
    }
    #endregion
}
