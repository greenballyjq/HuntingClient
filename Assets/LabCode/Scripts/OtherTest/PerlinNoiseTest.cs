using UnityEngine;

/// <summary>
/// PerlinNoise 测试脚本
/// 演示 Mathf.PerlinNoise 和 Mathf.PerlinNoise1D 的各种用法
/// </summary>
public class PerlinNoiseTest : MonoBehaviour
{
    [Header("测试模式")]
    [SerializeField] private TestMode _testMode = TestMode.MovingObject;
    
    public enum TestMode
    {
        MovingObject,      // 平滑随机移动
        TerrainHeight,     // 地形高度生成
        CloudTexture,      // 云朵纹理生成
        WavePattern,       // 波浪图案
        RandomOffset       // 随机偏移
    }
    
    [Header("移动物体测试")]
    [SerializeField] private Transform _targetObject;
    [SerializeField] private float _moveSpeed = 0.5f;
    [SerializeField] private float _moveRange = 5f;
    
    [Header("地形高度测试")]
    [SerializeField] private int _terrainSize = 50;
    [SerializeField] private float _terrainScale = 0.1f;
    [SerializeField] private float _terrainHeight = 10f;
    [SerializeField] private bool _showTerrainGizmos = true;
    
    [Header("云朵纹理测试")]
    [SerializeField] private int _textureSize = 256;
    [SerializeField] private float _cloudScale = 0.05f;
    [SerializeField] private bool _generateTexture = false;
    
    [Header("波浪图案测试")]
    [SerializeField] private int _wavePoints = 50;
    [SerializeField] private float _waveAmplitude = 2f;
    [SerializeField] private float _waveFrequency = 0.1f;
    [SerializeField] private bool _showWaveGizmos = true;
    
    [Header("随机偏移测试")]
    [SerializeField] private Transform[] _objectsToOffset;
    [SerializeField] private float _offsetRange = 1f;
    [SerializeField] private float _offsetSpeed = 0.3f;
    
    private Texture2D _noiseTexture;
    private float _time = 0f;
    private Vector3[] _wavePositions;

    private void Start()
    {
        if (_testMode == TestMode.CloudTexture)
        {
            GenerateNoiseTexture();
        }
        
        if (_testMode == TestMode.WavePattern)
        {
            GenerateWavePattern();
        }
    }

    private void Update()
    {
        _time += Time.deltaTime;
        
        switch (_testMode)
        {
            case TestMode.MovingObject:
                TestMovingObject();
                break;
            case TestMode.RandomOffset:
                TestRandomOffset();
                break;
        }
    }

    #region 示例1: 平滑随机移动物体
    /// <summary>
    /// 示例1: 使用 PerlinNoise 让物体平滑随机移动
    /// </summary>
    private void TestMovingObject()
    {
        if (_targetObject == null) return;
        
        // 使用时间作为输入，产生平滑的随机移动
        // PerlinNoise(x, y) 返回 0-1 之间的值，减去 0.5 后乘以范围得到 -范围/2 到 +范围/2
        float x = (Mathf.PerlinNoise(_time * _moveSpeed, 0) - 0.5f) * _moveRange;
        float y = (Mathf.PerlinNoise(0, _time * _moveSpeed) - 0.5f) * _moveRange;
        
        _targetObject.position = new Vector3(x, y, 0);
    }
    #endregion

    #region 示例2: 生成地形高度
    /// <summary>
    /// 示例2: 使用 PerlinNoise 生成地形高度图
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_testMode == TestMode.TerrainHeight && _showTerrainGizmos)
        {
            DrawTerrainGizmos();
        }
        
        if (_testMode == TestMode.WavePattern && _showWaveGizmos)
        {
            DrawWaveGizmos();
        }
    }
    
    private void DrawTerrainGizmos()
    {
        Gizmos.color = Color.green;
        
        for (int x = 0; x < _terrainSize; x++)
        {
            for (int z = 0; z < _terrainSize; z++)
            {
                // 使用 PerlinNoise 生成高度
                float height = Mathf.PerlinNoise(x * _terrainScale, z * _terrainScale) * _terrainHeight;
                
                Vector3 pos = transform.position + new Vector3(x, height, z);
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }
    }
    
    [ContextMenu("生成地形高度数据")]
    private void GenerateTerrainData()
    {
        Debug.Log("=== 地形高度数据示例 ===");
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                float height = Mathf.PerlinNoise(x * _terrainScale, z * _terrainScale) * _terrainHeight;
                Debug.Log($"位置 ({x}, {z}) 高度: {height:F2}");
            }
        }
    }
    #endregion

    #region 示例3: 生成云朵纹理
    /// <summary>
    /// 示例3: 使用 PerlinNoise 生成云朵纹理
    /// </summary>
    private void GenerateNoiseTexture()
    {
        _noiseTexture = new Texture2D(_textureSize, _textureSize);
        
        for (int x = 0; x < _textureSize; x++)
        {
            for (int y = 0; y < _textureSize; y++)
            {
                // 使用 PerlinNoise 生成云朵密度
                float noise = Mathf.PerlinNoise(x * _cloudScale, y * _cloudScale);
                
                // 转换为颜色（白色云朵）
                Color color = new Color(noise, noise, noise, 1f);
                _noiseTexture.SetPixel(x, y, color);
            }
        }
        
        _noiseTexture.Apply();
        Debug.Log($"[PerlinNoiseTest] 已生成云朵纹理: {_textureSize}x{_textureSize}");
    }
    
    [ContextMenu("生成云朵纹理")]
    private void GenerateCloudTexture()
    {
        GenerateNoiseTexture();
        
        // 可以保存纹理或应用到材质
        if (_noiseTexture != null)
        {
            Debug.Log($"[PerlinNoiseTest] 纹理已生成，可以使用 GetComponent<Renderer>().material.mainTexture = _noiseTexture 来应用");
        }
    }
    
    private void OnGUI()
    {
        if (_testMode == TestMode.CloudTexture && _noiseTexture != null)
        {
            // 在屏幕上显示生成的纹理
            GUI.DrawTexture(new Rect(10, 10, 256, 256), _noiseTexture);
        }
    }
    #endregion

    #region 示例4: 波浪图案（使用 PerlinNoise1D）
    /// <summary>
    /// 示例4: 使用 PerlinNoise1D 生成波浪图案
    /// </summary>
    private void GenerateWavePattern()
    {
        _wavePositions = new Vector3[_wavePoints];
        
        for (int i = 0; i < _wavePoints; i++)
        {
            float x = i;
            // 使用 PerlinNoise1D 生成平滑的波浪高度
            float y = (Mathf.PerlinNoise1D(i * _waveFrequency) - 0.5f) * _waveAmplitude;
            
            _wavePositions[i] = new Vector3(x * 0.2f, y, 0);
        }
    }
    
    private void DrawWaveGizmos()
    {
        if (_wavePositions == null || _wavePositions.Length == 0) return;
        
        Gizmos.color = Color.cyan;
        
        for (int i = 0; i < _wavePositions.Length - 1; i++)
        {
            Vector3 start = transform.position + _wavePositions[i];
            Vector3 end = transform.position + _wavePositions[i + 1];
            Gizmos.DrawLine(start, end);
        }
    }
    
    [ContextMenu("生成波浪数据")]
    private void GenerateWaveData()
    {
        Debug.Log("=== 波浪数据示例（使用 PerlinNoise1D）===");
        for (int i = 0; i < 20; i++)
        {
            float y = (Mathf.PerlinNoise1D(i * _waveFrequency) - 0.5f) * _waveAmplitude;
            Debug.Log($"点 {i}: Y = {y:F2}");
        }
    }
    #endregion

    #region 示例5: 随机偏移多个物体
    /// <summary>
    /// 示例5: 使用 PerlinNoise 给多个物体添加平滑随机偏移
    /// </summary>
    private void TestRandomOffset()
    {
        if (_objectsToOffset == null || _objectsToOffset.Length == 0) return;
        
        for (int i = 0; i < _objectsToOffset.Length; i++)
        {
            if (_objectsToOffset[i] == null) continue;
            
            // 每个物体使用不同的时间偏移，产生不同的运动
            float timeOffset = i * 100f; // 让每个物体有不同的起始点
            
            float offsetX = (Mathf.PerlinNoise(_time * _offsetSpeed + timeOffset, 0) - 0.5f) * _offsetRange;
            float offsetY = (Mathf.PerlinNoise(0, _time * _offsetSpeed + timeOffset) - 0.5f) * _offsetRange;
            
            // 保存原始位置（这里简化处理，实际应该保存初始位置）
            Vector3 basePos = transform.position + new Vector3(i * 2f, 0, 0);
            _objectsToOffset[i].position = basePos + new Vector3(offsetX, offsetY, 0);
        }
    }
    #endregion

    #region 其他示例方法
    /// <summary>
    /// 示例: 使用 PerlinNoise 生成随机但平滑的数值
    /// </summary>
    [ContextMenu("测试随机数值生成")]
    private void TestRandomValue()
    {
        Debug.Log("=== PerlinNoise 随机数值示例 ===");
        for (int i = 0; i < 10; i++)
        {
            float value = Mathf.PerlinNoise(i * 0.5f, 0);
            Debug.Log($"输入 {i * 0.5f}: 输出 {value:F3}");
        }
    }
    
    /// <summary>
    /// 示例: 比较 PerlinNoise 和 PerlinNoise1D
    /// </summary>
    [ContextMenu("比较 PerlinNoise 和 PerlinNoise1D")]
    private void CompareNoiseFunctions()
    {
        Debug.Log("=== 比较 PerlinNoise 和 PerlinNoise1D ===");
        float x = 5.5f;
        
        float noise2D = Mathf.PerlinNoise(x, 0);
        float noise1D = Mathf.PerlinNoise1D(x);
        
        Debug.Log($"输入: {x}");
        Debug.Log($"PerlinNoise({x}, 0) = {noise2D:F3}");
        Debug.Log($"PerlinNoise1D({x}) = {noise1D:F3}");
        Debug.Log($"注意: 两个函数的结果可能不同，因为算法不同");
    }
    
    /// <summary>
    /// 示例: 使用多层噪声生成更复杂的地形
    /// </summary>
    [ContextMenu("多层噪声示例")]
    private void TestMultiLayerNoise()
    {
        Debug.Log("=== 多层噪声示例（用于生成更复杂的地形）===");
        
        float x = 10f;
        float z = 10f;
        
        // 第一层：大尺度地形
        float baseHeight = Mathf.PerlinNoise(x * 0.01f, z * 0.01f) * 100f;
        
        // 第二层：中等细节
        float detail1 = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 20f;
        
        // 第三层：小细节
        float detail2 = Mathf.PerlinNoise(x * 0.5f, z * 0.5f) * 5f;
        
        float finalHeight = baseHeight + detail1 + detail2;
        
        Debug.Log($"位置 ({x}, {z}):");
        Debug.Log($"  基础高度: {baseHeight:F2}");
        Debug.Log($"  中等细节: {detail1:F2}");
        Debug.Log($"  小细节: {detail2:F2}");
        Debug.Log($"  最终高度: {finalHeight:F2}");
    }
    #endregion

    private void OnDestroy()
    {
        if (_noiseTexture != null)
        {
            Destroy(_noiseTexture);
        }
    }
}

