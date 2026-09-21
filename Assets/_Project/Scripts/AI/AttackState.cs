using UnityEngine;
using ArenaSurvival.Core;

namespace ArenaSurvival.AI
{
    public class AttackState : IState
    {
        private readonly Enemy _enemy;
        private float _nextAttackTime;

        public AttackState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _enemy.Agent.isStopped = true; // saldırırken yürüme
        }

        public void Update()
        {
            if (_enemy.Target == null) return;

            Vector3 direction = (_enemy.Target.position - _enemy.transform.position).normalized;
            direction.y = 0; // sadece yatay eksende dön
            if (direction.sqrMagnitude > 0.001f)
            {
                _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
            }

            float distance = Vector3.Distance(_enemy.transform.position, _enemy.Target.position);
            if (distance > _enemy.AttackRange)
            {
                _enemy.StateMachine.ChangeState(_enemy.ChaseState);
                return; // state değişti, bu frame'in geri kalanini çalıştırma
            }

            if (Time.time >= _nextAttackTime)
            {
                PerformAttack();
                _nextAttackTime = Time.time + _enemy.AttackRate;
            }
        }

        private void PerformAttack()
        {
            if (_enemy.Target.TryGetComponent<IDamageable>(out IDamageable playerDamageable))
            {
                playerDamageable.TakeDamage(_enemy.AttackDamage);
                Debug.Log($"[Enemy] Oyuncuya {_enemy.AttackDamage} hasar vuruldu!");
            }
        }

        public void Exit() { }
    }
}