using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The Loading Screen of the game.
/// Has an artifical loading time.
/// </summary>
public class LoadingSceneLogic : MonoBehaviour
{
    // Time of the loading screen.
    [SerializeField] private const float LOADING_TIMER = 1f;

    public static string nextScene;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        yield return new WaitForSeconds(LOADING_TIMER);

        SceneManager.LoadScene(nextScene);
    }
}