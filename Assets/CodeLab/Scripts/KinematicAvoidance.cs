using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicAvoidance : MonoBehaviour
{
    public float speed = 3f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 碰撞时随机转向
        transform.Rotate(0, Random.Range(90, 270), 0);
    }
}
