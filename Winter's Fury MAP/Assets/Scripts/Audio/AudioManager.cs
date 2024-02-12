using System;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] sounds;
        
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            
            foreach (var s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
            }
        }

        public void Play(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            s.source.Play();
        }

        public void StopAll()
        {
            foreach (var s in sounds)
            {
                s.source.Stop();
            }
        }
    }
}
