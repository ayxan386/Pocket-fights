using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SoundList", menuName = "SoundList", order = 0)]
    public class SoundList : ScriptableObject
    {
        [SerializeField] private AudioClip[] clips;
        private int lastIndex;

        public AudioClip GiveMeRandomClip()
        {
            return clips[Random.Range(0, clips.Length)];
        }

        public AudioClip GiveMeNext()
        {
            var res = clips[lastIndex];
            lastIndex = (lastIndex + 1) % clips.Length;

            return res;
        }
    }
}