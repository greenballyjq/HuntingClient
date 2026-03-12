using GameFramework.Game;
using GameFramework.Manager;
using GameFramework.Utility;
using UnityEngine;

/// <summary>
/// 相机管理器
/// </summary>
public class CameraManager : IAppManager
{
    /// <summary>
    /// 主摄像机
    /// </summary>
    private Camera _mainCamera => Camera.main;
    public Camera MainCamera => _mainCamera;

    /// <summary>
    /// 粒子摄像机
    /// </summary>
    private Camera _particleCamera;
    public Camera ParticleCamera => _particleCamera;

    /// <summary>
    /// UI摄像机
    /// </summary>
    public Camera _uiCamera;
    public Camera UICamera => _uiCamera;

    private UIManager _uiManager;


    public void Init()
    {
        RegisterServices();

        _particleCamera = GameObject.FindGameObjectWithTag("Particle Camera").GetComponent<Camera>();
        Object.DontDestroyOnLoad(_particleCamera.gameObject);

        _uiCamera = _uiManager.UICamera;

        Log.Info("[CameraManager] 初始化完成");
    }

    public void Dispose()
    {
        _particleCamera = null;
        Log.Info("[CameraManager] 已释放");
    }

    #region 私有方法
    private void RegisterServices()
    {
        _uiManager = GameServiceLocator.UIManager;
    }
    #endregion

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

