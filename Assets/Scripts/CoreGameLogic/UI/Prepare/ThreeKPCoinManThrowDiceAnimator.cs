using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 三千盘金币人投骰子动画器组件
/// </summary>
public class ThreeKPCoinManThrowDiceAnimator : MonoBehaviour
{
    /// <summary>
    /// 动画器组件
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// 骰子预制体
    /// </summary>
    [SerializeField] private GameObject _dicePrefab;

    /// <summary>
    /// 投掷起始位置
    /// </summary>
    [SerializeField] private Transform _diceThrowStartPosition;

    /// <summary>
    /// 移动速度
    /// </summary>
    [SerializeField] private float _moveSpeed = 5f;

    /// <summary>
    /// Y轴移动偏移
    /// </summary>
    [SerializeField] private float _yMoveOffset = 0f;

    /// <summary>
    /// 矩形变换组件缓存
    /// </summary>
    private RectTransform _rectTransform;

    /// <summary>
    /// 骰子投出等待源
    /// </summary>
    private UniTaskCompletionSource<DiceRollAnimator> _diceThrowedCompletionSource;

    /// <summary>
    /// 投骰子动画结束等待源
    /// </summary>
    private UniTaskCompletionSource<bool> _throwAnimationEndedCompletionSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rectTransform = GetComponent<RectTransform>();
    }

    #region 公共方法
    /// <summary>
    /// 播放投骰子动画
    /// </summary>
    /// <returns>创建的骰子动画器组件</returns>
    public async UniTask<DiceRollAnimator> PlayThrowDiceAsync()
    {
        _diceThrowedCompletionSource = new UniTaskCompletionSource<DiceRollAnimator>();
        _throwAnimationEndedCompletionSource = new UniTaskCompletionSource<bool>();

        PlayRoll();

        return await _diceThrowedCompletionSource.Task;
    }

    /// <summary>
    /// 等待投骰子动画结束
    /// </summary>
    public async UniTask WaitForThrowAnimationEndAsync()
    {
        await _throwAnimationEndedCompletionSource.Task;
    }

    /// <summary>
    /// 播放走格子动画
    /// </summary>
    /// <param name="targetPoints">目标点序列</param>
    /// <param name="directions">朝向序列</param>
    public async UniTask PlayWalkSlot(Vector3[] targetPoints, bool[] directions)
    {
        PlayWalk();

        for (int i = 0; i < targetPoints.Length; i++)
        {
            // 计算目标位置（应用Y轴偏移）
            Vector3 targetPosition = targetPoints[i];
            targetPosition.y += _yMoveOffset;

            // 计算移动时间
            float distance = Vector3.Distance(_rectTransform.anchoredPosition3D, targetPosition);
            float duration = distance / _moveSpeed;

            // 移动到目标位置
            await _rectTransform
                .DOAnchorPos3D(targetPosition, duration)
                .SetLink(gameObject)
                .SetEase(Ease.Linear)
                .ToUniTask();

            // 设置朝向
            Vector3 scale = transform.localScale;
            scale.x = directions[i] ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        PlayIdle();
    }

    /// <summary>
    /// 骰子投出回调
    /// </summary>
    public void OnDiceThrowed()
    {
        // 创建骰子
        GameObject diceObj = Instantiate(_dicePrefab, _diceThrowStartPosition.position, Quaternion.identity, transform.parent);
        DiceRollAnimator diceAnimation = diceObj.GetComponent<DiceRollAnimator>();

        // 完成等待
        _diceThrowedCompletionSource.TrySetResult(diceAnimation);
    }

    /// <summary>
    /// 投骰子动画结束回调
    /// </summary>
    public void OnThrowAnimationEnded()
    {
        _throwAnimationEndedCompletionSource.TrySetResult(true);
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 重置动画状态
    /// </summary>
    private void ResetAnimationStates()
    {
        _animator.SetBool("Idle", false);
        _animator.SetBool("Walk", false);
        _animator.SetBool("Roll", false);
    }

    private void PlayIdle()
    {
        ResetAnimationStates();
        _animator.SetBool("Idle", true);
    }

    private void PlayWalk()
    {
        ResetAnimationStates();
        _animator.SetBool("Walk", true);
    }

    private void PlayRoll()
    {
        ResetAnimationStates();
        _animator.SetBool("Roll", true);
    }
    #endregion
}

