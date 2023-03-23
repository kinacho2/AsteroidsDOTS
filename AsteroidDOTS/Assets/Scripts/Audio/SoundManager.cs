using Asteroids.Data;
using Asteroids.Setup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AudioType = Asteroids.Data.AudioType;

namespace Asteroids.Audio 
{
    public class SoundManager : MonoBehaviour
    {
        private AudioSource[] Pool;
        private int _index;
        private AudioDataSO AudioDB;
        private AudioSource[] Loopeables;
        // Start is called before the first frame update
        public void Initialize(AudioDataSO audioDB)
        {
            Pool = new AudioSource[Configs.AUDIO_PLAY_COUNT];
            
            for (int i = 0; i< Pool.Length; i++)
            {
                Pool[i] = InstantiateAudioSource(i);
            }
            _index = 0;
            AudioDB = audioDB;
            Loopeables = new AudioSource[AudioDB.Sounds.Length];
        }

        private AudioSource InstantiateAudioSource(int index)
        {
            GameObject go = new GameObject("Audio_Source_" + index);
            go.transform.parent = transform;
            var source = go.AddComponent<AudioSource>();
            source.volume = 1;
            source.loop = false;
            source.playOnAwake = false;
            return source;
        }

        public void PlayClip(AudioType audioType, bool loop)
        {
            if (!loop)
            {
                var source = Pool[_index];
                _index = (_index + 1) % Pool.Length;
                source.Stop();
                source.clip = AudioDB.Sounds[((int)audioType)].clip;
                source.Play();
            }
            else
            {
                var i = (int)audioType;
                if (!Loopeables[i])
                    Loopeables[i] = InstantiateAudioSource(i);
                var source = Loopeables[i];
                if (!source.isPlaying)
                {
                    source.clip = AudioDB.Sounds[i].clip;
                    source.loop = true;
                    source.Play();
                }
            }
        }

        public void StopLoopeableClip(AudioType audioType)
        {
            var i = (int)audioType;
            if (!Loopeables[i]) return;
            Loopeables[i].Stop();
        }
    }
}