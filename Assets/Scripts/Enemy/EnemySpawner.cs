using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float spawn = 0;
    [SerializeField] private EnemySpawnData[] enemies;
    public Transform[] spawnPoints;

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject prefab;
        public float spawnWeight;
    }
    public void Update()
    {

        if (spawn >= 1)
        {
            SpawnEnemy();
            spawn = 0;
        }
        else
        {
            spawn += Time.deltaTime;
        }

    }
    private void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int i = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[i].position;

        foreach (EnemySpawnData enemy in enemies)
        {
            Instantiate(enemy.prefab, spawnPos, Quaternion.identity);
        }
        return;
    }
    public void UpdateSpawnWeights()
    {
        enemies[0].spawnWeight = 60f;

        enemies[1].spawnWeight = 20f;

        enemies[2].spawnWeight = 5f;
    }
}
