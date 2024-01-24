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
            StartCoroutine(DungeonComplete());
        }
        else
        {
            StartCoroutine(FloorComplete());
        }
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