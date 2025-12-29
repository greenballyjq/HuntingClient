using UnityEngine;

public class TestFind : MonoBehaviour
{
    public string tagName = "Funky";
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("测试 transform.Find('Player'):");
            Transform found = transform.Find("Player");
            Debug.Log(found != null ? $"找到: {found.name}" : "未找到");
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("测试 GameObject.Find('Player'):");
            GameObject found = GameObject.Find("Player");
            Debug.Log(found != null ? $"找到: {found.name}" : "未找到");
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log($"测试 FindGameObjectWithTag('{tagName}'):");
            GameObject found = GameObject.FindGameObjectWithTag(tagName);
            Debug.Log(found != null ? $"找到: {found.name}" : "未找到");
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log($"测试 FindGameObjectsWithTag('{tagName}'):");
            GameObject[] found = GameObject.FindGameObjectsWithTag(tagName);
            Debug.Log($"找到 {found.Length} 个对象");
        }
    }
}