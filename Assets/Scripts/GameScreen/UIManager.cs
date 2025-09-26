using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI of the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Image _healthBar;
    [SerializeField] private TextMeshProUGUI _weaponText;

    public void ResetAmmo(int maxAmmo)
    {
        _ammoText.text = maxAmmo + " | " + maxAmmo;
    }

    public void RefreshAmmo(int maxAmmo, int currentAmmo)
    {
        _ammoText.text = currentAmmo + " | " + maxAmmo;
    }

    public void UpdateScore(int newScore)
    {
        _scoreText.text = "Score: " + newScore;
    }

    public void UpdateHealthBar(float fillAmount)
    {
        _healthBar.fillAmount = fillAmount;
    }

    public void SetWeaponText(string weaponText)
    {
        _weaponText.text = weaponText; 
    }
}