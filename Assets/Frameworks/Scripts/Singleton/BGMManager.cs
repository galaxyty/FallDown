using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : BaseSingleton<BGMManager>
{
    private AudioSource m_Source = null;

    // 초기화.
    public void Initialization()
    {
        // 오디오소스 추가.
        if (m_Source == null)
        {
            m_Source = GetComponent<AudioSource>();
        }
    }

    // BGM 재생.
    public void Play(string name)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audios/BGM/{name}");

        if (clip == null)
        {
            return;
        }

        m_Source.clip = clip;
        m_Source.Play();
    }
    
    // BGM 정지.
    public void Stop()
    {
        m_Source.Stop();
    }

    // BGM 일시 정지.
    public void Pause()
    {
        m_Source.Pause();
    }
}
