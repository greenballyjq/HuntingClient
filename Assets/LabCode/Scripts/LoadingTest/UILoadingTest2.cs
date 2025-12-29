using UnityEngine;

/// <summary>
/// 测试加载界面
/// </summary>
public class UILoadingTest2 : TestUIBase
{
    /// <summary>
    /// 加载进度条组件
    /// </summary>
    [SerializeField] private UIComponentLoadingProgress _uiComponentLoadingProgress;

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        _uiComponentLoadingProgress.Init();
    }

    public override void OnClose()
    {
        _uiComponentLoadingProgress.CleanUp();

        base.OnClose();
    }

    #region 公共方法
    /// <summary>
    /// 设置加载进度
    /// </summary>
    /// <param name="progress">进度值（0-1）</param>
    public void SetProgress(float progress)
    {
        _uiComponentLoadingProgress.SetProgress(progress);
    }
    #endregion
}

