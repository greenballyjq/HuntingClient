using GameFramework.Manager;
using GameFramework.UI;
using UnityEngine;

/// <summary>
/// 准备页入场编排：选角揭晓时 RollRole 回休息位，纸和内容从屏外落下。
/// 运动本身由 <see cref="UIRevealToRest"/> 负责，这里只对时机和对象编组。
/// </summary>
public class UIPrepareIntro : MonoBehaviour, IUIComponent
{
    [SerializeField] private UIRevealToRest _rollRoleReveal;
    [SerializeField] private UIRevealToRest[] _paperReveals;
    [SerializeField] private UIRevealToRest[] _contentReveals;

    private EventManager _eventManager;
    private bool _revealed;

    public void Init()
    {
        _eventManager = GameServiceLocator.EventManager;
        _revealed = false;

        SnapAllToStart();
        _eventManager.AddListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);
    }

    public void CleanUp()
    {
        if (_eventManager != null)
            _eventManager.RemoveListener(PrepareEvents.SlotAnimationEnded, OnSlotAnimationEnded);

        KillAll();
        _revealed = false;
    }

    private void OnSlotAnimationEnded()
    {
        if (_revealed)
            return;

        _revealed = true;
        PlayAll();
    }

    private void SnapAllToStart()
    {
        if (_rollRoleReveal != null)
            _rollRoleReveal.SnapToStart();

        SnapArray(_paperReveals);
        SnapArray(_contentReveals);
    }

    private void PlayAll()
    {
        if (_rollRoleReveal != null)
            _rollRoleReveal.Play();

        PlayArray(_paperReveals);
        PlayArray(_contentReveals);
    }

    private void KillAll()
    {
        if (_rollRoleReveal != null)
            _rollRoleReveal.KillTween();

        KillArray(_paperReveals);
        KillArray(_contentReveals);
    }

    private static void SnapArray(UIRevealToRest[] reveals)
    {
        if (reveals == null)
            return;

        for (int i = 0; i < reveals.Length; i++)
        {
            if (reveals[i] != null)
                reveals[i].SnapToStart();
        }
    }

    private static void PlayArray(UIRevealToRest[] reveals)
    {
        if (reveals == null)
            return;

        for (int i = 0; i < reveals.Length; i++)
        {
            if (reveals[i] != null)
                reveals[i].Play();
        }
    }

    private static void KillArray(UIRevealToRest[] reveals)
    {
        if (reveals == null)
            return;

        for (int i = 0; i < reveals.Length; i++)
        {
            if (reveals[i] != null)
                reveals[i].KillTween();
        }
    }
}
