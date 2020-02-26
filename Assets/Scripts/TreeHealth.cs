using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public int Order;
    private bool IsAlive = true;
    private bool m_IsHealing = false;
    [ColorUsage(false, true)]
    public Color FocusedColor;
    [ColorUsage(false, true)]
    public Color UnfocusedColor;
    private float currentHP;


    void Awake()
    {
        GameManager.Instance.AddTree(this.gameObject);
        currentHP = maxHP;
    }
    // Start is called before the first frame update
    void Start()
    {
        UnfocusedColor = gameObject.GetComponentsInChildren<MeshRenderer>()[0].material.GetColor("_EmissionColor");
        
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
            return !IsAlive;
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

        RefreshColor();

        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void HealDamage()
    {
        currentHP += 1f;

        //Debug.Log("Regénération:" + currentHP + "/" + maxHP);

        RefreshColor();

        if(currentHP >= maxHP)
        {
            currentHP = maxHP;
            IsAlive = true;
            GameManager.Instance.ReviveTree(this.gameObject);
        }
        
    }

    private void RefreshColor()
    {
        MeshRenderer[] t_Meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer t_mesh in t_Meshes)
        {
            t_mesh.material.SetColor("_EmissionColor", Color.Lerp(UnfocusedColor, FocusedColor, currentHP / maxHP));
        }
    }

    public IEnumerator FadeTreeColor(Color a_NewColor, float a_Duration)
    {
        float t_ElapsedTime = 0f;


        MeshRenderer[] t_Meshes = GetComponentsInChildren<MeshRenderer>();

        Color t_InitialColor = gameObject.GetComponentInChildren<MeshRenderer>().material.GetColor("_EmissionColor");
        while (t_ElapsedTime / a_Duration <= 1)
        {
            float t = t_ElapsedTime / a_Duration;
            
            foreach (MeshRenderer t_Mesh in t_Meshes)
            {
                t_Mesh.material.SetColor("_EmissionColor", Color.Lerp(t_InitialColor, a_NewColor, t));
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
