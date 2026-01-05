using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

/// <summary>
/// 骰子动画控制器
/// </summary>
public class DiceAnimation : MonoBehaviour
{
    /// <summary>
    /// 动画器组件
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// 骰子动画结束等待源
    /// </summary>
    private UniTaskCompletionSource<bool> _rollEndedCompletionSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    #region 公共方法
    /// <summary>
    /// 播放骰子滚动动画
    /// </summary>
    /// <param name="dicePoint">骰子点数</param>
    public async UniTask PlayRoll(int dicePoint)
    {
        _animator.SetFloat("RollPoint", dicePoint);
        _animator.SetBool("Roll", true);

        _rollEndedCompletionSource = new UniTaskCompletionSource<bool>();

        await _rollEndedCompletionSource.Task;
    }

    /// <summary>
    /// 骰子动画事件
    /// </summary>
    public void OnDiceRollEnded()
    {
        _rollEndedCompletionSource?.TrySetResult(true);
    }

    /// <summary>
    /// 销毁骰子
    /// </summary>
    public void DestroyDice()
    {
        // TODO: 之后定义销毁动画
        Destroy(gameObject);
    }
    #endregion
}

