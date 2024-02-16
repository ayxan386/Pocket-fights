using UnityEngine;

public class UiSoundEffect : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private bool random;
    private int currentIndex;

    public void Play()
    {
        if (!PlayerInputController.Instance.InUiMode) return;

        if (random)
        {
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
        else
        {
            audioSource.PlayOneShot(clips[currentIndex]);
            currentIndex = (currentIndex + 1) % clips.Length;
        }
    }
}