using UnityEngine;

public class TimeTestManager : MonoBehaviour
{
    public static TimeTestManager Instance { get; private set; }
    
    private int timeState = 0;
    private float[] timeScales = { 1f, 0f, 2f };
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            timeState = (timeState + 1) % timeScales.Length;
            Time.timeScale = timeScales[timeState];
            Debug.Log($"时间缩放: {Time.timeScale}");
        }
        Debug.Log(Time.time);
    }
}