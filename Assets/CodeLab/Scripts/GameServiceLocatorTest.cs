using System.Collections;
using System.Collections.Generic;
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
    private bool _initialized = false;
    private async void Start()
    {
        await GameServiceLocator.WaitForInitialization();
        _initialized = true;

        GameServiceLocator.Event.AddListener("GameStarted", () =>
        {
            Debug.LogWarning("准备好冰块旋转了吗");
        });
        HuntingGame.Instance.StartGame();

        Debug.Log(GameServiceLocator.Config.GetBullet(1));

        Debug.Log(await GameServiceLocator.Resource.LoadAssetAsync<GameObject>("Arts/Prefabs/UI/UIMeatProgress"));

        UIMeatProgress uIMeatProgress = await GameServiceLocator.UI.OpenUIAsync<UIMeatProgress>("UIMeatProgress");

        GameServiceLocator.GetFrameworkManager<HttpManager>().SetDefaultHeader("username", "zhangsan");
    }

    Queue<GameObject> animals = new Queue<GameObject>();
    private async void Update()
    {
        if (!_initialized) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = await GameServiceLocator.Pool.PullAsync("Animals/Animal_Large_301");
            animals.Enqueue(obj);
            obj.GetComponent<AnimalBehavior>().Init(GameServiceLocator.Config.GetSpecie(301),10,transform.forward);
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameServiceLocator.Pool.Push(animals.Dequeue());
        }
    }
}
