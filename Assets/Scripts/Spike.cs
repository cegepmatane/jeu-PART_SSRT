using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private float m_travellingSpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * m_travellingSpeed);
    }
}
