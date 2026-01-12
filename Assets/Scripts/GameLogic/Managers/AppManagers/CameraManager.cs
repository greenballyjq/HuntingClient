using GameFramework.Game;
using UnityEngine;

/// <summary>
/// 相机管理器
/// </summary>
public class CameraManager : IAppManager
{
    /// <summary>
    /// 粒子摄像机
    /// </summary>
    private Camera _particleCamera;

    /// <summary>
    /// 粒子摄像机
    /// </summary>
    public Camera ParticleCamera => _particleCamera;

    public void Init()
    {
        var particleCameraObject = GameObject.FindGameObjectWithTag("Particle Camera");
        _particleCamera = particleCameraObject.GetComponent<Camera>();
        Object.DontDestroyOnLoad(particleCameraObject);

        Debug.Log("[CameraManager] 初始化完成");
    }

    public void Dispose()
    {
        _particleCamera = null;
        Debug.Log("[CameraManager] 已释放");
    }
}

