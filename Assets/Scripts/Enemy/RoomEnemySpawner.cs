using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RoomEnemySpawner : MonoBehaviour
{
    public bool spawnActive = false;
    [SerializeField] Transform[] spawnPositions;

    [SerializeField] List<EnemyObject> spawnableEnemies = new();
    [SerializeField] List<EnemyObject> activeEnemies = new();
    [SerializeField] int maxEnemies = 30;
    [SerializeField] float spawnTime = 1f;
    ActionTimer spawnTimer;

    private void Awake() {
        spawnTimer = new(spawnTime);
    }

    private void Update() {
        if(spawnActive) {
            if(spawnTimer.UpdateTimer()) {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy() {
        if(spawnableEnemies.Count <= 0) {
            Debug.Log("Enemy list is empty cant spawn any enemies");
            return;
        }
        int max = spawnableEnemies.Count;

        EnemyObject spawnedEnemy = Instantiate(spawnableEnemies[Random.Range(0, max)]);
        spawnedEnemy.SpawnEnemy(GetSpawnPosition());
        activeEnemies.Add(spawnedEnemy);
    }

    Vector3 GetSpawnPosition() {
        if(spawnPositions.Length <= 0) {
            Debug.Log("Missing spawn positions in spawner, spawning enemy at zero");
            return Vector3.zero;
        }
        Vector3 spawnPos = Vector3.zero;

        int max = spawnPositions.Length;

        spawnPos = spawnPositions[Random.Range(0, max)].position;


        return spawnPos;
    }
}
