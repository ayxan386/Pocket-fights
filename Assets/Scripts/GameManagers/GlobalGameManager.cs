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

        if (CurrentFloorNumber == 0)
        {
            DataManager.Instance.LoadPlayer();
        }
    }

    private void OnDestroy()
    {
        EventManager.OnExitPortalDetected -= OnExitPortalDetected;
    }

    private void OnExitPortalDetected(bool isExitPortal)
    {
        CurrentFloorNumber++;
        DataManager.Instance.SaveEventTrigger();
        if (CurrentFloorNumber >= totalNumberOfFloors)
        {
            EndDungeon();
        }
        else
        {
            StartCoroutine(FloorComplete());
        }
    }

    public void EndDungeon()
    {
        StartCoroutine(DungeonComplete());
    }

    private IEnumerator FloorComplete()
    {
        //Save data
        PlayerPrefs.SetInt(CurrentFloor, CurrentFloorNumber);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(floorSceneName);
    }

    private IEnumerator DungeonComplete()
    {
        //Save data
        PlayerPrefs.DeleteKey(CurrentFloor);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(endSceneName);
    }
}