using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Handles the logic of the Start Menu Screen.
/// This includes the functions of each button.
/// Also tells the Game Logic what input scheme is currently in use.
/// </summary>
public class StartMenuLogic : MonoBehaviour
{
    [SerializeField] private StartMenuUIManager UIManager;

    private void Awake()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void Update()
    {
        CheckForMute();
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        CheckForMute();
    }

    /// <summary>
    /// Starts the game on Play Button press.
    /// First loads the Loading Screen Scene.
    /// </summary>
    public void PlayGame()
    {
        AudioManager.Instance.PlaySound(AudioManager.SoundType.Click);
        LoadingSceneLogic.nextScene = "GameScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySound(AudioManager.SoundType.Click);
        Application.Quit();
    }

    /// <summary>
    /// Checks if the mute button is pressed.
    /// If the button is pressed, tells Audio Manager to mute music.
    /// </summary>
    public void CheckForMute()
    {
        bool muteIsPressed = GameInput.Instance.MuteMusicInput();

        if (muteIsPressed)
        {
            AudioManager.Instance.PlayerDisableEnableMusic();
        }
    }

    /// <summary>
    /// Tells Game Input to change the selected input scheme on button press.
    /// Tells UI to change the shown controls and button text.
    /// </summary>
    public void ChangeControlScheme()
    {
        AudioManager.Instance.PlaySound(AudioManager.SoundType.Click);

        if (GameInput.Instance.inputScheme == "WASD")
        {
            GameInput.Instance.SwitchInput("Arrows");
        }
        else
        {
            GameInput.Instance.SwitchInput("WASD");
        }
        UIManager.ChangeControlsUI();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }
}