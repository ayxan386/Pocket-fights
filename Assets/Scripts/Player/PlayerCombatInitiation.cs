using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombatInitiation : MonoBehaviour
{
    [SerializeField] private Material colorSource;
    [SerializeField] private GameObject wholeScene;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask mobLayer;

    public bool IsCombatScene;
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
    }

    public void StartInitiation(float duration)
    {
        StartCoroutine(ColorChange(duration));
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
        yield return new WaitForSeconds(0.2f);
        if (mobs.Count <= 0) yield break;
        
        LoadingCombatScene();
        yield return new WaitForSeconds(0.2f);
        EventManager.OnCombatSceneLoading?.Invoke(true);
    }

    private void FindAllMobs()
    {
        var allMobs = Physics.SphereCastAll(transform.position, checkRadius, Vector3.forward, 0, mobLayer);
        print($"Found {allMobs.Length} mobs");
        mobs = new List<MobController>();
        foreach (var mob in allMobs)
        {
            if (mob.transform.TryGetComponent(out MobController controller))
            {
                mobs.Add(controller);
            }
        }
    }

    public void UnloadCombatScene()
    {
        wholeScene.SetActive(true);
        IsCombatScene = false;
        SceneManager.UnloadSceneAsync("CombatScene");
        EventManager.OnCombatSceneLoading?.Invoke(false);
    }

    private void LoadingCombatScene()
    {
        wholeScene.SetActive(false);
        IsCombatScene = true;
        SceneManager.LoadScene("CombatScene", LoadSceneMode.Additive);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, checkRadius);
    }
}