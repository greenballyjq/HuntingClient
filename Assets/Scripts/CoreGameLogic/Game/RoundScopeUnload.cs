using UnityEngine.SceneManagement;

/// <summary>
/// 局内场景路径与准备/启动场景判定。运行时卸载由 RoundFlow 执行。
/// </summary>
public static class RoundScopeUnload
{
    public const string ForestScene = "Assets/Scenes/GameplayForestScene.unity";
    public const string SnowScene = "Assets/Scenes/GameplaySnowMountainScene.unity";
    public const string PrepareScene = "Assets/Scenes/PrepareScene.unity";
    public const string BootstrapScene = "Assets/Scenes/BootstrapScene.unity";
    public const string SnowEffect = "Assets/Arts/Prefabs/Effects/FX_Snow_For_SnowMountainScene_UICamera";

    public static bool IsPrepareOrBootstrapActive()
    {
        string name = SceneManager.GetActiveScene().name;
        return name == System.IO.Path.GetFileNameWithoutExtension(PrepareScene)
            || name == System.IO.Path.GetFileNameWithoutExtension(BootstrapScene);
    }
}
