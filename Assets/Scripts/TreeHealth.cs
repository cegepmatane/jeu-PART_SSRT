using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public int Order;
    private bool IsAlive = true;
    private bool m_IsHealing = false;
    private float currentHP;
    void Awake()
    {
        GameManager.Instance.AddTree(this.gameObject);
        currentHP = maxHP;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
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
            Color t_color = t_mesh.material.GetColor("_EmissionColor");
            //Debug.Log(t_color);
            float t_Greyscale = currentHP / maxHP;
            t_color.a = t_Greyscale;
            //Debug.Log("Greyscale = " + t_Greyscale);
            
            
            t_mesh.material.SetColor("_EmissionColor", Color.magenta * t_Greyscale);
            
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

    private void Die()
    {
        if (IsAlive)
        {
            GameManager.Instance.KillTree(this.gameObject);
            IsAlive = false;
        }  
    }
}
