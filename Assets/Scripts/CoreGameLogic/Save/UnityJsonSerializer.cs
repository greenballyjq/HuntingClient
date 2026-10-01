using System;
using GameFramework.Utility;
using UnityEngine;

/// <summary>
/// Unity JsonUtility 封装
/// </summary>
public static class UnityJsonSerializer
{
    /// <summary>
    /// 序列化为 Json 字符串
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">数据</param>
    /// <param name="prettyPrint">是否格式化</param>
    public static string ToJson<T>(T data, bool prettyPrint = true)
    {
        return JsonUtility.ToJson(data, prettyPrint);
    }

    /// <summary>
    /// 反序列化 Json 字符串
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="json">Json 文本</param>
    public static T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// 尝试反序列化；失败时返回 false 且 result 为 default
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="json">Json 文本</param>
    /// <param name="result">输出结果</param>
    public static bool TryFromJson<T>(string json, out T result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            result = JsonUtility.FromJson<T>(json);
            return result != null;
        }
        catch (Exception e)
        {
            Log.Warning($"[UnityJsonSerializer] 反序列化失败: {e.Message}");
            result = default;
            return false;
        }
    }
}
