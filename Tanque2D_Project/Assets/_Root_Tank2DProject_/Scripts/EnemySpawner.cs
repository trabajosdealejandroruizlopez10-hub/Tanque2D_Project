using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int spawnWeight = 1; // Probabilidad relativa de spawn
    public int minBiome = 0; // En qué bioma empieza a aparecer (0=Bosque, 1=Desierto, 2=Nieve)
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform player;
    public float spawnDistance = 15f; // Distancia del jugador donde spawnnean
    public float spawnInterval = 3f; // Tiempo entre spawns
    public float spawnHeightMin = 2f; // Altura mínima de spawn
    public float spawnHeightMax = 8f; // Altura máxima de spawn

    [Header("Difficulty Scaling")]
    public bool enableDifficultyScaling = true;
    public float difficultyIncreaseRate = 0.1f; // Cada cuántos segundos aumenta
    public float minSpawnInterval = 0.5f; // Intervalo mínimo entre spawns
    public int maxSimultaneousEnemies = 10;

    [Header("Enemy Types")]
    public EnemySpawnData[] enemyTypes;

    [Header("Current State")]
    public int currentBiome = 0;
    public float currentDifficulty = 1f;

    private float nextSpawnTime;
    private int activeEnemyCount = 0;
    private float gameStartTime;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        nextSpawnTime = Time.time + spawnInterval;
        gameStartTime = Time.time;
    }

    void Update()
    {
        // Actualizar dificultad con el tiempo
        if (enableDifficultyScaling)
        {
            float elapsed = Time.time - gameStartTime;
            currentDifficulty = 1f + (elapsed * difficultyIncreaseRate);
        }

        // Intentar spawn
        if (Time.time >= nextSpawnTime && activeEnemyCount < maxSimultaneousEnemies)
        {
            SpawnEnemy();

            // Calcular siguiente spawn basado en dificultad
            float adjustedInterval = spawnInterval / currentDifficulty;
            adjustedInterval = Mathf.Max(adjustedInterval, minSpawnInterval);
            nextSpawnTime = Time.time + adjustedInterval;
        }

        // Contar enemigos activos
        activeEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void SpawnEnemy()
    {
        if (player == null || enemyTypes.Length == 0)
            return;

        // Filtrar enemigos disponibles para el bioma actual
        List<EnemySpawnData> availableEnemies = new List<EnemySpawnData>();
        int totalWeight = 0;

        foreach (EnemySpawnData data in enemyTypes)
        {
            if (data.minBiome <= currentBiome && data.enemyPrefab != null)
            {
                availableEnemies.Add(data);
                totalWeight += data.spawnWeight;
            }
        }

        if (availableEnemies.Count == 0)
        {
            Debug.LogWarning("No hay enemigos disponibles para spawnnear en el bioma " + currentBiome);
            return;
        }

        // Selección ponderada
        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;
        EnemySpawnData selectedEnemy = availableEnemies[0];

        foreach (EnemySpawnData data in availableEnemies)
        {
            currentWeight += data.spawnWeight;
            if (randomWeight < currentWeight)
            {
                selectedEnemy = data;
                break;
            }
        }

        // Calcular posición de spawn
        Vector3 spawnPos = CalculateSpawnPosition();

        // Instanciar enemigo
        GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPos, Quaternion.identity);

        // Asignar referencia al jugador si el enemigo la necesita
        AssignPlayerReference(enemy);

        Debug.Log($"Spawned {enemy.name} at {spawnPos}");
    }

    Vector3 CalculateSpawnPosition()
    {
        // Spawn a la derecha del jugador (fuera de vista)
        float spawnX = player.position.x + spawnDistance;
        float spawnY = Random.Range(spawnHeightMin, spawnHeightMax);

        return new Vector3(spawnX, spawnY, 0f);
    }

    void AssignPlayerReference(GameObject enemy)
    {
        // Intentar asignar referencia al jugador en scripts comunes
        var airplane = enemy.GetComponent<EnemyAirplane>();
        if (airplane != null)
            airplane.player = player;

        var helicopter = enemy.GetComponent<EnemyHelicopter>();
        if (helicopter != null)
            helicopter.player = player;

        var shooter = enemy.GetComponent<EnemyShoot>();
        if (shooter != null)
            shooter.player = player;
    }

    // Método público para cambiar el bioma actual
    public void SetCurrentBiome(int biomeIndex)
    {
        currentBiome = biomeIndex;
        Debug.Log("Spawner cambiado a bioma: " + biomeIndex);
    }

    // Método para resetear dificultad (si se reinicia el nivel)
    public void ResetDifficulty()
    {
        currentDifficulty = 1f;
        gameStartTime = Time.time;
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        // Visualizar zona de spawn
        Gizmos.color = Color.red;
        Vector3 spawnCenter = player.position + Vector3.right * spawnDistance;
        Vector3 spawnSize = new Vector3(2f, spawnHeightMax - spawnHeightMin, 1f);

        Gizmos.DrawWireCube(spawnCenter + Vector3.up * ((spawnHeightMin + spawnHeightMax) / 2f), spawnSize);
    }
}