using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float speed = 3f;

    void Update()
    {
        // 一直往前走
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}