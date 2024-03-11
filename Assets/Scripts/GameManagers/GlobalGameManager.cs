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
        CurrentFloorNumber = PlayerPrefs.GetInt(CurrentFloor, 0);
    }

    private void OnDestroy()
    {
        EventManager.OnExitPortalDetected -= OnExitPortalDetected;
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
        DataManager.Instance.SaveEventTrigger("Global game manager FloorComplete");
        yield return new WaitForSeconds(1.5f);
        PlayerPrefs.SetInt(CurrentFloor, CurrentFloorNumber);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(floorSceneName);
    }

    private IEnumerator DungeonComplete()
    {
        DataManager.Instance.SaveEventTrigger("Global game manager Dungeon complete");
        yield return new WaitForSeconds(1.5f);
        PlayerPrefs.DeleteKey(CurrentFloor);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(endSceneName);
    }
}