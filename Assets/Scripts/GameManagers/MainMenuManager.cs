using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Starting run")]
    [SerializeField] private GameObject loadingScreenRef;

    [SerializeField] private float loadingDuration;
    [SerializeField] private string gameSceneName;

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        loadingScreenRef.SetActive(true);
        var loadSceneAsync = SceneManager.LoadSceneAsync(gameSceneName);
        loadSceneAsync.allowSceneActivation = false;
        StartCoroutine(DelayThenLoad(loadSceneAsync));
    }

    private IEnumerator DelayThenLoad(AsyncOperation loadSceneAsync)
    {
        yield return new WaitForSeconds(loadingDuration);
        yield return new WaitUntil(() => loadSceneAsync.progress > 0.79f);

        loadSceneAsync.allowSceneActivation = true;
    }
}