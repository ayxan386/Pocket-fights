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

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private IEnumerator Start()
        {
            while (true)
            {
                audioSource.clip = soundSource.GiveMeNext();
                audioSource.Play();
                yield return new WaitUntil(() => !audioSource.isPlaying);
            }
        }
    }
}