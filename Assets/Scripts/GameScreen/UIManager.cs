using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI of the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image healthBar;

    public void ResetAmmo(int maxAmmo)
    {
        ammoText.text = maxAmmo + " | " + maxAmmo;
    }

    public void RefreshAmmo(int maxAmmo, int currentAmmo)
    {
        ammoText.text = currentAmmo + " | " + maxAmmo;
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }

    public void UpdateHealthBar(float fillAmount)
    {
        healthBar.fillAmount = fillAmount;
    }
}