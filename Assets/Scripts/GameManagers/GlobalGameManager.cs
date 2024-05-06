using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    [SerializeField] private int totalNumberOfFloors;
    [SerializeField] private string floorSceneName;
    [SerializeField] private string endSceneName;
    [field: SerializeField] public int CurrentFloorNumber { get; set; }

    private const string CurrentFloor = "currentFloor";

    public static GlobalGameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EventManager.OnExitPortalDetected += OnExitPortalDetected;
        EventManager.OnCombatSceneLoading += OnCombatSceneLoading;
        CurrentFloorNumber = PlayerPrefs.GetInt(CurrentFloor, 0);
    }


    private void OnDestroy()
    {
        EventManager.OnExitPortalDetected -= OnExitPortalDetected;
        EventManager.OnCombatSceneLoading -= OnCombatSceneLoading;
    }


    private void OnCombatSceneLoading(bool isLoaded)
    {
        if (PlayerInputController.Instance.State.isDead)
            EndDungeon();
    }

    private void OnExitPortalDetected(bool isExitPortal)
    {
        CurrentFloorNumber++;
        if (CurrentFloorNumber >= totalNumberOfFloors)
        {
            EndDungeon();
        }
        else
        {
            StartCoroutine(FloorComplete());
        }
    }

    [ContextMenu("End dungeon")]
    public void EndDungeon()
    {
        StartCoroutine(DungeonComplete());
    }

    private IEnumerator FloorComplete()
    {
        yield return new WaitUntil(() => !PlayerInputController.Instance.Stats.UpdateInProcess);
        DataManager.Instance.SaveEventTrigger("Global game manager FloorComplete");
        yield return new WaitForSeconds(1.5f);
        PlayerPrefs.SetInt(CurrentFloor, CurrentFloorNumber);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(floorSceneName);
    }

    private IEnumerator DungeonComplete()
    {
        yield return new WaitUntil(() => !PlayerInputController.Instance.Stats.UpdateInProcess);
        PlayerInputController.Instance.State.isDead = true;
        DataManager.Instance.SaveEventTrigger("Global game manager Dungeon complete");
        yield return new WaitForSeconds(1.5f);
        PlayerPrefs.DeleteKey(CurrentFloor);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(endSceneName);
    }
}