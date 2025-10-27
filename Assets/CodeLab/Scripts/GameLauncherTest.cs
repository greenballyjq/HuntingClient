using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLauncherTest : MonoBehaviour
{
    private bool isInitialized;


    // Start is called before the first frame update
    void Start()
    {
        //TestInitialization().Forget();
    }

    private async UniTaskVoid TestInitialization()
    {
        await UniTask.WaitUntil(() => GameFrameworkManager.Instance.IsInitialized);

        // 框架已初始化，可以安全使用
        GameFrameworkManager.Instance.GetManager<EventManager>().AddListener("Funky", () =>
        {
            Debug.Log("Funky");
        });

        GameFrameworkManager.Instance.GetManager<EventManager>().Trigger("Funky");
    }



    // Update is called once per frame
    void Update()
    {
        if (!isInitialized && GameFrameworkManager.Instance.IsInitialized)
        {
            isInitialized = true;
            GameFrameworkManager.Instance.GetManager<EventManager>().AddListener("Funky", () =>
            {
                Debug.LogWarning("Funky");
            });

            GameFrameworkManager.Instance.GetManager<EventManager>().Trigger("Funky");

        }
    }
}
