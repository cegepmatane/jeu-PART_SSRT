using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballExplosion : MonoBehaviour
{
    private AudioSource m_AudioSource;
    public AudioClip m_AudioClip;

    [SerializeField] private int m_ExplosionDamage;
    [SerializeField] private float m_ExplosionRadius;

    // Start is called before the first frame update
    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();

        PlayExplosionSound();
        ApplyExplodingDamage();
        //TODO Orienter les particules pour qu'elles se deplacent sur la normal du point touché et pas à travers

        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }

    private void PlayExplosionSound()
    {
        m_AudioSource.clip = m_AudioClip;
        m_AudioSource.spatialBlend = 1f;
        m_AudioSource.Play();
    }

    private void ApplyExplodingDamage()
    {
        Collider[] t_TouchedObjectsColliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
        foreach(Collider col in t_TouchedObjectsColliders)
        {
            if (col.gameObject.GetComponent<EnemyHealth>() != null)
                col.gameObject.GetComponent<EnemyHealth>().TakeDamage(m_ExplosionDamage);
        }

    }
}
