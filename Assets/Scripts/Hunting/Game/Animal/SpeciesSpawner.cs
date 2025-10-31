using cfg.HuntingConfig;
using Cysharp.Threading.Tasks;
using Hunting;
using Hunting.Game.Animal;
using Hunting.Manager;
using UnityEngine;

/// <summary>
/// �����ɷ���
/// </summary>
public class SpeciesSpawner : MonoBehaviour
{
    /// <summary>
    /// �����ɷ�����
    /// </summary>
    public enum SpawnDirection
    {
        /// <summary>
        /// ��������ɣ������ƶ�
        /// </summary>
        Left,

        /// <summary>
        /// ���Ҳ����ɣ������ƶ�
        /// </summary>
        Right
    }

    #region �༭������
    /// <summary>
    /// �ɷ�����
    /// </summary>
    [SerializeField] private SpawnDirection spawnDirection;

    /// <summary>
    /// ��ǰ��ͼID
    /// </summary>
    [SerializeField] private int currentMapId = 1;

    /// <summary>
    /// ���ɼ�����룩
    /// </summary>
    [SerializeField] private float spawnInterval = 2f;

    /// <summary>
    /// ����λ��Z���귶Χ��ǰ��λ�ã�
    /// </summary>
    [SerializeField] private Vector2 spawnZRange = new Vector2(-3f, 3f);

    /// <summary>
    /// ����λ��Xƫ�ƣ����ݷ���̶���
    /// </summary>
    [SerializeField] private float spawnXOffset = 2f;

    [Header("�ƶ���������")]
    [Tooltip("��С�Ƕȣ��ȣ�")]
    [SerializeField] private float minAngle = 0f;

    [Tooltip("���Ƕȣ��ȣ�")]
    [SerializeField] private float maxAngle = 60f;
    #endregion

    #region ����ʱ����
    /// <summary>
    /// �ϴ�����ʱ��
    /// </summary>
    private float lastSpawnTime;

    /// <summary>
    /// ����Ԥ�������·��
    /// </summary>
    private const string ANIMAL_PREFAB_BASE_PATH = "Animals/";
    #endregion

    #region ��ʼ�����
    private bool initialized;

    /// <summary>
    /// �첽��ʼ��
    /// </summary>
    private async UniTask Init()
    {
        // �ȴ� HuntingGameConfigManager ��ʼ�����
        while (HuntingGameConfigManager.Instance == null || !HuntingGameConfigManager.Instance.Initialized)
        {
            await UniTask.Delay(10);
        }
        initialized = true;

        Debug.Log($"[SpeciesSpawner] {spawnDirection}�����ɷ�����ʼ����ɣ���ͼID: {currentMapId}");
    }
    #endregion

    private async void Start()
    {
        EventManager eventManager = await GameServiceLocator.GetFrameworkManagerAsync<EventManager>();
        
        eventManager.AddListener(HuntingEvents.GameStarted, OnGameStarted);
    }

    private async void OnGameStarted()
    {
        await Init();
        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        if (!initialized) return;

        // ������ɼ��
        if (Time.time - lastSpawnTime >= spawnInterval)
        {
            TrySpawnAnimal();
            lastSpawnTime = Time.time;
        }
    }

    /// <summary>
    /// �������ɶ���
    /// </summary>
    private void TrySpawnAnimal()
    {
        // �����ù�������ȡ�������
        var (specie, stayTime) = HuntingGameConfigManager.Instance.GetRandomSpecieForMap(currentMapId);

        if (specie == null)
        {
            Debug.LogError($"[SpeciesSpawner] �޷���ȡ�������ݣ���ͼID: {currentMapId}");
            return;
        }

        // ���ɶ���
        SpawnAnimal(specie, stayTime);
    }

    /// <summary>
    /// ���ɶ���ʵ��
    /// </summary>
    /// <param name="specie">��������</param>
    /// <param name="stayTime">פ��ʱ��</param>
    private void SpawnAnimal(Specie specie, float stayTime)
    {
        // ��������ID��ȡ��Ӧ��Ԥ��������
        string prefabName = GetAnimalPrefabName(specie);
        string prefabPath = $"{ANIMAL_PREFAB_BASE_PATH}{prefabName}";

        // ���ض���Ԥ����
        GameObject animalPrefab = Resources.Load<GameObject>(prefabPath);
        if (animalPrefab == null)
        {
            Debug.LogError($"[SpeciesSpawner] ����Ԥ���岻����: {prefabPath}");
            return;
        }

        // ��������λ�ã������ɷ���λ�ã�
        Vector3 spawnPosition = CalculateSpawnPosition();

        // �����ƶ�����
        Vector3 moveDirection = CalculateMoveDirection();

        // ʵ��������
        GameObject animalObj = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
        AnimalBehavior animal = animalObj.GetComponent<AnimalBehavior>();

        if (animal != null)
        {
            // ��ʼ������
            animal.Initialize(specie, stayTime, moveDirection);

            // ���ö��ﳯ�����ƶ�����һ��
            if (moveDirection != Vector3.zero)
            {
                animalObj.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
        else
        {
            Debug.LogError($"[SpeciesSpawner] ����Ԥ����ȱ��Animal���: {prefabName}");
            Destroy(animalObj);
        }
    }

    /// <summary>
    /// �����������ݻ�ȡԤ��������
    /// </summary>
    /// <param name="specie">��������</param>
    /// <returns>Ԥ��������</returns>
    private string GetAnimalPrefabName(Specie specie)
    {
        return $"Animal_{specie.VolumeType}_{specie.ID}";
    }

    /// <summary>
    /// ��������λ��
    /// </summary>
    /// <returns>����λ��</returns>
    private Vector3 CalculateSpawnPosition()
    {
        // �����ɷ��������λ��
        Vector3 basePosition = transform.position;

        // ���ݷ������X��ƫ��
        float xOffset = spawnDirection == SpawnDirection.Left ? -spawnXOffset : spawnXOffset;

        // ��Z�᷶Χ�����
        float zOffset = Random.Range(spawnZRange.x, spawnZRange.y);

        return basePosition + new Vector3(xOffset, 0f, zOffset);
    }

    /// <summary>
    /// �����ƶ�����
    /// </summary>
    /// <returns>�ƶ���������</returns>
    private Vector3 CalculateMoveDirection()
    {
        // �����õĽǶȷ�Χ�����
        float randomAngle = Random.Range(minAngle, maxAngle);
        return Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
    }

    /// <summary>
    /// ���õ�ǰ��ͼ
    /// </summary>
    /// <param name="mapId">��ͼID</param>
    public void SetCurrentMap(int mapId)
    {
        currentMapId = mapId;
    }

    /// <summary>
    /// �������ɼ��
    /// </summary>
    /// <param name="interval">���ɼ�����룩</param>
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
    }

    #region ����
    /// <summary>
    /// ��Scene��ͼ����ʾ���ɷ�Χ���ƶ�����
    /// </summary>
    private void OnDrawGizmos()
    {
        // ����λ�÷�Χ
        Gizmos.color = spawnDirection == SpawnDirection.Left ? Color.blue : Color.red;

        Vector3 center = transform.position;
        Vector3 leftPos = center + new Vector3(spawnDirection == SpawnDirection.Left ? -spawnXOffset : spawnXOffset, 0f, spawnZRange.x);
        Vector3 rightPos = center + new Vector3(spawnDirection == SpawnDirection.Left ? -spawnXOffset : spawnXOffset, 0f, spawnZRange.y);

        Gizmos.DrawLine(leftPos, rightPos);
        Gizmos.DrawWireSphere(leftPos, 0.5f);
        Gizmos.DrawWireSphere(rightPos, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        // �ƶ�����Χ
        Gizmos.color = Color.green;
        DrawDirectionArc(center, minAngle, maxAngle, 3f);
    }

    /// <summary>
    /// ���Ʒ����ߣ������ã�
    /// </summary>
    private void DrawDirectionArc(Vector3 center, float startAngle, float endAngle, float radius)
    {
        int segments = 10;
        float angleStep = (endAngle - startAngle) / segments;

        Vector3 prevPoint = center + Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 nextPoint = center + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // ������ʼ�ͽ���������
        Gizmos.DrawLine(center, center + Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward * radius);
        Gizmos.DrawLine(center, center + Quaternion.Euler(0f, endAngle, 0f) * Vector3.forward * radius);
    }
    #endregion
}

