using UnityEngine;

public class TestRotateAround : MonoBehaviour
{
    public Transform centerObject;
    
    void Update()
    {
        if (centerObject != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log($"绕Y轴旋转45度前: 位置={transform.position}");
                transform.RotateAround(centerObject.position, Vector3.up, 45f);
                Debug.Log($"绕Y轴旋转45度后: 位置={transform.position}");
            }
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log($"绕X轴旋转45度前: 位置={transform.position}");
                transform.RotateAround(centerObject.position, Vector3.right, 45f);
                Debug.Log($"绕X轴旋转45度后: 位置={transform.position}");
            }
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log($"绕Z轴旋转45度前: 位置={transform.position}");
                transform.RotateAround(centerObject.position, Vector3.forward, 45f);
                Debug.Log($"绕Z轴旋转45度后: 位置={transform.position}");
            }
        }
    }
}