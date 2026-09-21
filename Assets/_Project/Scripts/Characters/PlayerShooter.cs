using UnityEngine;
using ArenaSurvival.Combat;

namespace ArenaSurvival.Characters
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Weapon currentWeapon;

        private void Update()
        {
            // Sol fare tıklandığında veya basılı tutulduğunda ateş et
            if (Input.GetButton("Fire1"))
            {
                if (currentWeapon != null && currentWeapon.CanShoot)
                {
                    currentWeapon.Shoot();
                }
            }
        }

        public void SetWeapon(Weapon newWeapon)
        {
            currentWeapon = newWeapon;
        }
    }
}