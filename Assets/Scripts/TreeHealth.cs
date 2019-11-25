using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public int Order;
    private bool IsAlive = true;
    private bool m_IsHealing = false;
    private Color m_FocusedColor = Color.magenta;
    private Color m_UnfocusedColor;
    private float currentHP;
    void Awake()
    {
        GameManager.Instance.AddTree(this.gameObject);
        currentHP = maxHP;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_UnfocusedColor = gameObject.GetComponentsInChildren<MeshRenderer>()[0].material.GetColor("_EmissionColor");
        
        //Debug.Log("HP MAX: " + maxHP + " / HP CURRENT: " + currentHP);
    }

    public bool IsHurt
    {
        get
        {
            //Debug.Log("HP MAX: " + maxHP + " / HP CURRENT: " + currentHP);
            if (currentHP < maxHP)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }

    public bool IsDead
    {
        get
        {
            if (!IsAlive)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }

    public bool IsHealing
    {
        get
        {
            return m_IsHealing;
        }

        set
        {
            m_IsHealing = value;
        }


    }



    public void ApplyDamage(float a_damage)
    {
        currentHP -= a_damage;
       
        MeshRenderer[] t_Meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer t_mesh in t_Meshes)
        {
            Color t_Color = t_mesh.material.GetColor("_EmissionColor");
            //Debug.Log(t_color);
            float t_Greyscale = currentHP / maxHP;
            t_Color.a = t_Greyscale;
            //Debug.Log("Greyscale = " + t_Greyscale);
            
            
            t_mesh.material.SetColor("_EmissionColor", m_FocusedColor * t_Greyscale);
            
            //t_mesh.material.SetColor("_EmissionColor", t_color * t_Greyscale);


        }

        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void HealDamage()
    {
        currentHP += 1f;
        MeshRenderer[] t_Meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer t_mesh in t_Meshes)
        {
            Color t_color = t_mesh.material.GetColor("_EmissionColor");
            float t_Greyscale = currentHP / maxHP;
            Debug.Log("Regénération:" + currentHP + "/" + 100 * t_Greyscale + "%");
            //t_color.a = t_Greyscale;
            //Debug.Log("Greyscale = " + t_Greyscale);
            t_mesh.material.SetColor("_EmissionColor", Color.magenta * t_Greyscale);
        }

        if(currentHP >= maxHP)
        {
            currentHP = maxHP;
            IsAlive = true;
            GameManager.Instance.ReviveTree(this.gameObject);
        }
        
    }

    public IEnumerator FadeTreeColor(Color a_NewColor, float a_Duration)
    {
        //Debug.Log("AAAAAAAAAAA");
        float t_ElapsedTime = 0f;
        List<Transform> t_RenderList = new List<Transform>();
        foreach (Transform child in gameObject.transform)
        {
            t_RenderList.Add(child);
        }
        Color t_InitialColor = gameObject.GetComponentInChildren<MeshRenderer>().material.color;
        while (t_ElapsedTime / a_Duration < 1)
        {
            float t = t_ElapsedTime / a_Duration;
            if (t > 1)
            {
                t = 1;
            }
            foreach (Transform child in t_RenderList)
            {
                child.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(t_InitialColor, a_NewColor, t));
            }
            //m_Waves[0].TargetTree.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(t_InitialColor, a_NewColor, t));
            t_ElapsedTime += Time.deltaTime;
            //Debug.Log(t_ElapsedTime);
            yield return null;
        }

        yield break;
    }

    private void Die()
    {
        if (IsAlive)
        {
            GameManager.Instance.KillTree(this.gameObject);
            IsAlive = false;
        }  
    }
}
