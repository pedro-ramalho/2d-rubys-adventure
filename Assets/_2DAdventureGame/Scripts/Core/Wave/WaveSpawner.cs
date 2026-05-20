using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] private List<Wave> waves;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float minSpawnSpacing = 3f;
    [SerializeField] private float minDistanceFromPlayer = 4f;
    [SerializeField] private int maxSpawnAttempts = 30;

    [Header("Telegraph")]
    [SerializeField] private GameObject telegraphPrefab;
    [SerializeField] private float telegraphDuration = 0.6f;
    [SerializeField] private float spawnInterval = 0.3f;

    [Header("Timing")]
    [SerializeField] private float initialDelay = 2f;
    [SerializeField] private float breatherDuration = 2f;

    [Header("References")]
    [SerializeField] private Player player;

    private readonly List<GameObject> aliveEnemies = new();

    public event Action OnAllWavesCleared;

    void Start()
    {
        if (player == null) player = FindAnyObjectByType<Player>();

        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        yield return new WaitForSeconds(initialDelay);

        foreach (Wave wave in waves)
        {
            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitUntil(IsCurrentWaveCleared);
            yield return new WaitForSeconds(breatherDuration);
        }

        OnAllWavesCleared?.Invoke();
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.count; i++)
        {
            yield return StartCoroutine(SpawnOne(wave.enemyPrefab));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnOne(GameObject enemyPrefab)
    {
        Transform point = PickSpawnPoint();
        if (point == null) yield break;

        if (telegraphPrefab != null)
            Instantiate(telegraphPrefab, point.position, Quaternion.identity);

        yield return new WaitForSeconds(telegraphDuration);

        GameObject enemy = Instantiate(enemyPrefab, point.position, Quaternion.identity);
        aliveEnemies.Add(enemy);
    }

    Transform PickSpawnPoint()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Transform candidate = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            if (IsTooCloseToPlayer(candidate.position)) continue;
            if (IsTooCloseToAlive(candidate.position)) continue;

            return candidate;
        }

        return null;
    }

    bool IsTooCloseToPlayer(Vector3 pos)
    {
        if (player == null) return false;
        return Vector2.Distance(pos, player.transform.position) < minDistanceFromPlayer;
    }

    bool IsTooCloseToAlive(Vector3 pos)
    {
        foreach (GameObject e in aliveEnemies)
        {
            if (e == null) continue;

            if (Vector2.Distance(pos, e.transform.position) < minSpawnSpacing)
                return true;
        }

        return false;
    }

    bool IsCurrentWaveCleared()
    {
        aliveEnemies.RemoveAll(e => e == null);
        return aliveEnemies.Count == 0;
    }
}
