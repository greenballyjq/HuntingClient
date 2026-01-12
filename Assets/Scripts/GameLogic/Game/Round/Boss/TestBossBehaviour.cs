using UnityEngine;

public class TestSnow : MonoBehaviour
{
    [SerializeField] private BossBehaviour bossBehaviour;
    [SerializeField] private Spawner[] spawners;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //bossBehaviour.Init(null, 2f);
            foreach (var spawner in spawners)
            {
                //spawner.SetMap(1);
                //spawner.SetActive(true);
            }
        }
    }
}