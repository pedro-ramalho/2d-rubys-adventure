using System;
using System.Collections;
using System.Collections.Generic;
using AdventureGame.Core.Quests;
using AdventureGame.Entities.Player;
using UnityEngine;

namespace AdventureGame.Core.Wave
{
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Waves")]
        [SerializeField]
        private List<Wave> m_Waves;

        [Header("Spawn Points")]
        [SerializeField]
        private Transform[] m_SpawnPoints;

        [SerializeField]
        private float m_MinSpawnSpacing = 3f;

        [SerializeField]
        private float m_MinDistanceFromPlayer = 4f;

        [SerializeField]
        private int m_MaxSpawnAttempts = 30;

        [Header("Telegraph")]
        [SerializeField]
        private GameObject m_TelegraphPrefab;

        [SerializeField]
        private float m_TelegraphDuration = 0.6f;

        [SerializeField]
        private float m_SpawnInterval = 0.3f;

        [Header("Timing")]
        [SerializeField]
        private float m_InitialDelay = 2f;

        [SerializeField]
        private float m_BreatherDuration = 2f;

        [Header("Trigger")]
        [SerializeField]
        private QuestData m_TriggerQuestData;

        private readonly List<GameObject> m_AliveEnemies = new();
        private bool m_HasStarted;
        private Quest m_Quest;

        public event Action OnAllWavesCleared;

        void Start()
        {
            if (Player.Instance != null)
                Player.Instance.OnDied += HandlePlayerDied;

            if (QuestManager.Instance == null)
                return;

            m_Quest = QuestManager.Instance.Get(m_TriggerQuestData);
            if (m_Quest != null)
                m_Quest.OnPhaseChanged += HandlePhaseChanged;
        }

        void OnDestroy()
        {
            if (Player.Instance != null)
                Player.Instance.OnDied -= HandlePlayerDied;

            if (m_Quest != null)
                m_Quest.OnPhaseChanged -= HandlePhaseChanged;
        }

        void HandlePlayerDied() => StopAllCoroutines();

        void HandlePhaseChanged(Quest q)
        {
            if (m_HasStarted || q.Phase != QuestPhase.During)
                return;

            m_HasStarted = true;

            StartCoroutine(RunWaves());
        }

        IEnumerator RunWaves()
        {
            yield return new WaitForSeconds(m_InitialDelay);

            foreach (Wave wave in m_Waves)
            {
                yield return StartCoroutine(SpawnWave(wave));
                yield return new WaitUntil(IsWaveCleared);
                yield return new WaitForSeconds(m_BreatherDuration);
            }

            yield return StartCoroutine(SpawnBonusWave());
            yield return new WaitUntil(IsWaveCleared);

            OnAllWavesCleared?.Invoke();
        }

        IEnumerator SpawnWave(Wave wave)
        {
            for (int i = 0; i < wave.EnemyCount; i++)
            {
                yield return StartCoroutine(SpawnOne(wave.EnemyPrefab));
                yield return new WaitForSeconds(m_SpawnInterval);
            }
        }

        IEnumerator SpawnBonusWave()
        {
            if (m_Waves.Count == 0)
                yield break;

            GameObject prefab = m_Waves[^1].EnemyPrefab;

            foreach (Transform point in m_SpawnPoints)
            {
                yield return StartCoroutine(SpawnAt(point, prefab));
                yield return new WaitForSeconds(m_SpawnInterval);
            }
        }

        IEnumerator SpawnOne(GameObject enemyPrefab)
        {
            Transform point = PickSpawnPoint();
            if (point == null)
                yield break;

            yield return StartCoroutine(SpawnAt(point, enemyPrefab));
        }

        IEnumerator SpawnAt(Transform point, GameObject enemyPrefab)
        {
            GameObject telegraph =
                m_TelegraphPrefab != null
                    ? Instantiate(m_TelegraphPrefab, point.position, Quaternion.identity)
                    : null;

            yield return new WaitForSeconds(m_TelegraphDuration);

            if (telegraph != null)
                Destroy(telegraph);

            GameObject enemy = Instantiate(enemyPrefab, point.position, Quaternion.identity);
            m_AliveEnemies.Add(enemy);
        }

        Transform PickSpawnPoint()
        {
            for (int attempt = 0; attempt < m_MaxSpawnAttempts; attempt++)
            {
                Transform candidate = m_SpawnPoints[
                    UnityEngine.Random.Range(0, m_SpawnPoints.Length)
                ];

                if (IsTooCloseToPlayer(candidate.position))
                    continue;
                if (IsTooCloseToAlive(candidate.position))
                    continue;

                return candidate;
            }

            return null;
        }

        bool IsTooCloseToPlayer(Vector3 pos)
        {
            Player p = Player.Instance;
            if (p == null)
                return false;
            return Vector2.Distance(pos, p.transform.position) < m_MinDistanceFromPlayer;
        }

        bool IsTooCloseToAlive(Vector3 pos)
        {
            foreach (GameObject e in m_AliveEnemies)
            {
                if (e == null)
                    continue;

                if (Vector2.Distance(pos, e.transform.position) < m_MinSpawnSpacing)
                    return true;
            }

            return false;
        }

        bool IsWaveCleared()
        {
            for (int i = m_AliveEnemies.Count - 1; i >= 0; i--)
                if (m_AliveEnemies[i] == null)
                    m_AliveEnemies.RemoveAt(i);

            return m_AliveEnemies.Count == 0;
        }
    }
}
