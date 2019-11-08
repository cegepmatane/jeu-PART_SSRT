using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float travellingSpeed = 50f;
    public float fireballRange = 50f;
    private Camera m_Cam;
    private RaycastHit m_Hit;
    private float m_HitThreshold = 1f;
    private bool m_Trajectoire;

    private AudioSource m_Audio;
    [SerializeField] private AudioClip m_Explosion;

    private void Awake()
    {
        m_Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        m_Audio = GetComponent<AudioSource>();
        m_Audio.clip = m_Explosion;
    }

    void Start()
    {
        m_Trajectoire = Physics.Raycast(m_Cam.transform.position, m_Cam.transform.forward, out m_Hit, fireballRange) ? true : false;
        Destroy(gameObject, 2f);
    }

    private void FixedUpdate()
    {
        float speed = travellingSpeed * Time.deltaTime;
        if (m_Trajectoire)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_Hit.point, speed);
            if (Mathf.Abs(transform.position.magnitude - m_Hit.point.magnitude) <= m_HitThreshold)
            {
                m_Audio.PlayOneShot(m_Audio.clip);
                Debug.Log("EXPLOOOOOOSION !");
                Destroy(gameObject);
            }
        }
        else
        {   
            transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.forward * fireballRange), speed);
        }
    }

}
