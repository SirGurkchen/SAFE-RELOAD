using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the Start Menu Screen UI.
/// </summary>
public class StartMenuUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject wasdUI;
    [SerializeField] private GameObject arrowUI;

    private void Start()
    {
        UpdateHighscore();
        ChangeControlsUI();
    }

    public void UpdateHighscore()
    {
        if (GameLogic.Instance != null)
        {
            highScore.text = "High Score: " + GameLogic.Instance.GetHighScore();
        }
        else
        {
            highScore.text = "High Score: 0";
        }
    }

    public void ChangeControlsUI()
    {
        string input = GameInput.Instance.inputScheme;
        buttonText.text = "Change Input: " + input;

        wasdUI.SetActive(input == "WASD");
        arrowUI.SetActive(input == "Arrows");
    }
}