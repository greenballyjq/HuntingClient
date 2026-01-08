using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 测试加载界面
/// </summary>
public class UILoadingTest2 : TestUIBase
{
    /// <summary>
    /// 加载进度条组件
    /// </summary>
    [SerializeField] private UIComponentLoadingProgress _uiComponentLoadingProgress;

    /// <summary>
    /// 状态文本组件
    /// </summary>
    [SerializeField] private Text _textStatus;

    /// <summary>
    /// CanvasGroup组件（用于控制透明度）
    /// </summary>
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        _uiComponentLoadingProgress.Init();
        
        _canvasGroup.alpha = 0f;
        StartCoroutine(FadeIn());
    }

    public override void OnClose()
    {
        StartCoroutine(WaitProgressAndFadeOut());
    }

    /// <summary>
    /// 等待进度插值到1.0后淡出并销毁
    /// </summary>
    private IEnumerator WaitProgressAndFadeOut()
    {
        while (_uiComponentLoadingProgress.GetCurrentProgress() < 0.999f)
        {
            yield return null;
        }

        _uiComponentLoadingProgress.CleanUp();
        yield return StartCoroutine(FadeOut());
        
        base.OnClose();
        
        TestUIManager.Instance.ForceCloseUI(gameObject.name);
    }

    protected override void OnShow()
    {
    }

    protected override void OnHide()
    {
        StartCoroutine(FadeOut());
    }

    #region 私有方法
    /// <summary>
    /// 淡入动画
    /// </summary>
    private IEnumerator FadeIn()
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 淡出动画
    /// </summary>
    private IEnumerator FadeOut()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 设置加载进度
    /// </summary>
    /// <param name="progress">进度值（0-1）</param>
    public void SetProgress(float progress)
    {
        _uiComponentLoadingProgress.SetProgress(progress);
    }

    /// <summary>
    /// 设置状态文本
    /// </summary>
    /// <param name="status">状态文本</param>
    public void SetStatusText(string status)
    {
        if (_textStatus != null)
        {
            _textStatus.text = status;
        }
    }
    #endregion
}

