using UnityEngine;
using ArenaSurvival.Core;

namespace ArenaSurvival.Combat
{
    public class Weapon : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private float damage = 25f;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float range = 50f;
        [SerializeField] private LayerMask hitLayers;

        [Header("Visual References")]
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private PooledEffect hitEffectPrefab;
        [SerializeField] private int effectPoolSize = 10;

        private float _nextFireTime;
        private ObjectPool<PooledEffect> _effectPool;

        public bool CanShoot => Time.time >= _nextFireTime;

        private void Awake()
        {
            if (hitEffectPrefab != null)
            {
                GameObject poolContainer = new GameObject($"{hitEffectPrefab.name}_Pool");
                _effectPool = new ObjectPool<PooledEffect>(hitEffectPrefab, effectPoolSize, poolContainer.transform);
            }
        }

        public void Shoot()
        {
            if (!CanShoot) return;

            _nextFireTime = Time.time + fireRate;

            Vector3 shootOrigin = muzzlePoint != null ? muzzlePoint.position : transform.position;
            Vector3 shootDirection = muzzlePoint != null ? muzzlePoint.forward : transform.forward; // muzzlePoint atanmamışsa fallback

            if (Physics.Raycast(shootOrigin, shootDirection, out RaycastHit hitInfo, range, hitLayers))
            {
                if (hitInfo.collider.TryGetComponent<IDamageable>(out IDamageable target))
                {
                    target.TakeDamage(damage); // Player mı Enemy mi bilmiyoruz, IDamageable yeterli
                }

                SpawnHitEffect(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Debug.DrawLine(shootOrigin, hitInfo.point, Color.red, 1f);
            }
            else
            {
                Debug.DrawLine(shootOrigin, shootOrigin + shootDirection * range, Color.yellow, 0.5f);
            }
        }
        
        private void SpawnHitEffect(Vector3 position, Quaternion rotation)
        {
            if (_effectPool == null) return;

            PooledEffect effect = _effectPool.Get();
            effect.transform.SetPositionAndRotation(position, rotation);
            effect.SetPool(_effectPool);
        }
    }
}