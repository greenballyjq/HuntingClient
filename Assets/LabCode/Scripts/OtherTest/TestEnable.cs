using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnable : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Hello World");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Time
        //Random
        //Mathf
        //Camera
        //IEnumerator

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnEnable()
    {
        Debug.Log("你好");
    }
}
