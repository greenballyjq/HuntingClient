/// <summary>
/// 幸运仪式增益来源
/// </summary>
public interface ILuckyBuffSource
{
    /// <summary>
    /// 激活：向修正层登记或瞬时发放
    /// </summary>
    void Activate();

    /// <summary>
    /// 停用：注销持续修正
    /// </summary>
    void Deactivate();
}
