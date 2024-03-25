using System.Collections;
using Data;
using UnityEngine;

namespace GameManagers
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private SoundList soundSource;

        private AudioSource audioSource;
        private bool isLastFinished = true;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            StartCoroutine(ContinusMusic());
        }

        private IEnumerator ContinusMusic()
        {
            while (true)
            {
                if (isLastFinished)
                {
                    audioSource.clip = soundSource.GiveMeNext();
                    isLastFinished = false;
                }

                audioSource.Play();
                yield return new WaitUntil(() => Application.isFocused && !audioSource.isPlaying);
                isLastFinished = true;
            }
        }
    }
}