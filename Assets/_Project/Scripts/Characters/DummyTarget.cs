using UnityEngine;

namespace ArenaSurvival.Characters
{
    public class DummyTarget : Character
    {
        protected override void Die()
        {
            base.Die();
            Debug.Log($"[DummyTarget] {gameObject.name} yok edildi!");
            Destroy(gameObject);
        }
    }
}