using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private SerializedDictionary<SoundType, AudioClip> audioClips = new SerializedDictionary<SoundType, AudioClip>();
    [SerializeField] private List<AudioSource> audioSources;
    

    public void PlaySound(SoundType soundType)
    {
        if (audioClips.TryGetValue(soundType, out AudioClip clip))
        {
            var volume = 0.5f;
            if (audioSources[0].isPlaying)
            {
                audioSources[1].PlayOneShot(clip, volume);
            }
            else
            {
                audioSources[0].PlayOneShot(clip, volume);
            }
        }
    }
}

public enum SoundType
{
    EnemySpawn,
    Shoot,
    PlayerHit,
    EnemyDie
}
