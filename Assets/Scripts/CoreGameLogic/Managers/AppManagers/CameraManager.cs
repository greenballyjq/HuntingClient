using GameFramework.Game;
using UnityEngine;

/// <summary>
/// 相机管理器
/// </summary>
public class CameraManager : IAppManager
{
    /// <summary>
    /// 主摄像机
    /// </summary>
    public Camera MainCamera => Camera.main;

    /// <summary>
    /// 粒子摄像机
    /// </summary>
    private Camera _particleCamera;
    public Camera ParticleCamera => _particleCamera;

    /// <summary>
    /// UI摄像机
    /// </summary>
    public Camera UICamera => GameServiceLocator.UIManager.UICamera;

    public void Init()
    {
        _particleCamera = GameObject.FindGameObjectWithTag("Particle Camera").GetComponent<Camera>();
        Object.DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Particle Camera"));

        Debug.Log("[CameraManager] 初始化完成");
    }

    public void Dispose()
    {
        _particleCamera = null;
        Debug.Log("[CameraManager] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 将世界坐标转换为屏幕坐标
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <returns>屏幕坐标</returns>
    public Vector3 WorldToScreenPoint(Vector3 worldPosition)
    {
        return MainCamera.WorldToScreenPoint(worldPosition);
    }

    /// <summary>
    /// 将屏幕坐标转换为世界坐标
    /// </summary>
    /// <param name="screenPosition">屏幕坐标</param>
    /// <param name="depth">世界空间深度</param>
    /// <returns>世界坐标</returns>
    public Vector3 ScreenToWorldPoint(Vector2 screenPosition, float depth = 1f)
    {
        return MainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
    }
    #endregion
}

