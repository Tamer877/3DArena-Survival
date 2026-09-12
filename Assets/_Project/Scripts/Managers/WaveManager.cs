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

        // Observer Pattern Event'leri (7. Adımdaki UI buna bağlanacak)
        public event Action<int> OnWaveStarted;
        public event Action<int> OnEnemiesRemainingChanged;

        private void Awake()
        {
            if (enemyPrefab != null)
            {
                GameObject poolContainer = new GameObject("Enemy_Pool");
                _enemyPool = new ObjectPool<Enemy>(enemyPrefab, initialPoolSize, poolContainer.transform);
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

                // Formül: Her dalgada düşman sayısı artar
                int enemiesToSpawn = baseEnemyCount + (CurrentWave - 1) * 3;
                ActiveEnemyCount = enemiesToSpawn;
                OnEnemiesRemainingChanged?.Invoke(ActiveEnemyCount);

                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(spawnInterval);
                }

                // Tüm düşmanlar ölene kadar bekle
                while (ActiveEnemyCount > 0)
                {
                    yield return null;
                }

                Debug.Log($"[WaveManager] Dalga {CurrentWave} Temizlendi! Yeni dalga bekleniyor...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        private void SpawnEnemy()
        {
            if (_enemyPool == null || spawnPoints.Length == 0) return;

            // Rastgele bir spawn noktası seç
            Transform randomPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPos = randomPoint.position;

            // NavMesh üzerinde geçerli en yakın noktayı bul
            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }

            Enemy enemy = _enemyPool.Get();

            // NavMeshAgent'ı doğru konuma taşımak için Warp kullanmak en güvenli yoldur
            if (enemy.Agent != null)
            {
                enemy.Agent.Warp(spawnPos);
            }
            else
            {
                enemy.transform.position = spawnPos;
            }

            enemy.Init(_enemyPool, _playerTransform);

            // Düşman öldüğünde kalan sayacı düşürmek için event dinle
            void HandleEnemyDeath()
            {
                enemy.OnDeath -= HandleEnemyDeath;
                ActiveEnemyCount--;
                OnEnemiesRemainingChanged?.Invoke(ActiveEnemyCount);
            }

            enemy.OnDeath += HandleEnemyDeath;
        }
    }
}