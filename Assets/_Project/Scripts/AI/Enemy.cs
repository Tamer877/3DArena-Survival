using UnityEngine;
using UnityEngine.AI;
using ArenaSurvival.Characters;
using ArenaSurvival.Core;

namespace ArenaSurvival.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : Character
    {
        [Header("Combat Settings")]
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackRate = 1f;

        [Header("Vision (Dot Product)")]
        [SerializeField] private float viewAngle = 90f;

        public NavMeshAgent Agent { get; private set; }
        public Transform Target { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public ChaseState ChaseState { get; private set; }
        public AttackState AttackState { get; private set; }

        public float AttackRange => attackRange;
        public float AttackRate => attackRate;
        public float AttackDamage => attackDamage;

        private ObjectPool<Enemy> _originPool;

        protected override void Awake()
        {
            base.Awake();
            Agent = GetComponent<NavMeshAgent>();

            StateMachine = new StateMachine();
            ChaseState = new ChaseState(this);
            AttackState = new AttackState(this); // her enemy kendi state instance'larına sahip
        }

        public void Init(ObjectPool<Enemy> pool, Transform targetTransform)
        {
            _originPool = pool;
            Target = targetTransform;
            ResetEnemy();
        }

        public void ResetEnemy()
        {
            CurrentHealth = maxHealth;
            IsDead = false;

            if (Agent != null)
            {
                Agent.isStopped = false;
            }

            StateMachine?.ChangeState(ChaseState);
        }

        private void Update()
        {
            if (IsDead) return;
            StateMachine.Update();
        }

        public bool IsTargetInFieldOfView()
        {
            if (Target == null) return false;

            Vector3 directionToTarget = (Target.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToTarget);
            float viewThreshold = Mathf.Cos((viewAngle * 0.5f) * Mathf.Deg2Rad); // Vector3.Angle yerine dot product: arccos hesabı yok

            return dotProduct >= viewThreshold;
        }

        protected override void Die()
        {
            base.Die();

            if (Agent != null && Agent.isOnNavMesh) // isOnNavMesh kontrolü olmadan exception alabiliriz
            {
                Agent.isStopped = true;
            }

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