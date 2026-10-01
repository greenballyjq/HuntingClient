using System.Collections.Generic;
using cfg.HuntingConfig.Enum;
using GameFramework.Manager;
using UnityEngine;

/// <summary>
/// 本局/本图要预热的预制体。
/// </summary>
public sealed class RoundLoadManifest
{
    private const int DefaultPrewarmCount = 8;
    private const int BulletPrewarmCount = 20;
    private const int TrapPrewarmCount = 4;

    public string ScenePath { get; private set; }
    private readonly List<(GameObject prefab, int count)> _entries = new List<(GameObject prefab, int count)>();
    private GameObjectPoolManager _gameObjectPoolManager;
    private HuntingConfigManager _configManager;

    private RoundLoadManifest()
    {
        BindServices();
    }

    public static RoundLoadManifest ForMainMap(RoundContext context)
    {
        var manifest = new RoundLoadManifest
        {
            ScenePath = RoundScopeUnload.ForestScene
        };
        manifest.AddMapAnimals(context.MapData.ID);
        manifest.AddCombatPrefabs(context);
        return manifest;
    }

    public static RoundLoadManifest ForHiddenMap(RoundContext context)
    {
        var manifest = new RoundLoadManifest
        {
            ScenePath = RoundScopeUnload.SnowScene
        };
        if (context.HiddenMapData != null)
            manifest.AddMapAnimals(context.HiddenMapData.ID);
        manifest.AddCombatPrefabs(context);
        return manifest;
    }

    public void Prewarm()
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            GameObject prefab = _entries[i].prefab;
            if (prefab == null)
                continue;
            _gameObjectPoolManager.Prewarm(prefab, _entries[i].count);
        }
    }

    private void BindServices()
    {
        _gameObjectPoolManager = GameServiceLocator.GameObjectPoolManager;
        _configManager = GameServiceLocator.ConfigManager;
    }

    private void AddMapAnimals(int mapId)
    {
        Dictionary<ESpecieType, int[]> mapSpecies = _configManager.GetMapSpecies(mapId);
        foreach (var pair in mapSpecies)
        {
            int[] ids = pair.Value;
            for (int i = 0; i < ids.Length; i++)
            {
                GameObject prefab = _configManager._AnimalRefSo.GetAnimalPrefab(ids[i]);
                AddPrefab(prefab, DefaultPrewarmCount);
            }
        }
    }

    private void AddCombatPrefabs(RoundContext context)
    {
        if (context.RoleData != null)
        {
            var bullet = _configManager.GetBullet(1);
            if (bullet != null)
                AddPrefab(_configManager.BulletRefSo.GetBulletPrefab(bullet.ID), BulletPrewarmCount);
        }

        var trap = _configManager.GetProp(EPropType.Trap);
        if (trap != null)
            AddPrefab(_configManager.PropRefSo.GetPropPrefab(trap.ID), TrapPrewarmCount);

        if (context.SkillData != null)
            AddPrefab(_configManager.SkillRefSo.GetSkillPrefab(context.SkillData.ID), DefaultPrewarmCount);
    }

    private void AddPrefab(GameObject prefab, int count)
    {
        if (prefab == null)
            return;

        for (int i = 0; i < _entries.Count; i++)
        {
            if (_entries[i].prefab == prefab)
                return;
        }

        _entries.Add((prefab, count));
    }
}
