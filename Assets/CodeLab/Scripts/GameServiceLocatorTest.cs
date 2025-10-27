using Hunting;
using Hunting.Game;
using Hunting.UI;
using UnityEngine;

/// <summary>
/// 服务定位器测试
/// </summary>
public class GameServiceLocatorTest : MonoBehaviour
{
    private async void Start()
    {
        await GameServiceLocator.WaitForInitialization();

        GameServiceLocator.Events.AddListener("GameStarted", () =>
        {
            Debug.LogWarning("准备好冰块旋转了吗");
        });
        HuntingGame.Instance.StartGame();

        Debug.Log(GameServiceLocator.Config.GetBullet(1));

        Debug.Log(await GameServiceLocator.Resources.LoadAssetAsync<GameObject>("Arts/Prefabs/UI/UIMeatProgress"));

        UIMeatProgress uIMeatProgress = await GameServiceLocator.UI.OpenUIAsync<UIMeatProgress>("UIMeatProgress");

        GameServiceLocator.GetFrameworkManager<HttpManager>().SetDefaultHeader("username", "zhangsan");
    }
}
