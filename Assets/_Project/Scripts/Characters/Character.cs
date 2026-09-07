using System;
using UnityEngine;
using ArenaSurvival.Core;

namespace ArenaSurvival.Characters
{
    public abstract class Character : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] protected float maxHealth = 100f;

        public float CurrentHealth { get; protected set; }
        public float MaxHealth => maxHealth;
        public bool IsDead { get; protected set; }

        // Observer pattern için can değişim event'leri (UI doğrudan buna bağlanacak)
        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action OnDeath;

        protected virtual void Awake()
        {
            CurrentHealth = maxHealth;
            IsDead = false;
        }

        public virtual void TakeDamage(float damageAmount)
        {
            if (IsDead || damageAmount <= 0f) return;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - damageAmount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            if (IsDead) return;

            IsDead = true;
            OnDeath?.Invoke();
        }
    }
}