using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombatInitiation : MonoBehaviour
{
    [SerializeField] private Material colorSource;
    [SerializeField] private Transform detectionBody;
    [SerializeField] private GameObject wholeScene;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask mobLayer;
    [SerializeField] private List<MobController> mobsToActivate;
    [SerializeField] private GameObject unloadingScreen;

    [Header("Detection LVL co-relation")]
    [SerializeField] private float conversionFactor;

    [SerializeField] private float radiusIncrementRate = 1.01f;
    [SerializeField] private float initialCheckRadius;

    public bool IsCombatScene;
    private Coroutine initiationCorr;
    public List<MobController> mobs { get; private set; }

    public static PlayerCombatInitiation Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var color = colorSource.color;
        color.a = 0;
        colorSource.color = color;
        StartCoroutine(MobCheck());
        EventManager.OnPlayerCoreUpdate += OnPlayerCoreUpdate;
    }

    private void OnDestroy()
    {
        EventManager.OnPlayerCoreUpdate -= OnPlayerCoreUpdate;
    }

    private IEnumerator MobCheck()
    {
        while (PlayerInputController.Instance != null)
        {
            var allMobs = Physics.SphereCastAll(transform.position, checkRadius, Vector3.forward, 0, mobLayer);
            var displayManager = DetailDisplayManager.Instance;
            if (allMobs.Length == 0)
            {
            }

            displayManager.TurnOff();
            MobController displayedMob = null;
            foreach (var mob in allMobs)
            {
                if (mob.transform.TryGetComponent(out displayedMob))
                {
                    displayManager.DisplayNextMob(displayedMob);
                }

                if (mob.transform.TryGetComponent(out InteractableDescriptionData data))
                {
                    displayManager.DisplayGeneric(data.title, data.icon, data.smallIcons);
                }
            }

            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => !IsCombatScene);
        }
    }

    public void StartInitiation(float duration)
    {
        if (initiationCorr != null) return;
        initiationCorr = StartCoroutine(ColorChange(duration));
    }

    private IEnumerator ColorChange(float duration)
    {
        var startTime = Time.time;
        var t = 0f;
        var color = colorSource.color;
        while (startTime + duration > Time.time)
        {
            color.a = Mathf.Lerp(0, 1, t / duration);
            colorSource.color = color;
            yield return null;
            t += Time.deltaTime;
        }

        color = colorSource.color;
        color.a = 0;
        colorSource.color = color;
        FindAllMobs();
        initiationCorr = null;
        yield return new WaitForSeconds(0.2f);
        if (mobs.Count <= 0) yield break;

        LoadingCombatScene();
        yield return new WaitForSeconds(0.2f);
        EventManager.OnCombatSceneLoading?.Invoke(true);
    }

    private void FindAllMobs()
    {
        var allMobs = Physics.SphereCastAll(transform.position, checkRadius, Vector3.forward, 0, mobLayer);
        mobs = new List<MobController>();
        foreach (var mob in allMobs)
        {
            if (mob.transform.TryGetComponent(out MobController controller) && mobs.Count < 6)
            {
                mobs.Add(controller);
            }
            else if (mobs.Count == 0 && mob.transform.TryGetComponent(out EntityController entity))
            {
                entity.Interact();
            }
        }
    }

    private void DeactivateAllMobs()
    {
        var allMobsInScene = FindObjectsOfType<MobController>();

        mobsToActivate.Clear();
        foreach (var mobInScene in allMobsInScene)
        {
            if (!mobs.Contains(mobInScene))
            {
                mobInScene.gameObject.SetActive(false);
                mobsToActivate.Add(mobInScene);
            }
        }
    }

    public void UnloadCombatScene()
    {
        SceneManager.UnloadSceneAsync("CombatScene");
        StartCoroutine(WaitThenLoad());
    }

    private IEnumerator WaitThenLoad()
    {
        DataManager.Instance.SaveEventTrigger("Combat mode initiator");
        unloadingScreen.SetActive(true);
        EventManager.OnCombatSceneLoading?.Invoke(false);
        wholeScene.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        unloadingScreen.SetActive(false);
        mobsToActivate.ForEach(mob => mob.gameObject.SetActive(true));
        IsCombatScene = false;
    }

    private void LoadingCombatScene()
    {
        wholeScene.SetActive(false);
        DeactivateAllMobs();
        IsCombatScene = true;
        SceneManager.LoadScene("CombatScene", LoadSceneMode.Additive);
    }

    private void OnPlayerCoreUpdate(int obj)
    {
        checkRadius = initialCheckRadius * Mathf.Pow(radiusIncrementRate, PlayerInputController.Instance.Stats.Level);
        UpdateDetectionBody();
    }

    [ContextMenu("Update detection body")]
    public void UpdateDetectionBody()
    {
        var scale = detectionBody.localScale;
        scale.x = checkRadius * conversionFactor;
        scale.z = checkRadius * conversionFactor;
        detectionBody.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}