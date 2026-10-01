using GameFramework.Game;
using GameFramework.Utility;
using System;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 存档读写服务
/// </summary>
public class SaveService : IAppManager
{
    /// <summary>
    /// 存档文件名
    /// </summary>
    private const string SaveFileName = "player_save.json";

    /// <summary>
    /// 存档完整路径
    /// </summary>
    private string _savePath;

    public UniTask InitAsync()
    {
        _savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        Log.Info($"[SaveService] 初始化完成，路径: {_savePath}");
        return UniTask.CompletedTask;
    }

    public void Dispose()
    {
        Log.Info("[SaveService] 已释放");
    }

    #region 公共方法
    /// <summary>
    /// 存档文件是否存在
    /// </summary>
    public bool Exists()
    {
        return File.Exists(_savePath);
    }

    /// <summary>
    /// 写入玩家存档
    /// </summary>
    /// <param name="data">存档数据</param>
    /// <returns>是否成功</returns>
    public bool Write(PlayerSaveData data)
    {
        if (data == null)
        {
            Log.Warning("[SaveService] Write 失败：data 为空");
            return false;
        }

        try
        {
            string json = UnityJsonSerializer.ToJson(data, prettyPrint: true);
            File.WriteAllText(_savePath, json);
            Log.Info($"[SaveService] 存档成功: {_savePath}");
            return true;
        }
        catch (Exception e)
        {
            Log.Error($"[SaveService] 存档失败: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 尝试读取玩家存档
    /// </summary>
    /// <param name="data">输出数据</param>
    /// <returns>是否读取成功</returns>
    public bool TryRead(out PlayerSaveData data)
    {
        data = null;

        if (!Exists())
        {
            Log.Info("[SaveService] 无存档文件");
            return false;
        }

        try
        {
            string json = File.ReadAllText(_savePath);
            if (!UnityJsonSerializer.TryFromJson(json, out data) || data == null)
            {
                Log.Warning("[SaveService] 存档内容解析失败");
                data = null;
                return false;
            }

            Log.Info("[SaveService] 读档成功");
            return true;
        }
        catch (Exception e)
        {
            Log.Error($"[SaveService] 读档失败: {e.Message}");
            data = null;
            return false;
        }
    }
    #endregion
}
