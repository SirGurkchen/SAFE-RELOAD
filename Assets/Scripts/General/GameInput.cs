using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// Handles the player input.
/// Includes the changing of the input scheme.
/// </summary>
public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    private InputActions inputActions;

    public string inputScheme;

    private InputAction currentMoveAction;
    private InputAction currentReloadAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        inputActions = new InputActions();

        if (string.IsNullOrEmpty(inputScheme))
        {
            inputScheme = "WASD";
        }

        EnableInput();
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            EnableInput();
        }
        else if (scene.name == "StartMenuScene")
        {
            inputActions.Player.MuteMusic.Enable();
        }
    }

    /// <summary>
    /// Gives the moving direction as a normalized Vec2
    /// </summary>
    /// <returns>Normalized Vec2</returns>
    public Vector2 PlayerMovementNormalized()
    {
        Vector2 inputVec = currentMoveAction.ReadValue<Vector2>();
        inputVec = inputVec.normalized;
        return inputVec;
    }

    public bool PlayerShoot()
    {
        return inputActions.Player.Shoot.WasPressedThisFrame();
    }

    public bool PlayerReload()
    {
        return currentReloadAction.WasPressedThisFrame();
    }

    public void DisableInput()
    {
        inputActions.Player.Disable();
    }

    public bool MuteMusicInput()
    {
        return inputActions.Player.MuteMusic.WasPressedThisFrame();
    }

    /// <summary>
    /// Changes the selected input scheme to the given input
    /// </summary>
    /// <param name="input">The new input scheme</param>
    public void SwitchInput(string input)
    {
        inputScheme = input;
        if (inputScheme == "WASD")
        {
            inputActions.Player.MoveArrow.Disable();
            inputActions.Player.Move.Enable();

            inputActions.Player.ReloadAlt.Disable();
            inputActions.Player.Reload.Enable();

            currentMoveAction = inputActions.Player.Move;
            currentReloadAction = inputActions.Player.Reload;
        }
        else if (inputScheme == "Arrows")
        {
            inputActions.Player.Move.Disable();
            inputActions.Player.MoveArrow.Enable();

            inputActions.Player.Reload.Disable();
            inputActions.Player.ReloadAlt.Enable();

            currentMoveAction = inputActions.Player.MoveArrow;
            currentReloadAction = inputActions.Player.ReloadAlt;
        }
    }

    private void EnableInput()
    {
        inputActions.Player.Shoot.Enable();
        inputActions.Player.MuteMusic.Enable();
        SwitchInput(inputScheme);
    }
}