using UnityEngine;

/// <summary>
/// 默认射击控制处理器
/// </summary>
public class DefaultShootingControlHandler : BaseControlHandler
{
    private const float AIM_PLANE_HEIGHT = 0f;

    protected override void OnInit() {}

    protected override void OnControlStart()
    {
        InputManager.SwitchToFireMode();
    }

    protected override void OnControlUpdate(float dt)
    {
        if (InputManager.IsFireHeld)
        {
            Ray ray = CameraManager.MainCamera.ScreenPointToRay(InputManager.PointerScreenPosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0f, AIM_PLANE_HEIGHT, 0f));
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 worldPos = ray.GetPoint(enter);
                PlayerWeapon.SetAimTarget(worldPos);
            }
            PlayerWeapon.TryFire();
        }
    }

    protected override void OnControlEnd() { }
}