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
    /// <summary>
    /// 初始化标识
    /// </summary>
    private bool _initialized = false;
    private async void Start()
    {
        // 初始化服务定位器
        await GameServiceLocator.WaitForInitialization();
        _initialized = true;

        // 事件中心管理器测试
        GameServiceLocator.Event.AddListener("GameStarted", () =>
        {
            Debug.LogWarning("准备好冰块旋转了吗");
        });
        HuntingGame.Instance.StartGame();

        // 配置管理器测试
        Debug.Log(GameServiceLocator.Config.GetBullet(1));

        // 资源加载管理器测试
        Debug.Log(await GameServiceLocator.Resource.LoadAssetAsync<GameObject>("Arts/Prefabs/UI/UIMeatProgress"));

        // UI管理器测试
        //UIMeatProgress uIMeatProgress = await GameServiceLocator.UI.OpenUIAsync<UIMeatProgress>("UIMeatProgress");

        // 网络管理器测试
        GameServiceLocator.GetFrameworkManager<HttpManager>().SetDefaultHeader("username", "zhangsan");
    }

    // 对象池管理器测试用队列
    Queue<GameObject> animals = new Queue<GameObject>();
    private async void Update()
    {
        if (!_initialized) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"[GameServiceLocatorTest] GameObjectPoolManager PullAsync");
            GameObject obj = await GameServiceLocator.Pool.PullAsync("Animals/Animal_Large_301");
            animals.Enqueue(obj);
            obj.GetComponent<AnimalBehavior>().Init(GameServiceLocator.Config.GetSpecie(301),10,transform.forward);

            Bullet bullet = GameServiceLocator.Config.GetBullet(1);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log($"[GameServiceLocatorTest] GameObjectPoolManager Push");
            GameServiceLocator.Pool.Push(animals.Dequeue());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log($"[GameServiceLocatorTest] GameObjectPoolManager ClearPool");
            GameServiceLocator.Pool.ClearPool("Animals/Animal_Large_301");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log($"[GameServiceLocatorTest] GameObjectPoolManager ClearAllPools");
            GameServiceLocator.Pool.ClearAllPools();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[GameServiceLocatorTest] GameObjectPoolManager DestroyAll");
            GameServiceLocator.Pool.DestroyAll();
            animals.Clear();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log($"[GameServiceLocatorTest] GameObjectPoolManager Release");
            GameServiceLocator.Pool.Release();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach (var obj in animals) 
            {
                obj.GetComponent<AnimalBehavior>().PauseMovement();
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (var obj in animals)
            {
                obj.GetComponent<AnimalBehavior>().ResumeMovement();
            }
        }

    }
}
