using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource m_AudioSource;
    [SerializeField] private List<AudioClip> sounds;

    private void Awake()
    {
        instance = this;
    }

    public void PlaySound(AudioClip audio, int volume)
    {
        
        
    }
}
