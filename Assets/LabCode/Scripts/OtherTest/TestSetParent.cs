using UnityEngine;

public class TestSetParent : MonoBehaviour
{
    public Transform oldParent;
    public Transform newParent;
    public Transform child;

    void Start()
    {
        if (oldParent != null && child != null)
        {
            child.SetParent(oldParent);
            child.localPosition = new Vector3(2, 0, 0);
            Debug.Log($"初始设置: 子对象在oldParent下，本地坐标={child.localPosition}，世界坐标={child.position}");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (newParent != null && child != null)
            {
                Debug.Log($"O键换父前: 世界坐标={child.position}, 本地坐标={child.localPosition}");
                child.SetParent(newParent, true);
                Debug.Log($"O键换父后(true): 世界坐标={child.position}, 本地坐标={child.localPosition}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (oldParent != null && child != null)
            {
                Debug.Log($"P键换回前: 世界坐标={child.position}, 本地坐标={child.localPosition}");
                child.SetParent(oldParent, true);
                Debug.Log($"P键换回后(true): 世界坐标={child.position}, 本地坐标={child.localPosition}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (newParent != null && child != null)
            {
                Debug.Log($"K键换父前: 世界坐标={child.position}, 本地坐标={child.localPosition}");
                child.SetParent(newParent, false);
                Debug.Log($"K键换父后(false): 世界坐标={child.position}, 本地坐标={child.localPosition}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (oldParent != null && child != null)
            {
                Debug.Log($"L键换回前: 世界坐标={child.position}, 本地坐标={child.localPosition}");
                child.SetParent(oldParent, false);
                Debug.Log($"L键换回后(false): 世界坐标={child.position}, 本地坐标={child.localPosition}");
            }
        }
    }
}