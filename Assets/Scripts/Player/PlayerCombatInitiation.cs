using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombatInitiation : MonoBehaviour
{
    [SerializeField] private Material colorSource;

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
            color.a = Mathf.Lerp(0, 1, t);
            colorSource.color = color;
            yield return null;
            t += Time.deltaTime;
        }

        yield return new WaitForSeconds(0.2f);
        color = colorSource.color;
        color.a = 0;
        colorSource.color = color;
        SceneManager.LoadScene("CombatScene");
    }
}