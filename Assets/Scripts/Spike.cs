using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float travellingSpeed = 50f;
    public float spikeRange = 50f;
    private Camera m_cam;
    private RaycastHit m_hit;
    private bool m_isInRange;

    private Vector3 direction;

    private void Awake()
    {
        m_cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {

        m_isInRange = Physics.Raycast(m_cam.transform.position, m_cam.transform.forward, out m_hit, spikeRange) ? true : false;

        Destroy(gameObject, 2f);
    }

    private void FixedUpdate()
    {
        float speed = travellingSpeed * Time.deltaTime;
        if (m_isInRange)
            transform.position = Vector3.MoveTowards(transform.position, m_hit.point, speed);
        else
            transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.forward*spikeRange), speed);


    }
}
