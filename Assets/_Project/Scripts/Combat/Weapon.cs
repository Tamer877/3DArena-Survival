using UnityEngine;
using ArenaSurvival.Core;

namespace ArenaSurvival.Combat
{
    public class Weapon : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private float damage = 25f;
        [SerializeField] private float fireRate = 0.2f; // Saniyede 5 mermi
        [SerializeField] private float range = 50f;
        [SerializeField] private LayerMask hitLayers;

        [Header("Visual References")]
        [SerializeField] private Transform muzzlePoint;

        private float _nextFireTime;

        public bool CanShoot => Time.time >= _nextFireTime;

        public void Shoot()
        {
            if (!CanShoot) return;

            _nextFireTime = Time.time + fireRate;

            Vector3 shootOrigin = muzzlePoint != null ? muzzlePoint.position : transform.position;
            Vector3 shootDirection = muzzlePoint != null ? muzzlePoint.forward : transform.forward;

            if (Physics.Raycast(shootOrigin, shootDirection, out RaycastHit hitInfo, range, hitLayers))
            {
                // Hedef hasar alabilir mi kontrol et
                if (hitInfo.collider.TryGetComponent<IDamageable>(out IDamageable target))
                {
                    target.TakeDamage(damage);
                }

                // Debug amaçlı vuruş çizgisini sahne ekranında göster (1 saniye)
                Debug.DrawLine(shootOrigin, hitInfo.point, Color.red, 1f);
            }
            else
            {
                // Boşa ateş edilirse ışını tam menzilde çiz
                Debug.DrawLine(shootOrigin, shootOrigin + shootDirection * range, Color.yellow, 0.5f);
            }
        }
    }
}