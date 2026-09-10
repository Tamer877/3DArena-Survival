using System.Collections;
using UnityEngine;
using ArenaSurvival.Core;

namespace ArenaSurvival.Combat
{
    public class PooledEffect : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.5f;
        private ObjectPool<PooledEffect> _originPool;

        public void SetPool(ObjectPool<PooledEffect> pool)
        {
            _originPool = pool;
        }

        private void OnEnable()
        {
            StartCoroutine(DeactivateRoutine());
        }

        private IEnumerator DeactivateRoutine()
        {
            yield return new WaitForSeconds(lifetime);

            if (_originPool != null)
            {
                _originPool.ReturnToPool(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}