using UnityEngine;

namespace ArenaSurvival.AI
{
    public class ChaseState : IState
    {
        private readonly Enemy _enemy;

        public ChaseState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _enemy.Agent.isStopped = false;
        }

        public void Update()
        {
            if (_enemy.Target == null) return;

            _enemy.Agent.SetDestination(_enemy.Target.position);

            float distance = Vector3.Distance(_enemy.transform.position, _enemy.Target.position);
            if (distance <= _enemy.AttackRange)
            {
                _enemy.StateMachine.ChangeState(_enemy.AttackState);
            }
        }

        public void Exit()
        {
            _enemy.Agent.isStopped = true;
        }
    }
}