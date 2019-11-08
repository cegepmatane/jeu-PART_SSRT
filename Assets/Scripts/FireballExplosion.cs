using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballExplosion : MonoBehaviour
{
    private AudioSource m_AudioSource;
    public AudioClip m_AudioClip;

    // Start is called before the first frame update
    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();

        m_AudioSource.clip = m_AudioClip;
        m_AudioSource.spatialBlend = 1f;
        m_AudioSource.Play();

        Destroy(gameObject, 2f);
    }
}
