using GameFramework.Audio;
using GameFramework.UI;
using UnityEngine;

/// <summary>
/// UI / 流程音效关联资源配置
/// </summary>
[CreateAssetMenu(fileName = "UiAudioRefSo", menuName = "SO/UiAudioRefSo")]
public class UiAudioRefSo : ScriptableObject, IUIButtonAudioDefaults
{
    /// <summary>
    /// 准备页 BGM
    /// </summary>
    public MusicCue PrepareBgm;

    /// <summary>
    /// Boss 警报
    /// </summary>
    public SfxCue BossAlert;

    /// <summary>
    /// Boss 血量增长
    /// </summary>
    public SfxCue BossHealthGrowth;

    /// <summary>
    /// 任务派发
    /// </summary>
    public SfxCue QuestDispatched;

    /// <summary>
    /// 任务完成
    /// </summary>
    public SfxCue QuestCompleted;

    /// <summary>
    /// 真结算 / 隐藏结算胜利
    /// </summary>
    public MusicCue SettlementVictory;

    /// <summary>
    /// 可悬停指针进入按钮
    /// </summary>
    public SfxCue ButtonHover;

    /// <summary>
    /// 未覆盖时的通用点击
    /// </summary>
    public SfxCue ButtonClick;

    /// <summary>
    /// 骰子投出时的欢呼
    /// </summary>
    public SfxCue Cheer;

    SfxCue IUIButtonAudioDefaults.Hover => ButtonHover;

    SfxCue IUIButtonAudioDefaults.Click => ButtonClick;
}
