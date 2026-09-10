using UnityEngine;
using UnityEngine.AI;
using ArenaSurvival.Characters;

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
        [SerializeField] private float viewAngle = 90f; // Toplam açı (45 sağ, 45 sol)

        public NavMeshAgent Agent { get; private set; }
        public Transform Target { get; private set; }
        public StateMachine StateMachine { get; private set; }

        public ChaseState ChaseState { get; private set; }
        public AttackState AttackState { get; private set; }

        public float AttackRange => attackRange;
        public float AttackRate => attackRate;
        public float AttackDamage => attackDamage;

        protected override void Awake()
        {
            base.Awake();
            Agent = GetComponent<NavMeshAgent>();

            // FSM kurulumu
            StateMachine = new StateMachine();
            ChaseState = new ChaseState(this);
            AttackState = new AttackState(this);
        }

        private void Start()
        {
            // Oyuncuyu dinamik bul
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                Target = player.transform;
            }

            StateMachine.ChangeState(ChaseState);
        }

        private void Update()
        {
            if (IsDead) return;
            StateMachine.Update();
        }

        // Mülakatlarda sorulan Dot Product ile görüş konisi kontrolü
        public bool IsTargetInFieldOfView()
        {
            if (Target == null) return false;

            Vector3 directionToTarget = (Target.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToTarget);

            // Açının yarısının kosinüsü sınır eşiğimizdir
            float viewThreshold = Mathf.Cos((viewAngle * 0.5f) * Mathf.Deg2Rad);

            return dotProduct >= viewThreshold;
        }

        protected override void Die()
        {
            base.Die();
            Agent.isStopped = true;
            gameObject.SetActive(false); // İleride ObjectPool ile havuza dönecek
        }
    }
}