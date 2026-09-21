using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArenaSurvival.Characters;
using ArenaSurvival.Managers;

namespace ArenaSurvival.UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Wave UI")]
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI enemiesRemainingText;

        private Player _player;
        private WaveManager _waveManager;

        private void Awake()
        {
            _player = FindFirstObjectByType<Player>();
            _waveManager = FindFirstObjectByType<WaveManager>();
        }

        private void OnEnable()
        {
            if (_player != null)
            {
                _player.OnHealthChanged += UpdateHealthUI;
            }

            if (_waveManager != null)
            {
                _waveManager.OnWaveStarted += UpdateWaveUI;
                _waveManager.OnEnemiesRemainingChanged += UpdateEnemiesRemainingUI;
            }
        }

        private void OnDisable()
        {
            if (_player != null)
            {
                _player.OnHealthChanged -= UpdateHealthUI;
            }

            if (_waveManager != null)
            {
                _waveManager.OnWaveStarted -= UpdateWaveUI;
                _waveManager.OnEnemiesRemainingChanged -= UpdateEnemiesRemainingUI; // unsubscribe zorunlu, yoksa memory leak
            }
        }

        private void Start()
        {
            if (_player != null)
            {
                UpdateHealthUI(_player.CurrentHealth, _player.MaxHealth); // event gelene kadar slider boş kalmasın
            }
        }

        private void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }

            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {maxHealth}";
            }
        }

        private void UpdateWaveUI(int currentWave)
        {
            if (waveText != null)
            {
                waveText.text = $"Dalga: {currentWave}";
            }
        }

        private void UpdateEnemiesRemainingUI(int remainingEnemies)
        {
            if (enemiesRemainingText != null)
            {
                enemiesRemainingText.text = $"Kalan Düşman: {remainingEnemies}";
            }
        }
    }
}