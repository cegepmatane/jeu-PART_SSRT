using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illuminate : MonoBehaviour
{
    private Vector3 m_SphereSize;
    private Vector3 m_MaxSize;
    private Vector3 m_MinSize;
    private Color m_GlowingColor;
    private float m_GlowingIntensity;
    private float m_MinIntensity;
    private float m_MaxIntensity;

    private bool m_IsGrowing;
    public float fluctuationSpeed = 0.1f;
    public float speed = 2f;
    public float lifetime = 5f;
    

    // Start is called before the first frame update
    void Start()
    {
        m_MinSize = transform.localScale;
        m_MaxSize = transform.localScale * 2f;
        m_SphereSize = m_MinSize;

        m_GlowingColor = GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
        m_GlowingIntensity = 0f;
        m_MinIntensity = 1.0f;
        m_MaxIntensity = 1.2f;

        GetComponent<Light>().intensity = 0f;

        m_IsGrowing = true;
        StartCoroutine("Fluctuation");

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (GameManager.Instance.LightFlower == null)
        { 
            transform.position += Vector3.up * Time.deltaTime * speed;
        } else
        {
            Vector3 t_FlowerPosition = GameManager.Instance.LightFlower.transform.position;
            Vector3 t_Target = new Vector3(t_FlowerPosition.x, t_FlowerPosition.y + 5, t_FlowerPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, t_Target, (speed * 2) * Time.deltaTime);
            
        }
    }

    private IEnumerator Fluctuation()
    {
        while (true)
        {
            if (m_GlowingIntensity < (m_MaxIntensity + m_MinIntensity) /2)
            {
                GetComponent<Light>().intensity = m_GlowingIntensity += 0.1f;
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", m_GlowingColor * m_GlowingIntensity);
                
                yield return null;
            }

            if ((transform.localScale.magnitude < m_MaxSize.magnitude) && m_IsGrowing)
            {
                m_GlowingIntensity += 0.01f;
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", m_GlowingColor * m_GlowingIntensity);
                transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
            }
            else if((transform.localScale.magnitude > m_MinSize.magnitude) && !m_IsGrowing)
            {
                m_GlowingIntensity -= 0.01f;
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", m_GlowingColor * m_GlowingIntensity);
                transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            }
            else
            {
                m_IsGrowing = !m_IsGrowing;
            }

            yield return new WaitForSeconds(fluctuationSpeed);
        }
    }
}
