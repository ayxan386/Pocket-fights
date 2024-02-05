using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonLoadingAction : MonoBehaviour
{
    [SerializeField] private string dungeonName;
    [SerializeField] private float minWaitPeriod;
    [SerializeField] private GameObject loadingScreen;

    public void LoadDungeon()
    {
        PlayerInputController.Instance.SaveEventTrigger();
        var ctx = SceneManager.LoadSceneAsync(dungeonName);
        ctx.allowSceneActivation = false;
        StartCoroutine(WaitThenLoad(ctx));
    }

    private IEnumerator WaitThenLoad(AsyncOperation ctx)
    {
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(minWaitPeriod);
        yield return new WaitUntil(() => ctx.progress > 0.8f);
        loadingScreen.SetActive(false);
        ctx.allowSceneActivation = true;
    }
}