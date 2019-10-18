using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private float m_travellingSpeed = 20f;
    private Camera m_cam;
    private RaycastHit m_hit;
    private Ray m_camRay;

    private void Awake()
    {
        m_cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        m_camRay = new Ray(m_cam.transform.position, m_cam.transform.forward);

    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(m_camRay, out m_hit, 100))
        {
            transform.Translate(m_hit.transform.position * Time.deltaTime);
        }
    }
}
