using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 加载进度条组件
/// </summary>
public class UIComponentLoadingProgress : MonoBehaviour, ITestUIComponent
{
    /// <summary>
    /// 进度条填充图片
    /// </summary>
    [SerializeField] private Image _imageProgressFill;

    /// <summary>
    /// 小人图片的RectTransform
    /// </summary>
    [SerializeField] private RectTransform _rectTransformCharacter;

    /// <summary>
    /// 进度条的RectTransform（用于计算宽度）
    /// </summary>
    [SerializeField] private RectTransform _rectTransformProgressBar;

    /// <summary>
    /// 当前进度（0-1）
    /// </summary>
    private float _currentProgress = 0f;

    public void Init()
    {
        _currentProgress = 0f;
        SetProgress(0f);
    }

    public void CleanUp()
    {
        // 清理逻辑（如有需要）
    }

    #region 私有方法
    /// <summary>
    /// 设置进度
    /// </summary>
    /// <param name="progress">进度值（0-1）</param>
    public void SetProgress(float progress)
    {
        _currentProgress = Mathf.Clamp01(progress);

        _imageProgressFill.fillAmount = _currentProgress;

        // 更新小人位置
        UpdateCharacterPosition();
    }

    /// <summary>
    /// 更新小人位置
    /// </summary>
    private void UpdateCharacterPosition()
    {
        // 计算进度条宽度
        float progressBarWidth = _rectTransformProgressBar.rect.width;

        // 计算进度条的左边位置（考虑pivot）
        float progressBarLeft = -progressBarWidth * _rectTransformProgressBar.pivot.x;

        // 计算小人X位置：左边位置 + 进度 * 进度条宽度
        float characterX = progressBarLeft + _currentProgress * progressBarWidth;

        // 保持小人Y位置不变，只更新X
        Vector3 localPos = _rectTransformCharacter.localPosition;
        localPos.x = characterX;
        _rectTransformCharacter.localPosition = localPos;
    }
    #endregion
}

