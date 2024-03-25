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

        Play(audioSource);
    }

    public void Play(AudioSource audioSourcePassed)
    {
        if (random)
        {
            audioSourcePassed.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
        else
        {
            audioSourcePassed.PlayOneShot(clips[currentIndex]);
            currentIndex = (currentIndex + 1) % clips.Length;
        }
    }

    public void PlayWithoutCheck()
    {
        Play(audioSource);
    }
}