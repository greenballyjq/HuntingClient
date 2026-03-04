using UnityEngine;

public class XAxisTiltBillboard : MonoBehaviour
{
    private CameraManager _cameraManager;

    void Awake()
    {
        _cameraManager = GameServiceLocator.GetAppManager<CameraManager>();
    }

    void LateUpdate()
    {
        Vector3 f = _cameraManager.MainCamera.transform.forward;
        float angle = Mathf.Asin(Mathf.Abs(f.y)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(angle, 0f, 0f);
    }
}