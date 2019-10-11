using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float a_damage)
    {
        currentHP -= a_damage;
        Color t_Color = GetComponent<MeshRenderer>().material.color;
        float t_Greyscale = currentHP/maxHP;
        t_Color.a = t_Greyscale;
        Debug.Log("Greyscale = " + t_Greyscale);
        GetComponent<MeshRenderer>().material.color = new Color(t_Greyscale,t_Greyscale,t_Greyscale);
        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        
    }
}
