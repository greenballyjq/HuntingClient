using System.Collections;
using System.Collections.Generic;
using cfg.HuntingConfig;
using Hunting;
using Hunting.Game;
using Hunting.Game.Animal;
using Hunting.UI;
using UnityEngine;

/// <summary>
/// 服务定位器测试
/// </summary>
public class GameServiceLocatorTest : MonoBehaviour
{
    public GameObject cube;
    private void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            cube.SetActive(true);
            cube.GetComponent<CubeTest>().Init();
        }
    }
}
