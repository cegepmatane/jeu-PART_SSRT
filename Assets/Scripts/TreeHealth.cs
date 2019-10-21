using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public int Order;
    private bool IsAlive = true;
    private float currentHP;
    void Awake()
    {
        GameManager.Instance.AddTree(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsHurt
    {
        get
        {
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
            if (currentHP <= 0)
            {
                currentHP = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }



    public void ApplyDamage(float a_damage)
    {
        currentHP -= a_damage;
        Color t_Color = GetComponent<MeshRenderer>().material.color;
        float t_Greyscale = currentHP/maxHP;
        t_Color.a = t_Greyscale;
        //Debug.Log("Greyscale = " + t_Greyscale);
        GetComponent<MeshRenderer>().material.color = new Color(t_Greyscale,t_Greyscale,t_Greyscale);
        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void HealDamage()
    {
        currentHP += 1f;
        Color t_Color = GetComponent<MeshRenderer>().material.color;
        float t_Greyscale = currentHP / maxHP;
        Debug.Log("Regénération:" + currentHP + "/" + 100 * t_Greyscale + "%");
        t_Color.a = t_Greyscale;
        GetComponent<MeshRenderer>().material.color = new Color(t_Greyscale, t_Greyscale, t_Greyscale);
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
