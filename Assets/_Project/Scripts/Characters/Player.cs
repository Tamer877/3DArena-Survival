using UnityEngine;

namespace ArenaSurvival.Characters
{
    [RequireComponent(typeof(PlayerController))]
    public class Player : Character
    {
        private PlayerController _controller;

        protected override void Awake()
        {
            base.Awake();
            _controller = GetComponent<PlayerController>();
        }

        protected override void Die()
        {
            base.Die();
            
            // Karakter öldüğünde kontrolleri kapatıyoruz
            _controller.enabled = false;
            Debug.Log("[Player] Oyuncu öldü. Game Over tetiklenebilir.");
        }
    }
}