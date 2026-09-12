using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ArenaSurvival.Characters;

namespace ArenaSurvival.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;

        private Player _player;

        private void Awake()
        {
            _player = FindFirstObjectByType<Player>();

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (_player != null)
            {
                _player.OnDeath += ShowGameOver;
            }
        }

        private void OnDisable()
        {
            if (_player != null)
            {
                _player.OnDeath -= ShowGameOver;
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(RestartGame);
            }
        }

        private void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        private void RestartGame()
        {
            // Aktif sahneyi yeniden yükle
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}