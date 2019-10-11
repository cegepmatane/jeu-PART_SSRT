using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private int m_Mana;
    [SerializeField]
    private float m_castingCooldown = 3f;

    [SerializeField]
    private GameObject m_spikeSpell;
    private int m_spikeCost = 10;

    private Camera m_camera;
    [SerializeField]
    private Canvas m_canvas;

    private void Start()
    {
        m_Mana = 100;
        m_camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        //Clic gauche souris
        if (Input.GetMouseButtonDown(0))
        {
            if (m_Mana >= m_spikeCost)
            {
                Instantiate(m_spikeSpell, transform.position, Quaternion.LookRotation(m_camera.transform.forward));
                m_Mana -= m_spikeCost;
            }
        }
    }

}
