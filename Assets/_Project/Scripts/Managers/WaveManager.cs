using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using ArenaSurvival.AI;
using ArenaSurvival.Characters;
using ArenaSurvival.Core;

namespace ArenaSurvival.Managers
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Pool & Enemy Settings")]
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private int initialPoolSize = 20;

        [Header("Wave Progression")]
        [SerializeField] private int baseEnemyCount = 5;
        [SerializeField] private float spawnInterval = 1.5f;
        [SerializeField] private float timeBetweenWaves = 3f;

        [Header("Spawn Positions")]
        [SerializeField] private Transform[] spawnPoints;

        private ObjectPool<Enemy> _enemyPool;
        private Transform _playerTransform;

        public int CurrentWave { get; private set; } = 0;
        public int ActiveEnemyCount { get; private set; } = 0;

        public event Action<int> OnWaveStarted;
        public event Action<int> OnEnemiesRemainingChanged;

        private void Awake()
        {
            if (enemyPrefab != null)
            {
                GameObject poolContainer = new GameObject("Enemy_Pool");
                _enemyPool = new ObjectPool<Enemy>(enemyPrefab, initialPoolSize, poolContainer.transform);
                // pool nesneleri hierarchy'yi kirletmesin diye ayrı bir parent altında tutuyoruz
            }
        }

        private void Start()
        {
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                _playerTransform = player.transform;
                StartCoroutine(WaveLoopRoutine());
            }
            else
            {
                Debug.LogError("[WaveManager] Sahnede Player bulunamadı!");
            }
        }

        private IEnumerator WaveLoopRoutine()
        {
            while (true)
            {
                CurrentWave++;
                OnWaveStarted?.Invoke(CurrentWave);
                Debug.Log($"[WaveManager] Dalga {CurrentWave} Başladı!");

                // +3: her dalgada biraz daha zorlaşıyor
                int enemiesToSpawn = baseEnemyCount + (CurrentWave - 1) * 3;
                ActiveEnemyCount = enemiesToSpawn;
                OnEnemiesRemainingChanged?.Invoke(ActiveEnemyCount);

                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(spawnInterval);
                }

                while (ActiveEnemyCount > 0)
                {
                    yield return null; // her frame kontrol et, UI bunu güncelliyor zaten
                }

                Debug.Log($"[WaveManager] Dalga {CurrentWave} Temizlendi! Yeni dalga bekleniyor...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        private void SpawnEnemy()
        {
            if (_enemyPool == null || spawnPoints.Length == 0) return;

            Transform randomPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPos = randomPoint.position;

            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }

            Enemy enemy = _enemyPool.Get();

            // Warp: transform.position'a doğrudan atama NavMesh senkronizasyonunu bozar
            if (enemy.Agent != null)
            {
                enemy.Agent.Warp(spawnPos);
            }
            else
            {
                enemy.transform.position = spawnPos;
            }

            enemy.Init(_enemyPool, _playerTransform);

            void HandleEnemyDeath()
            {
                enemy.OnDeath -= HandleEnemyDeath; // tek seferlik, kendini çıkar
                ActiveEnemyCount--;
                OnEnemiesRemainingChanged?.Invoke(ActiveEnemyCount);
            }

            enemy.OnDeath += HandleEnemyDeath;
        }
    }
}