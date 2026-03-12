using UnityEngine;

/// <summary>
/// 屏幕可见性工具
/// </summary>
public static class ScreenUtils
{
    /// <summary>
    /// 判断bounds是否在相机视野内
    /// </summary>
    public static bool IsVisible(Bounds bounds, Camera camera = null)
    {
        return TestBoundsAgainstFrustum(bounds, camera);
    }

    /// <summary>
    /// 判断物体是否在相机视野内
    /// </summary>
    public static bool IsVisible(Transform transform, Camera camera = null)
    {
        var renderer = transform.GetComponent<Renderer>();
        if (renderer != null)
            return TestBoundsAgainstFrustum(renderer.bounds, camera);

        var collider = transform.GetComponent<Collider>();
        if (collider != null)
            return TestBoundsAgainstFrustum(collider.bounds, camera);

        return TestBoundsAgainstFrustum(new Bounds(transform.position, Vector3.zero), camera);
    }

    private static bool TestBoundsAgainstFrustum(Bounds bounds, Camera camera)
    {
        camera ??= Camera.main;
        var planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
}
