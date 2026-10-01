using System;
using System.Collections.Generic;
using cfg.HuntingConfig.Enum;

/// <summary>
/// 伤害来源类别
/// </summary>
[Flags]
public enum DamageSourceKind
{
    None = 0,
    Weapon = 1,
    Skill = 2,
    Prop = 4
}

/// <summary>
/// 射速武器类别
/// </summary>
public enum FireRateWeaponKind
{
    Player,
    Skill
}

/// <summary>
/// 伤害求值上下文
/// </summary>
public readonly struct DamageContext
{
    public readonly DamageSourceKind SourceKind;

    public DamageContext(DamageSourceKind sourceKind)
    {
        SourceKind = sourceKind;
    }
}

/// <summary>
/// 射速求值上下文
/// </summary>
public readonly struct FireRateContext
{
    public readonly FireRateWeaponKind WeaponKind;

    public FireRateContext(FireRateWeaponKind weaponKind)
    {
        WeaponKind = weaponKind;
    }
}

/// <summary>
/// 死亡掉落草稿
/// </summary>
public class DropDraft
{
    public Dictionary<EDropType, int> Rewards { get; set; }
}

/// <summary>
/// 结算草稿
/// </summary>
public class SettlementDraft
{
    public int DisplayMeat;
    public int CoinFromMeat;
    public int CoinFromSpecie;
    public int Point;
    public float ExtraMultiplier = 1f;
    public bool IsDouble;
}

/// <summary>
/// 结算结果
/// </summary>
public class SettlementReward
{
    public int DisplayMeat;
    public int CoinFromMeat;
    public int Point;
}

/// <summary>
/// 修正登记句柄
/// </summary>
public readonly struct NumericHandle : IEquatable<NumericHandle>
{
    public readonly int Id;

    public NumericHandle(int id)
    {
        Id = id;
    }

    public bool Equals(NumericHandle other) => Id == other.Id;
    public override bool Equals(object obj) => obj is NumericHandle other && Equals(other);
    public override int GetHashCode() => Id;
}

/// <summary>
/// 伤害修正
/// </summary>
public interface IDamageModifier
{
    float Modify(float current, DamageContext context);
}

/// <summary>
/// 射速修正
/// </summary>
public interface IFireRateModifier
{
    float Modify(float current, FireRateContext context);
}

/// <summary>
/// 死亡掉落处理
/// </summary>
public interface IDropProcessor
{
    void Process(DropDraft draft);
}

/// <summary>
/// 刷怪权重修正
/// </summary>
public interface ISpawnWeightModifier
{
    float Modify(ESpecieType type, float current);
}

/// <summary>
/// 肉发放修正
/// </summary>
public interface IMeatGainModifier
{
    int Modify(int current);
}

/// <summary>
/// 能量发放修正
/// </summary>
public interface IEnergyGainModifier
{
    float Modify(float current);
}

/// <summary>
/// 结算修正
/// </summary>
public interface ISettlementModifier
{
    void Modify(SettlementDraft draft);
}

/// <summary>
/// 最大生命修正
/// </summary>
public interface IMaxHealthModifier
{
    float Modify(float current);
}

/// <summary>
/// 刷怪冷却修正
/// </summary>
public interface ISpawnCooldownModifier
{
    float Modify(float current);
}

/// <summary>
/// 刷怪每轮只数修正
/// </summary>
public interface ISpawnCountModifier
{
    int Modify(int current);
}

/// <summary>
/// 同轮刷怪间隔修正
/// </summary>
public interface ISpawnIntervalModifier
{
    float Modify(float current);
}

/// <summary>
/// 技能能量消耗修正
/// </summary>
public interface ISkillEnergyCostModifier
{
    int Modify(int current);
}

/// <summary>
/// 按来源类别连乘的伤害修正
/// </summary>
public sealed class MultiplyDamageModifier : IDamageModifier
{
    private readonly float _multiplier;
    private readonly DamageSourceKind _kinds;

    public MultiplyDamageModifier(float multiplier, DamageSourceKind kinds)
    {
        _multiplier = multiplier;
        _kinds = kinds;
    }

    public float Modify(float current, DamageContext context)
    {
        if ((_kinds & context.SourceKind) == 0)
            return current;

        return current * _multiplier;
    }
}

/// <summary>
/// 按武器类别连乘的射速修正
/// </summary>
public sealed class MultiplyFireRateModifier : IFireRateModifier
{
    private readonly float _multiplier;
    private readonly FireRateWeaponKind _weaponKind;

    public MultiplyFireRateModifier(float multiplier, FireRateWeaponKind weaponKind)
    {
        _multiplier = multiplier;
        _weaponKind = weaponKind;
    }

    public float Modify(float current, FireRateContext context)
    {
        if (context.WeaponKind != _weaponKind)
            return current;

        return current * _multiplier;
    }
}

/// <summary>
/// 高阶体型权重连乘
/// </summary>
public sealed class HighTierSpawnWeightModifier : ISpawnWeightModifier
{
    private readonly float _multiplier;

    public HighTierSpawnWeightModifier(float multiplier)
    {
        _multiplier = multiplier;
    }

    public float Modify(ESpecieType type, float current)
    {
        if (type != ESpecieType.Large && type != ESpecieType.Bullet && type != ESpecieType.ThreeKPCoin)
            return current;

        return current * _multiplier;
    }
}
