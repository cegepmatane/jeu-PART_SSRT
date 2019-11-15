using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float travellingSpeed = 50f;
    public float fireballRange = 50f;
    public int fireballDamage = 50;
    private bool m_Trajectoire;

    [SerializeField] private GameObject m_ExplosionEffect;

    void Start()
    {
        //m_Trajectoire = Physics.Raycast(m_Cam.transform.position, m_Cam.transform.forward, out m_Hit, fireballRange);
        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        float speed = travellingSpeed * Time.deltaTime;
        transform.position += transform.forward * speed;

        /*if (m_Trajectoire)
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
        {   */
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.forward * fireballRange), speed);

        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //L'explosion gère les dégâts et le son d'explosion
        GameObject t_Explosion = Instantiate(m_ExplosionEffect, transform.position, Quaternion.identity);

        //Debug.Log("EXPLOOOOOOSION !");
        Destroy(gameObject);
    }

}
