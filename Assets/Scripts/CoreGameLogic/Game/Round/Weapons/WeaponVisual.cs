using UnityEngine;

/// <summary>
/// 武器视觉效果
/// </summary>
public class WeaponVisual : MonoBehaviour
{
    /// <summary>
    /// 技能特效
    /// </summary>
    [SerializeField] private GameObject _skillEffect;

    /// <summary>
    /// 设置技能特效
    /// </summary>
    public void SetSkillEffect(bool active)
    {
        _skillEffect.SetActive(active);
    } 
}
