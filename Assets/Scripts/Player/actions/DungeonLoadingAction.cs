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
        var ctx = SceneManager.LoadSceneAsync(dungeonName);
        ctx.allowSceneActivation = false;
        StartCoroutine(WaitThenLoad(ctx));
    }

    private IEnumerator WaitThenLoad(AsyncOperation ctx)
    {
        loadingScreen.SetActive(true);
        var stats = PlayerInputController.Instance.Stats;
        stats.UpdateStatValue(StatValue.Mana, (int)stats.GetStatValue(StatValue.Mana).maxValue * 10);
        stats.UpdateStatValue(StatValue.Health, (int)stats.GetStatValue(StatValue.Health).maxValue * 10);
        DataManager.Instance.SaveEventTrigger("Dungeon loading action");
        yield return new WaitForSeconds(minWaitPeriod);
        yield return new WaitUntil(() => ctx.progress > 0.8f);
        loadingScreen.SetActive(false);
        ctx.allowSceneActivation = true;
    }
}