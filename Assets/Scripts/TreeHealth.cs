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
        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(GameObject.Find("BasicEnemy(Clone)"));
        Destroy(gameObject);
        
    }
}
