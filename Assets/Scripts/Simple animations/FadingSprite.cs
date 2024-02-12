using System.Collections;
using UnityEngine;

public class FadingSprite : MonoBehaviour
{
    [SerializeField] private float fadeDuration;
    [SerializeField] private SpriteRenderer rendRef;
    [SerializeField] private Color targetColor;

    private IEnumerator Start()
    {
        Destroy(gameObject, fadeDuration);
        var time = 0f;
        var transformEulerAngles = transform.eulerAngles;
        transformEulerAngles.y = -90;
        transform.eulerAngles = transformEulerAngles;
        while (true)
        {
            yield return null;
            time += Time.deltaTime;
            var rendRefColor = rendRef.color;
            rendRef.color = Color.Lerp(rendRefColor, targetColor, time / fadeDuration);
        }
    }
}