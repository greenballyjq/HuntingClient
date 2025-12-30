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
    /// 小人图片组件（用于显示序列帧）
    /// </summary>
    [SerializeField] private Image _imageCharacter;

    /// <summary>
    /// 进度条的RectTransform（用于计算宽度）
    /// </summary>
    [SerializeField] private RectTransform _rectTransformProgressBar;

    /// <summary>
    /// Loading文字组件
    /// </summary>
    [SerializeField] private Text _textLoading;

    /// <summary>
    /// 角色序列帧精灵数组
    /// </summary>
    [SerializeField] private Sprite[] _characterSprites;

    /// <summary>
    /// 动画帧率（每秒播放帧数）
    /// </summary>
    [SerializeField] private float _animationFrameRate = 10f;

    /// <summary>
    /// Loading文字更新间隔（秒）
    /// </summary>
    [SerializeField] private float _loadingTextInterval = 0.5f;

    /// <summary>
    /// 当前进度（0-1）
    /// </summary>
    private float _currentProgress = 0f;

    /// <summary>
    /// 目标进度（0-1）
    /// </summary>
    private float _targetProgress = 0f;

    /// <summary>
    /// 当前动画帧索引
    /// </summary>
    private int _currentFrameIndex = 0;

    /// <summary>
    /// 动画累计时间
    /// </summary>
    private float _animationTimer = 0f;

    /// <summary>
    /// Loading文字计时器
    /// </summary>
    private float _loadingTextTimer = 0f;

    /// <summary>
    /// Loading文字点数量（0-3）
    /// </summary>
    private int _loadingDotCount = 0;

    public void Init()
    {
        _currentProgress = 0f;
        _targetProgress = 0f;
        _imageProgressFill.fillAmount = 0f;
        _currentFrameIndex = 0;
        _animationTimer = 0f;
        _loadingTextTimer = 0f;
        _loadingDotCount = 0;
        UpdateCharacterPosition();
        UpdateCharacterSprite();
        UpdateLoadingText();
    }

    public void CleanUp()
    {
        // 清理逻辑（如有需要）
    }

    private void Update()
    {
        if (Mathf.Abs(_currentProgress - _targetProgress) > 0.001f)
        {
            _currentProgress = Mathf.Lerp(_currentProgress, _targetProgress, Time.deltaTime * 5f);
            _imageProgressFill.fillAmount = _currentProgress;
            UpdateCharacterPosition();
        }

        UpdateCharacterAnimation();
        UpdateLoadingText();
    }

    #region 私有方法
    /// <summary>
    /// 更新Loading文字
    /// </summary>
    private void UpdateLoadingText()
    {
        if (_textLoading == null)
            return;

        _loadingTextTimer += Time.deltaTime;

        if (_loadingTextTimer >= _loadingTextInterval)
        {
            _loadingTextTimer = 0f;
            _loadingDotCount = (_loadingDotCount + 1) % 4;
            
            string dots = "";
            for (int i = 0; i < _loadingDotCount; i++)
            {
                dots += ".";
            }
            
            _textLoading.text = "Loading" + dots;
        }
    }
    /// <summary>
    /// 更新角色序列帧动画
    /// </summary>
    private void UpdateCharacterAnimation()
    {
        if (_characterSprites == null || _characterSprites.Length == 0)
            return;

        _animationTimer += Time.deltaTime;
        float frameInterval = 1f / _animationFrameRate;

        if (_animationTimer >= frameInterval)
        {
            _animationTimer = 0f;
            _currentFrameIndex = (_currentFrameIndex + 1) % _characterSprites.Length;
            UpdateCharacterSprite();
        }
    }

    /// <summary>
    /// 更新角色精灵
    /// </summary>
    private void UpdateCharacterSprite()
    {
        if (_imageCharacter != null && _characterSprites != null && _characterSprites.Length > 0)
        {
            _imageCharacter.sprite = _characterSprites[_currentFrameIndex];
        }
    }

    /// <summary>
    /// 设置进度
    /// </summary>
    /// <param name="progress">进度值（0-1）</param>
    public void SetProgress(float progress)
    {
        _targetProgress = Mathf.Clamp01(progress);
    }

    /// <summary>
    /// 获取当前进度
    /// </summary>
    public float GetCurrentProgress()
    {
        return _currentProgress;
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

