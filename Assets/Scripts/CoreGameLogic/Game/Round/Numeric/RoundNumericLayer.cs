using System;
using System.Collections.Generic;
using cfg.HuntingConfig.Enum;
using Hunting.Game.Animal;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 局内数值修正层
/// </summary>
public class RoundNumericLayer : IRoundManager
{
    private readonly List<ModifierEntry<IDamageModifier>> _damageModifiers = new List<ModifierEntry<IDamageModifier>>();
    private readonly List<ModifierEntry<IFireRateModifier>> _fireRateModifiers = new List<ModifierEntry<IFireRateModifier>>();
    private readonly List<ModifierEntry<IDropProcessor>> _dropProcessors = new List<ModifierEntry<IDropProcessor>>();
    private readonly List<ModifierEntry<ISpawnWeightModifier>> _spawnWeightModifiers = new List<ModifierEntry<ISpawnWeightModifier>>();
    private readonly List<ModifierEntry<IMeatGainModifier>> _meatGainModifiers = new List<ModifierEntry<IMeatGainModifier>>();
    private readonly List<ModifierEntry<IEnergyGainModifier>> _energyGainModifiers = new List<ModifierEntry<IEnergyGainModifier>>();
    private readonly List<ModifierEntry<ISettlementModifier>> _settlementModifiers = new List<ModifierEntry<ISettlementModifier>>();
    private readonly List<ModifierEntry<IMaxHealthModifier>> _maxHealthModifiers = new List<ModifierEntry<IMaxHealthModifier>>();
    private readonly List<ModifierEntry<ISpawnCooldownModifier>> _spawnCooldownModifiers = new List<ModifierEntry<ISpawnCooldownModifier>>();
    private readonly List<ModifierEntry<ISpawnCountModifier>> _spawnCountModifiers = new List<ModifierEntry<ISpawnCountModifier>>();
    private readonly List<ModifierEntry<ISpawnIntervalModifier>> _spawnIntervalModifiers = new List<ModifierEntry<ISpawnIntervalModifier>>();
    private readonly List<ModifierEntry<ISkillEnergyCostModifier>> _skillEnergyCostModifiers = new List<ModifierEntry<ISkillEnergyCostModifier>>();

    private int _nextId = 1;

    /// <summary>
    /// 射速修正变化
    /// </summary>
    public event Action FireRateChanged;

    public UniTask InitAsync(RoundContext context)
    {
        return UniTask.CompletedTask;
    }

    public void Dispose()
    {
        Clear();
        FireRateChanged = null;
    }

    #region 登记
    public NumericHandle RegisterDamage(IDamageModifier modifier) => Register(_damageModifiers, modifier);

    public void UnregisterDamage(NumericHandle handle) => Unregister(_damageModifiers, handle);

    public NumericHandle RegisterFireRate(IFireRateModifier modifier)
    {
        NumericHandle handle = Register(_fireRateModifiers, modifier);
        FireRateChanged?.Invoke();
        return handle;
    }

    public void UnregisterFireRate(NumericHandle handle)
    {
        if (Unregister(_fireRateModifiers, handle))
            FireRateChanged?.Invoke();
    }

    public NumericHandle RegisterDrop(IDropProcessor processor) => Register(_dropProcessors, processor);

    public void UnregisterDrop(NumericHandle handle) => Unregister(_dropProcessors, handle);

    public NumericHandle RegisterSpawnWeight(ISpawnWeightModifier modifier) => Register(_spawnWeightModifiers, modifier);

    public void UnregisterSpawnWeight(NumericHandle handle) => Unregister(_spawnWeightModifiers, handle);

    public NumericHandle RegisterMeatGain(IMeatGainModifier modifier) => Register(_meatGainModifiers, modifier);

    public void UnregisterMeatGain(NumericHandle handle) => Unregister(_meatGainModifiers, handle);

    public NumericHandle RegisterEnergyGain(IEnergyGainModifier modifier) => Register(_energyGainModifiers, modifier);

    public void UnregisterEnergyGain(NumericHandle handle) => Unregister(_energyGainModifiers, handle);

    public NumericHandle RegisterSettlement(ISettlementModifier modifier) => Register(_settlementModifiers, modifier);

    public void UnregisterSettlement(NumericHandle handle) => Unregister(_settlementModifiers, handle);

    public NumericHandle RegisterMaxHealth(IMaxHealthModifier modifier) => Register(_maxHealthModifiers, modifier);

    public void UnregisterMaxHealth(NumericHandle handle) => Unregister(_maxHealthModifiers, handle);

    public NumericHandle RegisterSpawnCooldown(ISpawnCooldownModifier modifier) => Register(_spawnCooldownModifiers, modifier);

    public void UnregisterSpawnCooldown(NumericHandle handle) => Unregister(_spawnCooldownModifiers, handle);

    public NumericHandle RegisterSpawnCount(ISpawnCountModifier modifier) => Register(_spawnCountModifiers, modifier);

    public void UnregisterSpawnCount(NumericHandle handle) => Unregister(_spawnCountModifiers, handle);

    public NumericHandle RegisterSpawnInterval(ISpawnIntervalModifier modifier) => Register(_spawnIntervalModifiers, modifier);

    public void UnregisterSpawnInterval(NumericHandle handle) => Unregister(_spawnIntervalModifiers, handle);

    public NumericHandle RegisterSkillEnergyCost(ISkillEnergyCostModifier modifier) => Register(_skillEnergyCostModifiers, modifier);

    public void UnregisterSkillEnergyCost(NumericHandle handle) => Unregister(_skillEnergyCostModifiers, handle);
    #endregion

    #region 求值
    public float EvaluateDamage(float baseDamage, DamageContext context)
    {
        float value = baseDamage;
        for (int i = 0; i < _damageModifiers.Count; i++)
            value = _damageModifiers[i].Modifier.Modify(value, context);
        return value;
    }

    public float EvaluateFireRate(float baseFireRate, FireRateContext context)
    {
        float value = baseFireRate;
        for (int i = 0; i < _fireRateModifiers.Count; i++)
            value = _fireRateModifiers[i].Modifier.Modify(value, context);
        return value;
    }

    public Dictionary<EDropType, int> EvaluateDropRewards(Dictionary<EDropType, int> baseRewards)
    {
        var draft = new DropDraft
        {
            Rewards = new Dictionary<EDropType, int>(baseRewards)
        };

        for (int i = 0; i < _dropProcessors.Count; i++)
            _dropProcessors[i].Modifier.Process(draft);

        return draft.Rewards;
    }

    public float EvaluateSpawnWeight(ESpecieType type, float baseWeight)
    {
        float value = baseWeight;
        for (int i = 0; i < _spawnWeightModifiers.Count; i++)
            value = _spawnWeightModifiers[i].Modifier.Modify(type, value);
        return value;
    }

    public int EvaluateMeatGain(int baseAmount)
    {
        int value = baseAmount;
        for (int i = 0; i < _meatGainModifiers.Count; i++)
            value = _meatGainModifiers[i].Modifier.Modify(value);
        return value;
    }

    public float EvaluateEnergyGain(float baseAmount)
    {
        float value = baseAmount;
        for (int i = 0; i < _energyGainModifiers.Count; i++)
            value = _energyGainModifiers[i].Modifier.Modify(value);
        return value;
    }

    public void EvaluateSettlement(SettlementDraft draft)
    {
        for (int i = 0; i < _settlementModifiers.Count; i++)
            _settlementModifiers[i].Modifier.Modify(draft);

        draft.CoinFromMeat = Mathf.RoundToInt(draft.CoinFromMeat * draft.ExtraMultiplier);
        draft.Point = Mathf.RoundToInt(draft.Point * draft.ExtraMultiplier);
        if (draft.IsDouble)
        {
            draft.CoinFromMeat *= 2;
            draft.Point *= 2;
        }
    }

    public float EvaluateMaxHealth(float baseHp)
    {
        float value = baseHp;
        for (int i = 0; i < _maxHealthModifiers.Count; i++)
            value = _maxHealthModifiers[i].Modifier.Modify(value);
        return value;
    }

    public float EvaluateSpawnCooldown(float baseCooldown)
    {
        float value = baseCooldown;
        for (int i = 0; i < _spawnCooldownModifiers.Count; i++)
            value = _spawnCooldownModifiers[i].Modifier.Modify(value);
        return value;
    }

    public int EvaluateSpawnCount(int baseCount)
    {
        int value = baseCount;
        for (int i = 0; i < _spawnCountModifiers.Count; i++)
            value = _spawnCountModifiers[i].Modifier.Modify(value);
        return value;
    }

    public float EvaluateSpawnInterval(float baseInterval)
    {
        float value = baseInterval;
        for (int i = 0; i < _spawnIntervalModifiers.Count; i++)
            value = _spawnIntervalModifiers[i].Modifier.Modify(value);
        return value;
    }

    public int EvaluateSkillEnergyCost(int baseCost)
    {
        int value = baseCost;
        for (int i = 0; i < _skillEnergyCostModifiers.Count; i++)
            value = _skillEnergyCostModifiers[i].Modifier.Modify(value);
        return Mathf.Max(0, value);
    }
    #endregion

    #region 伤害应用
    /// <summary>
    /// 对已求值伤害直接扣血
    /// </summary>
    public void Apply(IDamageable target, float amount, Vector3 hitPoint = default, Vector3 hitDir = default)
    {
        if (target == null)
            return;

        target.TakeDamage(amount, hitPoint, hitDir);
    }

    /// <summary>
    /// 求值后扣血
    /// </summary>
    public void Deal(IDamageable target, float baseDamage, DamageContext context, Vector3 hitPoint = default, Vector3 hitDir = default)
    {
        Apply(target, EvaluateDamage(baseDamage, context), hitPoint, hitDir);
    }
    #endregion

    private NumericHandle Register<T>(List<ModifierEntry<T>> list, T modifier)
    {
        int id = _nextId++;
        list.Add(new ModifierEntry<T> { Id = id, Modifier = modifier });
        return new NumericHandle(id);
    }

    private bool Unregister<T>(List<ModifierEntry<T>> list, NumericHandle handle)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Id != handle.Id)
                continue;

            list.RemoveAt(i);
            return true;
        }

        return false;
    }

    private void Clear()
    {
        _damageModifiers.Clear();
        _fireRateModifiers.Clear();
        _dropProcessors.Clear();
        _spawnWeightModifiers.Clear();
        _meatGainModifiers.Clear();
        _energyGainModifiers.Clear();
        _settlementModifiers.Clear();
        _maxHealthModifiers.Clear();
        _spawnCooldownModifiers.Clear();
        _spawnCountModifiers.Clear();
        _spawnIntervalModifiers.Clear();
        _skillEnergyCostModifiers.Clear();
    }

    private struct ModifierEntry<T>
    {
        public int Id;
        public T Modifier;
    }
}
