using UnityEngine;

public class XAxisTiltBillboard : MonoBehaviour
{
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        // 相机前方向
        Vector3 f = cam.transform.forward;

        // 用相机俯角直接算一个“躺的角度”
        // forward.y ∈ [-1,1] → asin ∈ [0,90]
        float angle = Mathf.Asin(Mathf.Abs(f.y)) * Mathf.Rad2Deg;

        // 只绕 X 轴
        transform.rotation = Quaternion.Euler(angle, 0f, 0f);
    }
}
